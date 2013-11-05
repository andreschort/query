query
=====

query can:
- Transform an IQueryable<T> with projections into an anonymous type, apply filters and multiple sortings. The IQueryable can be anything, from a list in memory to an Entity Framework query.
- Easily create a GridView which gets the data from an IQueryable using the transformations provided.

You can find an example application in https://github.com/andreschort/query-sample

Demo
----
![demo](https://raw.github.com/andreschort/query/master/query-sample.gif)

TODO
----

Query.Core
- Support for compiled queries
 - Filter values and sortings as parameters.
 - Research: http://msdn.microsoft.com/en-us/library/bb896297.aspx
- Support for usage stadistics (ie. how often a combination of filters and sortings is used).
- Support for arbitrary filter expressions ie. ">10 AND !=15".
- Extend operators to any target type.

Query.Web
- TD Tooltip
- TH y TD CssClass
- Improve the way you can apply styles to the Query.Web controls.
- New field types:
 - Autocomplete (from a service or automatically from the Query - if posible)
 - DropDownList with multiple options (Query.Core currently supports many filter values with an OR conjunction)
- Calendar button option for DateField.

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
query.AddField("FullName").Select(x => x.Name + " " + x.LastName); // FieldName = "FullName"

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
If you want to filter the IQueryable<Employee> by some user-entered value.

The filters values are defined using a Dictionary<string, string>. The key is the name of the field and the value is the filter value. You can use a set of operators to filter.
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

Advance filtering
-----------------
You can use a small set of operators to filter. The kind of expressions you can use to filter depends on the type of the property that the filter is targeting. This can be overriden using the FilterAsX methods.

For System.String you can use the wildcard `%` at the start, end or both to create 'like' expressions. For example:
- `%someText`: Will return the values that ends with "someText".
- `someText%`: Will return the values that start with "someText".
- `%someText`: Will return the values that contains "someText".

The filter is by default case insensitive but you can override this while creating the query object.

For numeric types (int, double, decimal, etc) you can use these operators:
- `=2`: This will return the values equal to 2 (this is the default operator)
- `>2`: Greater than..
- `>=2`: Great or equal to..
- `<2`: Smaller than..
- `<=2`: Samll or equal than to..
- `2|3`: Between.. (this is the pipe character `|`)

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
- __GridViewId__ (string - required): The ID of the GridView.
- __Placeholder__ (string - optional): The placeholder to show in the filter UI elements.
- __AutoFilterDelay__ (int - optional): The milliseconds to wait before automatically triggering the search after the user changes a filter.
- __EnableFilters__ (bool - optional - default=true): Determines wheather the filter elements are shown (default: True)
- __OnFilter__ (event): Triggered when the user changes a filter or hits the enter key while the focus is in a filter.
- __OnSort__ (event): Triggered when the user clicks on a header's title.

The `GridExtender` exposes all the current filters and sortings through two properties:
- Filters (`Dictionary<string, string>`): The key is the field's name and the value is the filter value. The dictionary will hold as many fields as you define, empty filters will have an empty string as the value.
- Sortings (`List<KeyValuePair<string, SortDirection>>`): The key is the field's name and the value is the sort direction of the field. Only the fields which are currently sorting will be on the list.

There are three kinds of `DataControlField`'s:
- __TextField__: Renders an text field as the filter UI element.
- __DateField__: Renders two text fields as the filter UI elements (we need two elements to filter by a range of dates). Both elements have jquery datepicker.
- __DropDownField__: Renders a select as the filter UI element.
- __DynamicField__: Enables to programmatically set the field type. Uses one of the above fields internally.

All fields have a anchor as title above them which when clicked triggers the `Sort` event in `GridExtender`.
The `Filter` event is triggered by hitting the enter key on a field's filter UI element or automatically by the AutoFilterDelay parameter.

These are the properties common to all fields:
- __Name__ (string - required): The name of the field. Determines the value shown and is the name used to identify the field's filter value and sorting direction in the `GridExtender` Filter and Sorting properties as explained above.
- __Placeholder__ (string - optional): The placeholder to show in the filter UI element.
- __UrlFormat__ (string - optional): Turns the text in the cell into a link. The string is used as the url of the link.
- __UrlFields__ (string, optional): A comma separated list of properties to use with UrlFormat. This allows to generate an url customized for each row.
- __AutoFilterDelay__ (int - optional): Same as GridExtender but affects only this field.
- __ItemEnabled__ (bool - optional - default=true): Set to false when you want to disable everything inside the data cell.
- __ItemTemplate__ (ITemplate - optional): Enables you to define the full content of the data cells (similar to TemplateField.ItemTemplate).
- __Format__ (string - optional): Used to format the value shown in the data cell. Can be any standard format strings.
- __FormatDelegate__ (Func<object, string> - optional): Use this when you want full control over how the value show inside the data cell is formatted. Overrides __Format__.
- __Click__ (Event - optional): Makes the data cell show a link with a postback event. Overrides __UrlFormat__.

The following are properties specific to DropDownField:
- __Items__ (List<ListItem> - required): Describes the list of options to put in the select element. Do not include the empty option, it is inserted automatically.

This is an example of a GridView using the `DataControlField`'s provided:

```aspx
<asp:GridView ID="GridView" runat="server" AutoGenerateColumns="False" ShowHeaderWhenEmpty="True"
              AllowPaging="True" AllowSorting="False" PageSize="8">
    <Columns>
        <query:TextField Name="Nombre" HeaderText="Nombre"
                         UrlFormat="https://www.google.com.ar/search?q={0} {1}" UrlFields="Nombre, Apellido" />
        <query:TextField Name="Apellido" HeaderText="Apellido" />
        <query:TextField Name="Dni" HeaderText="Dni" />
        <query:DropDownField Name="EstadoCivil" HeaderText="Estado civil" />
        <query:TextField Name="Edad" HeaderText="Edad" />
        <query:TextField Name="Salario" HeaderText="Salario" />
        <query:DateField Name="FechaNacimiento" HeaderText="Fecha Nacimiento" Format="d" />
        <query:TextField Name="AttachmentCount" HeaderText="Number of attachments" />
    </Columns>
</asp:GridView>
```
