using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class OrdenEmpleado
{
    public int FkIdOrdenes { get; set; }

    public string FkRutEmpleado { get; set; } = null!;

    public virtual Orden FkIdOrdenesNavigation { get; set; } = null!;

    public virtual Empleado FkRutEmpleadoNavigation { get; set; } = null!;
}
