query
=====

A small library that transforms IQueryable objects and a set of ASP.NET WebForms controls that extend GridView with filtering features.

Basic projection
----------------
Let's say you have this Employee class:  
```csharp
public class Empleado  
   {  
      public string Name { get; set; }  
      public string LastName { get; set; }  
    }
```

You create a Query object like this:
```csharp
var query = new Query<Employee>();
query.AddField("FullName", x => x.Name + " " + x.LastName); // FieldName = "FullName"
```

Then take an IEqueryable<Employee> and apply the projection:
```csharp
IQueryable<Employee> employees = ...
IQueryable theResult = query.Project(employees);
```
theResult is an untyped IQueryable. The ElementType is an annonymous type based in the defined fields.

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
