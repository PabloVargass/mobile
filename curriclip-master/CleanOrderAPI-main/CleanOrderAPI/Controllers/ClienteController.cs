using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Models;
using CleanOrderAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CleanOrderAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class ClienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailValidationService _emailValidationService;
        public ClienteController(ApplicationDbContext context, IEmailValidationService emailValidationService)
        {
            _context = context;
            _emailValidationService = emailValidationService;
        }

        // GET: /Cliente
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteModel>>> GetClientes()
        {
            List<ClienteModel> clientes = await _context.Clientes.AsNoTracking()
                .Select(c => new ClienteModel
                {
                    Rut = c.RutCliente,
                    Dv = c.Dv,
                    RazonSocial = c.RazonSocial,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    Activo = c.Activo
                })
                .ToListAsync();

            return Ok(clientes);
        }

        // GET: /Cliente/activos - solo clientes activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<ClienteModel>>> GetClientesActivos()
        {
            var activos = await _context.Clientes.AsNoTracking()
                .Where(c => c.Activo == "S")
                .Select(c => new ClienteModel
                {
                    Rut = c.RutCliente,
                    Dv = c.Dv,
                    RazonSocial = c.RazonSocial,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    Activo = c.Activo
                })
                .ToListAsync();
            return Ok(activos);
        }

        // GET: /Cliente/{rut}
        [HttpGet("{rut}")]
        public async Task<ActionResult<ClienteModel>> GetCliente(string rut)
        {
            Cliente? cliente = await _context.Clientes.FindAsync(rut);

            if (cliente == null)
            {
                return NotFound($"Cliente with RUT {rut} not found.");
            }

            ClienteModel clienteModel = new ClienteModel
            {
                Rut = cliente.RutCliente,
                Dv = cliente.Dv,
                RazonSocial = cliente.RazonSocial,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono,
                Activo = cliente.Activo
            };

            return Ok(clienteModel);
        }

        // POST: /Cliente
        [HttpPost]
        public async Task<ActionResult<ClienteModel>> CreateCliente(ClienteModel clienteModel)
        {
            // Validate custom business rules
            List<string> validationErrors = ValidateClienteModel(clienteModel);
            if (validationErrors.Any())
            {
                return BadRequest("Errores de validacion en campos");
                //Mejorar mensajes de error a futuro
                //BadRequest(                                   
                //new { 
                //Message = "Errores de validación encontrados.", 
                //Errors = validationErrors 
                //});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if cliente already exists
            Cliente? existingCliente = await _context.Clientes.FindAsync(clienteModel.Rut);
            if (existingCliente != null)
            {
                return Conflict($"Cliente con RUT {clienteModel.Rut} ya existe.");
            }

            Cliente cliente = new Cliente
            {
                RutCliente = clienteModel.Rut,
                Dv = clienteModel.Dv.ToUpper(),
                RazonSocial = clienteModel.RazonSocial,
                Correo = clienteModel.Correo,
                Telefono = clienteModel.Telefono,
                Activo = clienteModel.Activo.ToUpper()
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCliente), new { rut = cliente.RutCliente }, clienteModel);
        }

        // PUT: /Cliente/{rut}
        [HttpPut("{rut}")]
        public async Task<IActionResult> UpdateCliente(string rut, ClienteModel clienteModel)
        {
            if (!rut.Equals(clienteModel.Rut))
            {
                return BadRequest("RUT en URL no coincide con el del payload.");
            }

            // Validate custom business rules
            List<string> validationErrors = ValidateClienteModel(clienteModel);
            if (validationErrors.Any())
            {
                return BadRequest("Errores de validacion en campos");
                //Mejorar mensajes de error a futuro
                //BadRequest(                                   
                //new { 
                //Message = "Errores de validación encontrados.", 
                //Errors = validationErrors 
                //});
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Cliente? cliente = await _context.Clientes.FindAsync(rut);
            if (cliente == null)
            {
                return NotFound($"Cliente con RUT {rut} no existente.");
            }

            // Update properties
            cliente.Dv = clienteModel.Dv;
            cliente.RazonSocial = clienteModel.RazonSocial;
            cliente.Correo = clienteModel.Correo;
            cliente.Telefono = clienteModel.Telefono;
            cliente.Activo = clienteModel.Activo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(rut))
                {
                    return NotFound($"Cliente with RUT {rut} not found.");
                }
                throw;
            }

            return NoContent();
        }

        // DELETE: /Cliente/{rut}
        [HttpDelete("{rut}")]
        public async Task<IActionResult> DeleteCliente(string rut)
        {
            Cliente? cliente = await _context.Clientes.FindAsync(rut);
            if (cliente == null)
            {
                return NotFound($"Cliente with RUT {rut} not found.");
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ClienteExists(string rut)
        {
            return _context.Clientes.Any(e => e.RutCliente == rut);
        }

        private List<string> ValidateClienteModel(ClienteModel clienteModel)
        {
            var errors = new List<string>();

            // Validate RUT
            if (string.IsNullOrWhiteSpace(clienteModel.Rut))
            {
                errors.Add("RUT es requerido.");
            }
            else if (clienteModel.Rut.Length < 7 || clienteModel.Rut.Length > 8)
            {
                errors.Add("RUT debe tener entre 7 y 8 dígitos.");
            }

            // Validate DV (Dígito Verificador)
            if (string.IsNullOrWhiteSpace(clienteModel.Dv))
            {
                errors.Add("Dígito verificador (DV) es requerido.");
            }
            else if (clienteModel.Dv.Length != 1)
            {
                errors.Add("Dígito verificador (DV) debe ser un solo carácter.");
            }
            else
            {
                // Check if DV is a digit (0-9) or the letter 'K' (case insensitive)
                char dv = clienteModel.Dv.ToUpperInvariant()[0];
                if (!char.IsDigit(dv) && dv != 'K')
                {
                    errors.Add("Dígito verificador (DV) debe ser un número (0-9) o la letra 'K'.");
                }
            }
            // Validate Razón Social
            if (string.IsNullOrWhiteSpace(clienteModel.RazonSocial))
            {
                errors.Add("Razón Social es requerida.");
            }
            else if(clienteModel.RazonSocial.Length < 3)
            {
                errors.Add("Razón Social debe ser mayor a 2 caracteres.");
            }
            else if (clienteModel.RazonSocial.Length > 100)
            {
                errors.Add("Razón Social no puede exceder los 100 caracteres.");
            }

            // Validate Email
            if (string.IsNullOrWhiteSpace(clienteModel.Correo))
            {
                errors.Add("Correo electrónico es requerido.");
            }
            else if (!_emailValidationService.IsValidEmail(clienteModel.Correo))
            {
                errors.Add("Formato de correo electrónico inválido.");
            }

            // Validate Telefono (optional field)
            if (!string.IsNullOrWhiteSpace(clienteModel.Telefono))
            {
                if (clienteModel.Telefono.Length < 8 || clienteModel.Telefono.Length > 15)
                {
                    errors.Add("Teléfono debe tener entre 8 y 15 dígitos.");
                }
                if (!clienteModel.Telefono.All(char.IsDigit))
                {
                    errors.Add("Teléfono debe contener solo números.");
                }
            }

            // Validate Activo
            if (string.IsNullOrWhiteSpace(clienteModel.Activo))
            {
                errors.Add("Estado activo es requerido.");
            }
            else if (clienteModel.Activo.ToUpper() != "S" && clienteModel.Activo.ToUpper() != "N")
            {
                errors.Add("Estado activo debe ser 'S' (Sí) o 'N' (No).");
            }

            return errors;
        }

    }
}
