http://www.google.com.ar|urlize

TODO's
======
[DONE]Disparar el filtrado con la tecla enter en DateField
[DONE]Auto filter en DateField
[DONE]Links en la celda de datos
Ordenamiento
Flechas de ordenamiento en las cabeceras
Estilos

Issues
======
Como manejar los nombres de attributes? Estan repetidos en codigo y en js
DateField: Como manejar el separador en los valores from y to del filtro? Ahora esta hardcodeado y repetido ";"

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

[DONE]
Facilidad para reducir este codigo
            query.Fields.Add(
                fieldBuilder.Create("EstadoCivil")
                            .Select(x => x.EstadoCivil_Id.Equals((int) EstadoCivil.Soltero) // When using EntityFramework without enum support
                                             ? "Soltero"
                                             : x.EstadoCivil_Id.Equals((int) EstadoCivil.Casado)
                                                   ? "Casado"
                                                   : x.EstadoCivil_Id.Equals((int) EstadoCivil.Separado)
                                                         ? "Separado"
                                                         : x.EstadoCivil_Id.Equals((int) EstadoCivil.Divorciado)
                                                               ? "Divorciado"
                                                               : x.EstadoCivil_Id.Equals((int) EstadoCivil.Viudo)
                                                                     ? "Viudo"
                                                                     : string.Empty)
																	 
Se me ocurre algo similar al filtrado con When.
fieldBuilder.Create("EstadoCivil")
			.Select(x => x.EstadoCivil_Id)
			.SelectWhen((int) EstadoCivil.Soltero, "Soltero")
			etc