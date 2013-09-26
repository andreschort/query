using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Query.Sample.Model
{
    public class Empleado
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int Dni { get; set; }

        public int Edad { get; set; }

        public int? Cuit { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public decimal Salario { get; set; }

        public double AverageHourlyWage { get; set; }

        public int EstadoCivil_Id { get; set; }
        
        [NotMapped]
        public EstadoCivil EstadoCivil
        {
            get { return (EstadoCivil)this.EstadoCivil_Id; }
            set { this.EstadoCivil_Id = (int)value; }
        }

        public string Company { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Fax { get; set; }

        public string Email { get; set; }

        public string Web { get; set; }

        public Attachment Attachment { get; set; }
    }
}
