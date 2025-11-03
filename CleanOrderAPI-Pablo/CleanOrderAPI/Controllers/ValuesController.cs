using CleanOrderAPI.Data;
using CleanOrderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CleanOrderAPI.Data.Entities;

namespace CleanOrderAPI.Controllers
{
    [Route("tipos-carga")]
    [ApiController]
    [Authorize (Roles = "1")]
    public class TipoCargaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TipoCargaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /tipos-carga
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoCargaModel>>> Listar()
        {
            List<TipoCargaModel> tipos = await _context.TipoCargas
                .AsNoTracking()
                .Select(tc => new TipoCargaModel
                {
                    Id = tc.TipoCargaCodigo,
                    NombreCarga = tc.NombreCarga
                })
                .ToListAsync();

            return Ok(tipos);
        }

        // GET: /tipos-carga/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TipoCargaModel>> Obtener([FromRoute] int id)
        {
            TipoCarga? tipo = await _context.TipoCargas
                .AsNoTracking()
                .FirstOrDefaultAsync(tc => tc.TipoCargaCodigo == id);

            if (tipo is null)
                return NotFound();

            return Ok(new TipoCargaModel
            {
                Id = tipo.TipoCargaCodigo,
                NombreCarga = tipo.NombreCarga
            });
        }
    }
}
