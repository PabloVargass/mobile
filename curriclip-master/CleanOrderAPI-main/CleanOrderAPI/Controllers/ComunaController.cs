using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanOrderAPI.Data;
using CleanOrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanOrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ComunaController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ComunaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Comuna
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ComunaModel>>> GetAll()
        {
            List<ComunaModel> comunas = await _context.Comunas
                .AsNoTracking()
                .Select(c => new ComunaModel
                {
                    Id = c.Codigo,
                    Nombre = c.Nombre,
                    RegionId = c.FkCodigoRegion
                })
                .ToListAsync();

            return Ok(comunas);
        }

        // GET: /Comuna/ByRegion/{id}
        [HttpGet("ByRegion/{id:int}")]
        public async Task<ActionResult<IEnumerable<ComunaModel>>> GetComunasRegion(int id)
        {
            List<ComunaModel> comunas = await _context.Comunas
                .AsNoTracking()
                .Where(c => c.FkCodigoRegion == id)
                .Select(c => new ComunaModel
                {
                    Id = c.Codigo,
                    Nombre = c.Nombre,
                    RegionId = c.FkCodigoRegion
                })
                .ToListAsync();

            return Ok(comunas);
        }

        // GET: /Comuna/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ComunaModel>> GetById(int id)
        {
            var comuna = await _context.Comunas
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Codigo == id);

            if (comuna == null)
                return NotFound();

            var model = new ComunaModel
            {
                Id = comuna.Codigo,
                Nombre = comuna.Nombre,
                RegionId = comuna.FkCodigoRegion
            };

            return Ok(model);
        }
    }
}
