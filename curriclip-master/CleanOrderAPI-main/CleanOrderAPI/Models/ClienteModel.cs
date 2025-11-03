namespace CleanOrderAPI.Models
{
    public class ClienteModel
    {
        public required string Rut { get; set; }
        public required string Dv { get; set; }
        public required string RazonSocial { get; set; }
        public required string Correo { get; set; }
        public string? Telefono { get; set; }
        public required string Activo { get; set; }
    }
}
