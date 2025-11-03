using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Region
{
    public int Codigo { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Comuna> Comunas { get; set; } = new List<Comuna>();
}
