using System;
using System.Collections.Generic;

namespace CleanOrderAPI.Models
{
    public class OrdenForm
    {
        public int RutCliente { get; set; } // En BD puede ser Rut(string) pero sigue el contrato TS
        public required int HorasTrabajo { get; set; }
        public required DateTime FechaRegistro { get; set; }
        public required DateTime FechaAgendada { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public required string Direccion { get; set; } = string.Empty;
        public required int Folio { get; set; } 
        public required int IdComuna { get; set; }
        public required int IdEstado { get; set; }
        public required string patenteVehiculo { get; set; }
        public List<EmpleadoAsignar>? EmpleadoAsignar { get; set; }
    }

    // empleadoAsignar TS
    public class EmpleadoAsignar
    {
        public required string Rut { get; set; }
        public required string Dv { get; set; }
        public required string Nombre { get; set; }
    }

    // OrdenTrabajo TS (usa ComunaModel y RegionModel ya definidos en ComunaRegionModel.cs)
    public class OrdenTrabajoModel
    {
        public int Id { get; set; }
        public int HorasTrabajo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public DateTime FechaAgendada { get; set; }
        public DateTime FechaFinalizado { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Folio { get; set; } = string.Empty;
        public ComunaModel Comuna { get; set; } = new() { Nombre = string.Empty, RegionId = 0 };
        public RegionModel Region { get; set; } = new() { Nombre = string.Empty };
        public int IdCliente { get; set; }
        public string Cliente { get; set; } = string.Empty; // Nombre del cliente
        public int IdEstado { get; set; }
        public string Estado { get; set; } = string.Empty; // Descripción del estado
    }
}
