using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class OrdenEstado
{
    public int IdEstado { get; set; }

    public string? Nombre { get; set; }

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
