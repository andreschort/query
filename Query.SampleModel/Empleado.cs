using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Query.SampleModel
{
    public class Empleado
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int Dni { get; set; }

        public int Edad { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public decimal Salario { get; set; }

        public int EstadoCivil_Id { get; set; }

        [NotMapped]
        public EstadoCivil EstadoCivil
        {
            get { return (EstadoCivil)this.EstadoCivil_Id; }
            set { this.EstadoCivil_Id = (int)value; }
        }
    }
}
