using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanOrderAPI.Controllers
{
    [Route("ordenes-trabajo")] // Base path expected by Angular service
    [ApiController]
    [Authorize(Roles = "1")] // Ajustar roles según necesidad
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public OrdenesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /ordenes-trabajo/folio/{folio}
        [HttpGet("folio/{folio}")]
        public async Task<ActionResult<string>> FolioExiste(string folio)
        {
            if (!int.TryParse(folio, out int folioNum))
                return BadRequest("Folio inválido");

            Orden? existeEntity = await _context.Ordens.AsNoTracking().FirstOrDefaultAsync(o => o.Folio == folioNum);

            if (existeEntity is null)
                return Ok(string.Empty);
            else
                return Ok(existeEntity.Folio);

        }

        // GET: /ordenes-trabajo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenTrabajoModel>>> GetAll()
        {
            var data = await _context.Ordens
                .AsNoTracking()
                .Include(o => o.FkComunaNavigation).ThenInclude(c => c.FkCodigoRegionNavigation)
                .Include(o => o.FkEstadoNavigation)
                .Include(o => o.FkRutClientesNavigation)
                .Select(o => new
                {
                    o.IdOrden,
                    o.HorasTrabajo,
                    o.FechaRegistro,
                    o.FechaAgendada,
                    o.Observacion,
                    o.Direccion,
                    o.Folio,
                    Comuna = o.FkComunaNavigation,
                    Region = o.FkComunaNavigation.FkCodigoRegionNavigation,
                    o.FkRutClientes,
                    ClienteNombre = o.FkRutClientesNavigation.RazonSocial,
                    EstadoId = o.FkEstadoNavigation.IdEstado,
                    EstadoNombre = o.FkEstadoNavigation.Nombre
                })
                .ToListAsync();

            List<OrdenTrabajoModel> list = data.Select(o => new OrdenTrabajoModel
            {
                Id = o.IdOrden,
                HorasTrabajo = o.HorasTrabajo,
                FechaRegistro = o.FechaRegistro,
                FechaAgendada = o.FechaAgendada,
                Observaciones = o.Observacion ?? string.Empty,
                Direccion = o.Direccion,
                Folio = o.Folio.ToString(),
                Comuna = new ComunaModel { Id = o.Comuna.Codigo, Nombre = o.Comuna.Nombre, RegionId = o.Comuna.FkCodigoRegion },
                Region = new RegionModel { Id = o.Region.Codigo, Nombre = o.Region.Nombre },
                IdCliente = int.TryParse(o.FkRutClientes, out var tmpRut) ? tmpRut : 0,
                Cliente = o.ClienteNombre,
                IdEstado = o.EstadoId,
                Estado = o.EstadoNombre ?? string.Empty
            }).ToList();

            return Ok(list);
        }

        // GET: /ordenes-trabajo/cliente/{idCliente}
        [HttpGet("cliente/{idCliente}")]
        public async Task<ActionResult<IEnumerable<OrdenTrabajoModel>>> GetByCliente(int idCliente)
        {
            var data = await _context.Ordens
                .AsNoTracking()
                .Include(o => o.FkComunaNavigation).ThenInclude(c => c.FkCodigoRegionNavigation)
                .Include(o => o.FkEstadoNavigation)
                .Include(o => o.FkRutClientesNavigation)
                .Select(o => new
                {
                    o.IdOrden,
                    o.HorasTrabajo,
                    o.FechaRegistro,
                    o.FechaAgendada,
                    o.Observacion,
                    o.Direccion,
                    o.Folio,
                    Comuna = o.FkComunaNavigation,
                    Region = o.FkComunaNavigation.FkCodigoRegionNavigation,
                    o.FkRutClientes,
                    ClienteNombre = o.FkRutClientesNavigation.RazonSocial,
                    EstadoId = o.FkEstadoNavigation.IdEstado,
                    EstadoNombre = o.FkEstadoNavigation.Nombre
                })
                .ToListAsync();

            var list = data
                .Where(o => int.TryParse(o.FkRutClientes, out var rutParsed) && rutParsed == idCliente)
                .Select(o => new OrdenTrabajoModel
                {
                    Id = o.IdOrden,
                    HorasTrabajo = o.HorasTrabajo,
                    FechaRegistro = o.FechaRegistro,
                    FechaAgendada = o.FechaAgendada,
                    Observaciones = o.Observacion ?? string.Empty,
                    Direccion = o.Direccion,
                    Folio = o.Folio.ToString(),
                    Comuna = new ComunaModel { Id = o.Comuna.Codigo, Nombre = o.Comuna.Nombre, RegionId = o.Comuna.FkCodigoRegion },
                    Region = new RegionModel { Id = o.Region.Codigo, Nombre = o.Region.Nombre },
                    IdCliente = idCliente,
                    Cliente = o.ClienteNombre,
                    IdEstado = o.EstadoId,
                    Estado = o.EstadoNombre ?? string.Empty
                })
                .ToList();
            return Ok(list);
        }

        // GET: /ordenes-trabajo/estado
        [HttpGet("estado")]
        public async Task<ActionResult<IEnumerable<OrdenEstadoModel>>> GetEstados()
        {
            var estados = await _context.OrdenEstados
                .AsNoTracking()
                .Select(e => new OrdenEstadoModel { Id = e.IdEstado, Nombre = e.Nombre ?? string.Empty })
                .ToListAsync();
            return Ok(estados);
        }

        // GET: /ordenes-trabajo/fechas?inicio=yyyy-MM-dd&fin=yyyy-MM-dd
        [HttpGet("fechas")]
        public async Task<ActionResult<IEnumerable<OrdenTrabajoModel>>> GetByFechas([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            if (inicio == default || fin == default || fin < inicio)
                return BadRequest("Rango de fechas inválido.");

            var data = await _context.Ordens
                .AsNoTracking()
                .Where(o => o.FechaAgendada.Date >= inicio.Date && o.FechaAgendada.Date <= fin.Date)
                .Include(o => o.FkComunaNavigation).ThenInclude(c => c.FkCodigoRegionNavigation)
                .Include(o => o.FkEstadoNavigation)
                .Include(o => o.FkRutClientesNavigation)
                .Select(o => new
                {
                    o.IdOrden,
                    o.HorasTrabajo,
                    o.FechaRegistro,
                    o.FechaAgendada,
                    o.Observacion,
                    o.Direccion,
                    o.Folio,
                    Comuna = o.FkComunaNavigation,
                    Region = o.FkComunaNavigation.FkCodigoRegionNavigation,
                    o.FkRutClientes,
                    ClienteNombre = o.FkRutClientesNavigation.RazonSocial,
                    EstadoId = o.FkEstadoNavigation.IdEstado,
                    EstadoNombre = o.FkEstadoNavigation.Nombre
                })
                .ToListAsync();

            var list = data.Select(o => new OrdenTrabajoModel
            {
                Id = o.IdOrden,
                HorasTrabajo = o.HorasTrabajo,
                FechaRegistro = o.FechaRegistro,
                FechaAgendada = o.FechaAgendada,
                Observaciones = o.Observacion ?? string.Empty,
                Direccion = o.Direccion,
                Folio = o.Folio.ToString(),
                Comuna = new ComunaModel { Id = o.Comuna.Codigo, Nombre = o.Comuna.Nombre, RegionId = o.Comuna.FkCodigoRegion },
                Region = new RegionModel { Id = o.Region.Codigo, Nombre = o.Region.Nombre },
                IdCliente = int.TryParse(o.FkRutClientes, out var tmpRut) ? tmpRut : 0,
                Cliente = o.ClienteNombre,
                IdEstado = o.EstadoId,
                Estado = o.EstadoNombre ?? string.Empty
            }).ToList();
            return Ok(list);
        }

        // POST: /ordenes-trabajo
        [HttpPost]
        public async Task<ActionResult<OrdenTrabajoModel>> Create([FromBody] OrdenForm form)
        {
            try
            {
                if (form == null)
                    return BadRequest("Datos inválidos");

                // Generar folio simple (ejemplo) - idealmente usar secuencia/otra lógica
                //int folio = await _context.Ordens.MaxAsync(o => (int?)o.Folio) + 1 ?? 1;

                var entity = new Orden
                {
                    HorasTrabajo = form.HorasTrabajo,
                    FechaRegistro = form.FechaRegistro == default ? DateTime.UtcNow : form.FechaRegistro,
                    FechaAgendada = form.FechaAgendada,
                    FechaFinalizado = form.FechaAgendada.AddHours(form.HorasTrabajo),
                    Observacion = form.Observaciones,
                    Direccion = form.Direccion,
                    Folio = form.Folio,
                    FkComuna = form.IdComuna,
                    FkEstado = form.IdEstado,
                    FkRutClientes = form.RutCliente.ToString(),
                    FkPatente = _context.Vehiculos.Select(v => v.Patente).FirstOrDefault() ?? "00000000" // placeholder
                };

                _context.Ordens.Add(entity);
                await _context.SaveChangesAsync();
                entity = await _context.Ordens.FirstAsync(o => o.Folio == entity.Folio);

                // Asignar empleados si vienen
                if (form.EmpleadoAsignar != null && form.EmpleadoAsignar.Any())
                {
                    foreach (var emp in form.EmpleadoAsignar)
                    {
                        if (await _context.Empleados.AnyAsync(e => e.RutEmpleado == emp.Rut))
                        {
                            _context.OrdenEmpleados.Add(new OrdenEmpleado
                            {
                                FkIdOrdenes = entity.IdOrden,
                                FkRutEmpleado = emp.Rut
                            });
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return CreatedAtAction(nameof(GetAll), new { id = entity.IdOrden }, await ProjectOrden(entity.IdOrden));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        // PUT: /ordenes-trabajo/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<OrdenTrabajoModel>> Update(int id, [FromBody] OrdenForm form)
        {
            var entity = await _context.Ordens.FindAsync(id);
            if (entity == null) return NotFound();

            entity.HorasTrabajo = form.HorasTrabajo;
            entity.FechaAgendada = form.FechaAgendada;
            entity.Observacion = form.Observaciones;
            entity.Direccion = form.Direccion;
            entity.FkComuna = form.IdComuna;
            entity.FkEstado = form.IdEstado;
            entity.FkRutClientes = form.RutCliente.ToString();

            await _context.SaveChangesAsync();

            // Actualizar empleados (reemplazar)
            var actuales = _context.OrdenEmpleados.Where(oe => oe.FkIdOrdenes == id);
            _context.OrdenEmpleados.RemoveRange(actuales);
            if (form.EmpleadoAsignar != null)
            {
                foreach (var emp in form.EmpleadoAsignar)
                {
                    if (await _context.Empleados.AnyAsync(e => e.RutEmpleado == emp.Rut))
                    {
                        _context.OrdenEmpleados.Add(new OrdenEmpleado { FkIdOrdenes = id, FkRutEmpleado = emp.Rut });
                    }
                }
            }
            await _context.SaveChangesAsync();

            return await ProjectOrden(entity.IdOrden);
        }

        // DELETE: /ordenes-trabajo/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Ordens.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Ordens.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PATCH: /ordenes-trabajo/{id}/estado
        [HttpPatch("{id:int}/estado")]
        public async Task<ActionResult<OrdenTrabajoModel>> CambiarEstado(int id, [FromBody] dynamic body)
        {
            int idEstado;
            try { idEstado = (int)body.idEstado; } catch { return BadRequest("idEstado requerido"); }
            var entity = await _context.Ordens.FindAsync(id);
            if (entity == null) return NotFound();
            entity.FkEstado = idEstado;
            await _context.SaveChangesAsync();
            return await ProjectOrden(id);
        }

        // PATCH: /ordenes-trabajo/{id}/reagendar
        [HttpPatch("{id:int}/reagendar")]
        public async Task<ActionResult<OrdenTrabajoModel>> Reagendar(int id, [FromBody] dynamic body)
        {
            DateTime nuevaFecha;
            try { nuevaFecha = (DateTime)body.fechaAgendada; } catch { return BadRequest("fechaAgendada requerida"); }
            var entity = await _context.Ordens.FindAsync(id);
            if (entity == null) return NotFound();
            entity.FechaAgendada = nuevaFecha;
            await _context.SaveChangesAsync();
            return await ProjectOrden(id);
        }

        // POST: /ordenes-trabajo/empleados-disponibles
        // Body: { "fechaAgendada": "2025-01-01T00:00:00", "horasTrabajo": 4 }
        [HttpPost("empleados-disponibles")]
        public async Task<ActionResult<IEnumerable<EmpleadoAsignar>>> GetEmpleadosDisponibles([FromBody] EmpleadosDisponiblesRequest request)
        {
            if (request == null || request.HorasTrabajo <= 0 || request.FechaAgendada == default)
                return Ok(new List<EmpleadoAsignar>());

            // Intervalo solicitado
            DateTime inicio = request.FechaAgendada;
            DateTime fin = inicio.AddHours(request.HorasTrabajo);

            // Empleados ocupados: cualquier orden cuya ventana [FechaAgendada, FechaAgendada + HorasTrabajo) se solape con [inicio, fin)
            var ocupados = await (from o in _context.Ordens
                                  where o.FechaAgendada < fin
                                     && o.FechaAgendada.AddHours(o.HorasTrabajo) > inicio
                                  join oe in _context.OrdenEmpleados on o.IdOrden equals oe.FkIdOrdenes
                                  select oe.FkRutEmpleado)
                                 .Distinct()
                                 .ToListAsync();

            var disponibles = await _context.Empleados
                .AsNoTracking()
                .Where(e => e.Activo == "S" && !ocupados.Contains(e.RutEmpleado))
                .Select(e => new EmpleadoAsignar
                {
                    Rut = e.RutEmpleado,
                    Dv = e.Dv,
                    Nombre = e.Nombre + " " + e.Apellido,
                    Disponible = true
                })
                .ToListAsync();

            return Ok(disponibles);
        }

        private async Task<OrdenTrabajoModel> ProjectOrden(int id)
        {
            var o = await _context.Ordens
                .AsNoTracking()
                .Include(o => o.FkComunaNavigation).ThenInclude(c => c.FkCodigoRegionNavigation)
                .Include(o => o.FkEstadoNavigation)
                .Include(o => o.FkRutClientesNavigation)
                .Where(o => o.IdOrden == id)
                .Select(o => new
                {
                    o.IdOrden,
                    o.HorasTrabajo,
                    o.FechaRegistro,
                    o.FechaAgendada,
                    o.Observacion,
                    o.Direccion,
                    o.Folio,
                    Comuna = o.FkComunaNavigation,
                    Region = o.FkComunaNavigation.FkCodigoRegionNavigation,
                    o.FkRutClientes,
                    ClienteNombre = o.FkRutClientesNavigation.RazonSocial,
                    EstadoId = o.FkEstadoNavigation.IdEstado,
                    EstadoNombre = o.FkEstadoNavigation.Nombre
                })
                .FirstAsync();

            return new OrdenTrabajoModel
            {
                Id = o.IdOrden,
                HorasTrabajo = o.HorasTrabajo,
                FechaRegistro = o.FechaRegistro,
                FechaAgendada = o.FechaAgendada,
                Observaciones = o.Observacion ?? string.Empty,
                Direccion = o.Direccion,
                Folio = o.Folio.ToString(),
                Comuna = new ComunaModel { Id = o.Comuna.Codigo, Nombre = o.Comuna.Nombre, RegionId = o.Comuna.FkCodigoRegion },
                Region = new RegionModel { Id = o.Region.Codigo, Nombre = o.Region.Nombre },
                IdCliente = int.TryParse(o.FkRutClientes, out var tmpRut) ? tmpRut : 0,
                Cliente = o.ClienteNombre,
                IdEstado = o.EstadoId,
                Estado = o.EstadoNombre ?? string.Empty
            };
        }
    }

    public class EmpleadosDisponiblesRequest
    {
        public DateTime FechaAgendada { get; set; }
        public int HorasTrabajo { get; set; }
    }
}
