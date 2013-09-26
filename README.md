query
=====

A small library that transforms IQueryable objects and a set of ASP.NET WebForms controls that extend GridView with filtering features.
Look for the project Query.Sample.WebSite40 for a full example.

Basic projection
----------------
Let's say you have this Employee class:  
```csharp
public class Employee  
{  
  public string Name { get; set; }  
  public string LastName { get; set; }  
}
```

You create a Query object like this:
```csharp
var query = new Query<Employee>();
query.AddField("FullName", x => x.Name + " " + x.LastName); // FieldName = "FullName"

var employees = //some IQueryable<Employee>
IQueryable theResult = query.Project(employees);
```
theResult is an untyped IQueryable. The ElementType is an annonymous type of the form:
```chsarp
public class FullName;String;
{
  public string FullName;
}
```

Basic filtering
---------------
If you want to filter the IQueryable<Employee> by some user-entered value (for example):
```csharp
IQueryable<Employee> employees = ...
var filters = new Dictionary<string, string> {{"FullName", "Car%"}};   // '%' acts as the wildcard
employees = query.Filter(employees, filters);
```
This is equivalent to:
```csharp
employees = employees.Where(x => x.LastName.StartsWith("Car"));
```

Basic sorting
-------------
You can also apply sorting to an IQueryable pretty much the same way that you apply filters:
```csharp
var sortings = new List<KeyValuePair<string, SortDirection>>{
  new KeyValuePair<string, SortDirection>("FullName", SortDirection.Descending)
}
employees = query.OrderBy(employees, sortings);
```
This is equivalent to:
```csharp
employees = employees.OrderByDescending(x => x.Name + " " + x.LastName);
```
Multiple sortings are supported.

WebForms controls
=================
Query includes a set of `DataControlField` classes that enables a GridView with filtering fields right below each header. Multiple sorting is also supported.
This project relies on jquery so you should include it in your page.

The main entry point to this project is the GridExtender control:
```aspx
<query:GridExtender ID="GridExtender" runat="server"
                    GridViewId="GridView"
                    AutoFilterDelay="2000"
                    EnableFilters="True"
                    OnFilter="GridExtender_Filter"
                    OnSort="GridExtender_Sort" />
```
These are the more important properties:
- GridViewId (string - required): The ID of the GridView.
- AutoFilterDelay (int - optional): The milliseconds to wait before automatically triggering the search after the user changes a filter.
- EnableFilters (bool - optional): Determines wheather the filter elements are shown (default: True)
- OnFilter (event): Triggered when the user changes a filter or hits the enter key while the focus is in a filter.
- OnSort (event): Triggered when the user clicks on a header's title.

There are three kinds of `DataControlField`'s:
- TextField: Renders an text field as the filter UI element.
- DateField: Renders two text fields as the filter UI elements (we need two elements to filter by a range of dates). Both elements have jquery datepicker.
- DropDownField: Renders a select as the filter UI element.

All fields have a anchor as title above them which when clicked triggers sorting on that field.

These are the properties in common between the three fields:
- Name (string - required): The name of the field. Used to identify the field's filter value and sorting direction (if any)
- DataField (string - required): The name of the property bound to the field.The field uses this to get the value to display in each cell.
- FilterPlaceholder (string - optional): The placeholder to show in the filter UI element.
- UrlFormat (string - optional): Turns the text in the cell into a link. The string is used as the url of the link.
- UrlFields (string, optional): A comma separated list of properties to use with UrlFormat. This allows to generate an url customized for each row.

This is an example of a GridView using the `DataControlField`'s provided:

```aspx
<asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True"
              AllowPaging="True" AllowSorting="False" PageSize="8">
    <Columns>
        <query:TextField Name="Nombre" HeaderText="Nombre" DataField="Nombre"
                         UrlFormat="https://www.google.com.ar/search?q={0} {1}" UrlFields="Nombre, Apellido" />
        <query:TextField Name="Apellido" HeaderText="Apellido" DataField="Apellido" />
        <query:TextField Name="Dni" HeaderText="Dni" DataField="Dni" />
        <query:DropDownField Name="EstadoCivil" HeaderText="Estado civil" DataField="EstadoCivil" DefaultValue="-1" />
        <query:TextField Name="Edad" HeaderText="Edad" DataField="Edad" />
        <query:TextField Name="Salario" HeaderText="Salario" DataField="Salario" />
        <query:DateField Name="FechaNacimiento" HeaderText="Fecha Nacimiento" DataField="FechaNacimiento" Format="d" />
        <query:TextField Name="AttachmentCount" HeaderText="Number of attachments" DataField="AttachmentCount" />
    </Columns>
</asp:GridView>
```
