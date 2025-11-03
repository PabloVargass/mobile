using CleanOrderAPI.Data;
using CleanOrderAPI.Data.Entities;
using CleanOrderAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace CleanOrderAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
    public class DocumentoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private const int MaxFileBytes = 10 * 1024 * 1024; // 10 MB

        // Allowed MIME types para "PDF o Word"
        private static readonly HashSet<string> AllowedMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };

        public DocumentoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Documento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoSinArchivoModel>>> GetDocumentos()
        {
            var docs = await _context.Documentos
                .AsNoTracking()
                .Select(d => new DocumentoSinArchivoModel
                {
                    idDocumento = d.IdDocumento,
                    nombre = d.Nombre,
                    fechaSubida = d.FechaSubida,
                    tipoMime = d.TipoMime,
                    tamanoBytes = d.TamanoBytes,
                    RutCliente = d.FkRutCliente
                })
                .ToListAsync();

            return Ok(docs);
        }

        // GET: /Documento/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentoModel>> GetDocumento(int id)
        {
            var doc = await _context.Documentos.FindAsync(id);
            if (doc == null)
            {
                return NotFound($"Documento with id {id} not found.");
            }

            var model = new DocumentoModel
            {
                idDocumento = doc.IdDocumento,
                nombre = doc.Nombre,
                fechaSubida = doc.FechaSubida,
                tipoMime = doc.TipoMime,
                archivo = Convert.ToBase64String(doc.Archivo),
                tamanoBytes = doc.TamanoBytes,
                RutCliente = doc.FkRutCliente
            };

            return Ok(model);
        }

        // GET: /Documento/cliente/{rut}
        [HttpGet("cliente/{rut}")]
        public async Task<ActionResult<IEnumerable<DocumentoSinArchivoModel>>> GetDocumentosByCliente(string rut)
        {
            var exists = await _context.Clientes.AnyAsync(c => c.RutCliente == rut);
            if (!exists)
            {
                return NotFound($"Cliente with RUT {rut} not found.");
            }

            var docs = await _context.Documentos
                .Where(d => d.FkRutCliente == rut)
                .AsNoTracking()
                .Select(d => new DocumentoSinArchivoModel
                {
                    idDocumento = d.IdDocumento,
                    nombre = d.Nombre,
                    fechaSubida = d.FechaSubida,
                    tipoMime = d.TipoMime,
                    tamanoBytes = d.TamanoBytes,
                    RutCliente = d.FkRutCliente
                })
                .ToListAsync();

            return Ok(docs);
        }

        // POST: /Documento
        [HttpPost]
        public async Task<ActionResult<DocumentoSinArchivoModel>> CreateDocumento(DocumentoModel model)
        {
            (bool IsValid, List <string> errors)  = await ValidateDocumentoAsync(model);
            if (!IsValid)
                return BadRequest(errors);

            (byte[]? validationResult, string? errorFile) = ValidateAndDecodeArchivo(model.archivo);
            if (!errorFile.IsNullOrEmpty())
                return BadRequest(errorFile);

            Documento entity = new Documento
            {
                Nombre = model.nombre,
                FechaSubida = model.fechaSubida == default ? DateTime.UtcNow : model.fechaSubida,
                TipoMime = model.tipoMime,
                Archivo = validationResult!,
                TamanoBytes = validationResult!.Length,
                FkRutCliente = model.RutCliente
            };

            _context.Documentos.Add(entity);
            await _context.SaveChangesAsync();
            DocumentoSinArchivoModel respuesta = new DocumentoSinArchivoModel
            {
                idDocumento = entity.IdDocumento,
                nombre = entity.Nombre,
                fechaSubida = entity.FechaSubida,
                tipoMime = entity.TipoMime,
                tamanoBytes = entity.TamanoBytes,
                RutCliente = entity.FkRutCliente
            };

            return CreatedAtAction(nameof(GetDocumento), new { id = entity.IdDocumento }, respuesta);
        }

        // PUT: /Documento/{id}
        //[HttpPut("{id:int}")]
        //public async Task<ActionResult<DocumentoModel>> UpdateDocumento(int id, DocumentoModel model)
        //{
        //    var entity = await _context.Documentos.FindAsync(id);
        //    if (entity == null)
        //    {
        //        return NotFound($"Documento with id {id} not found.");
        //    }

        //    if (entity.IdDocumento != model.idDocumento && model.idDocumento != 0)
        //    {
        //        // Keep route authority
        //        model.idDocumento = entity.IdDocumento;
        //    }

        //    if (!string.IsNullOrWhiteSpace(model.fkRutCliente) &&
        //        entity.FkRutCliente != model.fkRutCliente)
        //    {
        //        // Validate new client exists
        //        if (!await _context.Clientes.AnyAsync(c => c.RutCliente == model.fkRutCliente))
        //            return BadRequest($"Cliente with RUT {model.fkRutCliente} does not exist.");

        //        entity.FkRutCliente = model.fkRutCliente;
        //    }

        //    entity.Nombre = model.nombre;
        //    entity.TipoMime = model.tipoMime;

        //    // Update file if provided
        //    if (!string.IsNullOrWhiteSpace(model.archivo))
        //    {
        //        try
        //        {
        //            var newBytes = Convert.FromBase64String(model.archivo);
        //            entity.Archivo = newBytes;
        //            entity.TamanoBytes = newBytes.Length;
        //        }
        //        catch
        //        {
        //            return BadRequest("Invalid Base64 content for archivo.");
        //        }
        //    }

        //    // Preserve original upload date unless explicitly changed
        //    if (model.fechaSubida != default && model.fechaSubida != entity.FechaSubida)
        //    {
        //        entity.FechaSubida = model.fechaSubida;
        //    }

        //    await _context.SaveChangesAsync();

        //    // Return updated model
        //    var updated = new DocumentoModel
        //    {
        //        idDocumento = entity.IdDocumento,
        //        nombre = entity.Nombre,
        //        fechaSubida = entity.FechaSubida,
        //        tipoMime = entity.TipoMime,
        //        archivo = Convert.ToBase64String(entity.Archivo),
        //        tamanoBytes = entity.TamanoBytes,
        //        fkRutCliente = entity.FkRutCliente
        //    };

        //    return Ok(updated);
        //}

        // DELETE: /Documento/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDocumento(int id)
        {
            var entity = await _context.Documentos.FindAsync(id);
            if (entity == null)
            {
                return NotFound($"Documento with id {id} not found.");
            }

            _context.Documentos.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Validación general (excepto la parte de archivo que se separó)
        private async Task<(bool IsValid, List<string> errors)> ValidateDocumentoAsync(DocumentoModel model)
        {
            List<string> errors = new List<string>(0);

            // nombre
            if (string.IsNullOrWhiteSpace(model.nombre))
                errors.Add("nombre is required.");

            // fechaSubida (ISO 8601 ya parseado por el modelo => validar no default)
            if (model.fechaSubida == default)
                errors.Add("fechaSubida must be a valid ISO 8601 date/time.");

            // tipoMime
            if (string.IsNullOrWhiteSpace(model.tipoMime))
                errors.Add("tipoMime is required.");
            else if (!AllowedMimeTypes.Contains(model.tipoMime))
                errors.Add("tipoMime must be one of: application/pdf, application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document.");

            // RutCliente
            if (string.IsNullOrWhiteSpace(model.RutCliente))
            {
                errors.Add("RutCliente is required.");
            }
            else
            {
                var exists = await _context.Clientes.AnyAsync(c => c.RutCliente == model.RutCliente);
                if (!exists)
                    errors.Add($"Cliente with RUT {model.RutCliente} does not exist.");
            }

            return (errors.IsNullOrEmpty(), errors);
        }

        // Método especializado SOLO para validar y convertir el archivo Base64 -> byte[]
        // Reutilizable para Create / Update.
        private (byte[] FileBytes, string Error) ValidateAndDecodeArchivo(string? base64Archivo)
        {
            byte[] FileBytes = Array.Empty<byte>();
            string Error = string.Empty;
            if (string.IsNullOrWhiteSpace(base64Archivo))
                Error = "Se requiere archivo (Base64).";
            else
            {
                try
                {
                    // Podríamos usar Convert.TryFromBase64String con un buffer prealocado si quisiéramos micro-optimizar.
                    FileBytes = Convert.FromBase64String(base64Archivo!);
                    if (FileBytes.Length > MaxFileBytes)
                    {
                        Error = $"archivo exede el lomite de 10 MB. Tamaño: {FileBytes.Length} bytes.";
                        FileBytes = Array.Empty<byte>();
                    }
                }
                catch
                {
                    // Intentar decodificar sin lanzar excepción costosa en caso de formato inválido
                    Error = "Archivo tiene contenido Base64 invalido.";
                    return (Array.Empty<byte>(), Error);
                }
            }
            return (FileBytes, Error);


        }
    }
}
