using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Data.Entities;

public partial class Cliente
{
    public string RutCliente { get; set; } = null!;

    public string Dv { get; set; } = null!;

    public string RazonSocial { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Telefono { get; set; }

    public string Activo { get; set; } = null!;

    public virtual ICollection<Documento> Documentos { get; set; } = new List<Documento>();

    public virtual ICollection<Orden> Ordens { get; set; } = new List<Orden>();
}
