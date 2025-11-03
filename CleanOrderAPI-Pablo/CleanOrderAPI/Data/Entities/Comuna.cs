using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Comuna
{
    public int Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public int FkCodigoRegion { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual Region FkCodigoRegionNavigation { get; set; } = null!;

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
