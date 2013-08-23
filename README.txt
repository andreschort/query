TODO's
======
[DONE]Disparar el filtrado con la tecla enter en DateField
[DONE]Auto filter en DateField
[DONE]Links en la celda de datos
[DONE]Ordenamiento
[DONE]Flechas de ordenamiento en las cabeceras
Estilos
Ejemplos con EntityFramework
MVC Helpers

Issues
======
Como manejar los nombres de attributes? Estan repetidos en codigo y en js
DateField: Como manejar el separador en los valores from y to del filtro? Ahora esta hardcodeado y repetido ";"
DropDownField: Default value

Ideas
=====
Desagregar QueryFieldBuilder?
	QueryFieldSelectBuilder
	QueryFieldWhereBuilder
	etc

de manera de tener API especifica para cada elemento. Ejemplos:
QueryFieldWhereBuilder
	TruncateTime: Tomaria la actual expression para Where y le aplicaria EntityFunctions.TruncateTime
	Implica referencia a System.Data.Objects en Query.csproj
	
fieldBuilder.Create(x => x.FechaNacimiento).Where().TruncateTime()
-------------------------------------------------------------------------------------------------------------------
