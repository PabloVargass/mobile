using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Activo { get; set; }

    public int FkIdRol { get; set; }

    public string? FkRutEmpleado { get; set; }

    public virtual Rol FkIdRolNavigation { get; set; } = null!;

    public virtual Empleado? FkRutEmpleadoNavigation { get; set; }
}
