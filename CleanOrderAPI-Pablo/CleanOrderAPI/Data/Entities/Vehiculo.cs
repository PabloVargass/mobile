using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Vehiculo
{
    public string Patente { get; set; } = null!;

    public int Capacidad { get; set; }

    public string Activo { get; set; } = null!;

    public int FkTipo { get; set; }

    public virtual TipoCarga FkTipoNavigation { get; set; } = null!;

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
