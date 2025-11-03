using CleanOrderAPI.Models;
using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanOrderAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ControllerBase
    { 
        private readonly ApplicationDbContext _context;
        public RolesController(ApplicationDbContext context) 
        {
            _context = context;
        }
        [HttpGet]
        [Authorize(Roles = "1")]
        public async Task<ActionResult<IEnumerable<RolModel>>> GetRoles()
        {
            List<Rol> roles = await _context.Rols.ToListAsync();
            List<RolModel> rolModels = roles.Select(r => new RolModel
            {
                id = r.IdRol,
                nombre = r.Nombre
            }).ToList();
            return rolModels;
        }


    }
}
