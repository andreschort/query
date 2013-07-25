namespace Query.SampleModel
{
    public class Empleado
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public int Dni { get; set; }

        public int EstadoCivil_Id { get; set; }

        public EstadoCivil EstadoCivil
        {
            get { return (EstadoCivil)this.EstadoCivil_Id; }
            set { this.EstadoCivil_Id = (int)value; }
        }
    }
}
