using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class TipoCarga
{
    public int TipoCargaCodigo { get; set; }

    public string NombreCarga { get; set; } = null!;

    public virtual ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
