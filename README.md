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
-GridViewId (string - Required): The ID of the GridView.
-AutoFilterDelay (int - Optional): The milliseconds to wait before automatically triggering the search after the user changes a filter.
-EnableFilters (bool - Optional): Determines wheather the filter elements are shown (Default: True)
-OnFilter (Event): Triggered when the user changes a filter or hits the enter key while the focus is in a filter.
-OnSort (Event): Triggered when the user clicks on a header's title.

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
