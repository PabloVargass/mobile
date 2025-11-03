using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Reporte
{
    public int IdReporte { get; set; }

    public string? Observacion { get; set; }

    public int FkUsuario { get; set; }

    public int FkIdOrden { get; set; }

    public virtual Orden FkIdOrdenNavigation { get; set; } = null!;

    public virtual ICollection<ImagenesReporte> ImagenesReportes { get; set; } = new List<ImagenesReporte>();
}
