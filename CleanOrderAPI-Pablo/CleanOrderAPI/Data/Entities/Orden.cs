using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Orden
{
    public int IdOrden { get; set; }

    public int HorasTrabajo { get; set; }

    public DateTime FechaRegistro { get; set; }

    public DateTime FechaAgendada { get; set; }

    public DateTime FechaFinalizado { get; set; }

    public string? Observacion { get; set; }

    public string Direccion { get; set; } = null!;

    public int Folio { get; set; }

    public int FkComuna { get; set; }

    public string FkRutClientes { get; set; } = null!;

    public string FkPatente { get; set; } = null!;

    public int FkEstado { get; set; }

    public virtual Comuna FkComunaNavigation { get; set; } = null!;

    public virtual OrdenEstado FkEstadoNavigation { get; set; } = null!;

    public virtual Vehiculo FkPatenteNavigation { get; set; } = null!;

    public virtual Cliente FkRutClientesNavigation { get; set; } = null!;

    public virtual ICollection<Reporte> Reportes { get; set; } = new List<Reporte>();
}
