namespace CleanOrderAPI.Models
{
    public class EmpleadoModel
    {
        public string Rut { get; set; } = null!;

        public string Dv { get; set; } = null!;

        public string Nombre { get; set; } = null!;

        public string Apellido { get; set; } = null!;

        public string? Direccion { get; set; }

        public string? Telefono { get; set; }

        public string Activo { get; set; } = null!;
        public int? IdComuna { get; set; }
        public string? NombreComuna { get; set; }
        public string? NombreRegion { get; set; }

    }
}
