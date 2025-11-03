using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CleanOrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class RegionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RegionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Region
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegionModel>>> GetAll()
        {
            List<RegionModel> regions = await _context.Regions
                                        .AsNoTracking()
                                        .Select(r => new RegionModel
                                        {
                                            Id = r.Codigo,
                                            Nombre = r.Nombre
                                        })
                                        .ToListAsync();

            return Ok(regions);
        }

        // GET: /Region/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegionModel>> GetById(int id)
        {
            var region = await _context.Regions
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Codigo == id);

            if (region == null)
                return NotFound();

            return Ok(new RegionModel
            {
                Id = region.Codigo,
                Nombre = region.Nombre
            });
        }

        [HttpGet("GetByComunaId/{id:int}")]
        public async Task<ActionResult<RegionModel>> GetByComunaId(int id)
        {
            Comuna? comuna= await _context.Comunas
                                .AsNoTracking()
                                .FirstOrDefaultAsync(c => c.Codigo == id);
            if (comuna == null)
                return NotFound("Comuna no encontrada.");

            Region? region = await _context.Regions
                            .AsNoTracking()
                            .FirstOrDefaultAsync(r => r.Codigo == comuna.FkCodigoRegion);

            if (region == null)
                return NotFound("Region no encontrada");

            return Ok(new RegionModel
            {
                Id = region.Codigo,
                Nombre = region.Nombre
            });
        }

    }
}
