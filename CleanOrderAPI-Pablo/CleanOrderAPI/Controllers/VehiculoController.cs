using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.Arm;


namespace CleanOrderAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize (Roles = "1")]
    public class VehiculoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public VehiculoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /vehiculos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<VehiculoModel>>> Listar()
        {
            List<Vehiculo> vehiculos = await _context.Vehiculos
                .AsNoTracking()
                .Include(v => v.FkTipoNavigation)
                .ToListAsync();

            var result = vehiculos.Select(MapToModel).ToList();
            return Ok(result);
        }
        [HttpPost("disponibles")]
        public async Task<ActionResult<IEnumerable<VehiculoModel>>> Disponibles([FromBody] DisponibilidadRequest request)
        {
            DateTime inicio = request.FechaAgendada;
            DateTime fin = inicio.AddHours(request.HorasTrabajo);
            List<string> ocupados = await (from o in _context.Ordens
                                  where o.FechaAgendada < fin
                                     && o.FechaAgendada.AddHours(o.HorasTrabajo) > inicio && 
                                     (o.FkEstado == 1 || o.FkEstado == 2) //Estado agednado o en proceso 
                                  join v in _context.Vehiculos on o.FkPatente equals v.Patente
                                              select v.Patente)
                                 .Distinct()
                                 .ToListAsync();
            List<Vehiculo> vehiculos = await _context.Vehiculos.AsNoTracking()
                .Include(v => v.FkTipoNavigation)
                .Where(v => !ocupados.Contains(v.Patente) && v.Activo.Equals("S"))
                .ToListAsync();
            List<VehiculoModel> result = vehiculos.Select(MapToModel).ToList();
            return Ok(result);
        }

        // GET: /vehiculos/{patente}
        [HttpGet("{patente}")]
        public async Task<ActionResult<VehiculoModel>> Obtener([FromRoute] string patente)
        {
            if (string.IsNullOrWhiteSpace(patente))
                return BadRequest("Patente requerida.");

            var vehiculo = await _context.Vehiculos
                .AsNoTracking()
                .Include(v => v.FkTipoNavigation)
                .FirstOrDefaultAsync(v => v.Patente.ToLower() == patente.ToLower());

            if (vehiculo is null)
                return NotFound();

            return Ok(MapToModel(vehiculo));
        }

        // POST: /api/vehiculos
        [HttpPost]
        public async Task<ActionResult<VehiculoModel>> Crear([FromBody] VehiculoModel request)
        {
            if (request is null || string.IsNullOrWhiteSpace(request.Patente))
                return BadRequest("Datos inválidos. 'patente' es requerido.");

            // Unique by patente
            var exists = await _context.Vehiculos.AnyAsync(v => v.Patente.ToLower() == request.Patente.ToLower());
            if (exists)
                return BadRequest($"Vehiculo con patente '{request.Patente}' ya existe.");

            // Validate TipoCarga
            if (request.TipoCarga is null || request.TipoCarga.Id <= 0)
                return BadRequest("TipoCarga id es requerido.");

            TipoCarga? tipoCarga = await _context.TipoCargas.FirstOrDefaultAsync(tc => tc.TipoCargaCodigo == request.TipoCarga.Id);
            if (tipoCarga is null)
                return BadRequest($"TipoCarga con id '{request.TipoCarga.Id}' no existe.");

            Vehiculo entity = new Vehiculo
            {
                Patente = request.Patente,
                Capacidad = request.Capacidad,
                Activo = request.Activo,
                FkTipo = tipoCarga.TipoCargaCodigo
            };

            _context.Vehiculos.Add(entity);
            await _context.SaveChangesAsync();

            // Reload with navigation for response
            await _context.Entry(entity).Reference(v => v.FkTipoNavigation).LoadAsync();

            var dto = MapToModel(entity);
            return CreatedAtAction(nameof(Obtener), new { patente = dto.Patente }, dto);
        }

        // PUT: /api/vehiculos/{patente}
        // Supports partial updates (VehiculoUpdate = Partial<vehiculo>):
        [HttpPut("{patente}")]
        public async Task<ActionResult<VehiculoModel>> Actualizar([FromRoute] string patente, [FromBody] VehiculoUpdateRequest request)
        {
            if (string.IsNullOrWhiteSpace(patente))
                return BadRequest("Patente requerida.");

            Vehiculo? vehiculo = await _context.Vehiculos
                .Include(v => v.FkTipoNavigation)
                .FirstOrDefaultAsync(v => v.Patente.ToLower() == patente.ToLower());

            if (vehiculo is null)
                return NotFound($"Vehiculo con patente '{patente}' no existe.");

            // Apply partial updates
            /*if (request.Capacidad.HasValue)
                vehiculo.Capacidad = request.Capacidad.Value;*/

            if (request.Activo is not null)
                vehiculo.Activo = request.Activo;

            if (request.TipoCarga is not null && request.TipoCarga.Id.HasValue)
            {
                TipoCarga? tipoCarga = await _context.TipoCargas.FirstOrDefaultAsync(tc => tc.TipoCargaCodigo == request.TipoCarga.Id.Value);
                if (tipoCarga is null)
                    return BadRequest($"TipoCarga con id '{request.TipoCarga.Id.Value}' no existe.");

                vehiculo.FkTipo = tipoCarga.TipoCargaCodigo;
            }

            await _context.SaveChangesAsync();

            // Reload navigation for response
            await _context.Entry(vehiculo).Reference(v => v.FkTipoNavigation).LoadAsync();

            return Ok(MapToModel(vehiculo));
        }

        // DELETE: /api/vehiculos/{patente}
        [HttpDelete("{patente}")]
        public async Task<IActionResult> Eliminar([FromRoute] string patente)
        {
            if (string.IsNullOrWhiteSpace(patente))
                return BadRequest("Patente requerida.");

            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.Patente.ToLower() == patente.ToLower());
            if (vehiculo is null)
                return NotFound();

            _context.Vehiculos.Remove(vehiculo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private static VehiculoModel MapToModel(Vehiculo v)
        {
            return new VehiculoModel
            {
                Patente = v.Patente,
                Capacidad = v.Capacidad,
                Activo = v.Activo,
                TipoCarga = new TipoCargaModel
                    {
                        Id = v.FkTipoNavigation.TipoCargaCodigo,
                        NombreCarga = v.FkTipoNavigation.NombreCarga
                    }
            };
        }
    }
}

    
