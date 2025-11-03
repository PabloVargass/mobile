using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Empleado
{
    public string RutEmpleado { get; set; } = null!;

    public string Dv { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public string Activo { get; set; } = null!;

    public int? FkComuna { get; set; }

    public virtual Comuna? FkComunaNavigation { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
