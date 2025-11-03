namespace CleanOrderAPI.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        public string Correo { get; set; } = null!;
        public int Activo { get; set; }
        public required string Rol { get; set; }
        public string? RutEmpleado { get; set; }
        public int RolId { get; set; }


        public bool AreFieldsValid()
        {
            return !string.IsNullOrEmpty(Correo) && Activo >= 0 && RolId >= 0;
        }
    }
    public class PasswordChangeRequest
    {
        public required string correo { get; set; }
        public required string oldPassword { get; set; }
        public required string newPassword { get; set; }
    }

    public class CreateUserRequest
    {
        public string Correo { get; set; } = null!;
        public int Activo { get; set; }
        public int RolId { get; set; }
        public required string Password { get; set; }
    }

    public class UpdateUserRequest
    {
        public string correo { get; set; } = null!;
        public int activo { get; set; }
        public int rolId { get; set; }
    }

}
