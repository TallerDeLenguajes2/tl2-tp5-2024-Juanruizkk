/* Se anade este namespace ya que las clases como ControllerBAase, IActionResult estan dentro de ellas */
using Microsoft.AspNetCore.Mvc;
using TiendaAPI.Models;
using TiendaAPI.Repositories;

namespace TiendaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestoController : ControllerBase
    {
        private readonly IPresupuestoRepository _repoPresupuesto;

        public PresupuestoController(IPresupuestoRepository repoPresupuesto)
        {
            _repoPresupuesto = repoPresupuesto;
        }

        // Ejemplo de un método que usa el repositorio
        [HttpGet("listarPresupuestos")]
        public async Task<IActionResult> GetPresupuestos()
        {
            try
            {
                var presupuestos = await _repoPresupuesto.GetAllAsync();
                if (presupuestos == null || !presupuestos.Any())
                {
                    return NotFound("No se encontraron presupuestos.");
                }
                return Ok(presupuestos);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al listar los presupuestos.");
            }

        }
        [HttpPost("agregarPresupuesto")]
        public async Task<IActionResult> AddPresupuesto([FromBody] Presupuestos presupuesto)
        {
            try
            {
                var presupuestosExistentes = await _repoPresupuesto.GetAllAsync();
                if (presupuestosExistentes.Any(p => p.NombreDestinatario == presupuesto.NombreDestinatario))
                {
                    return BadRequest("Ya existe un presupuesto con el mismo nombre de destinatario.");
                }
                await _repoPresupuesto.AddAsync(presupuesto);
                return CreatedAtAction(nameof(GetPresupuestos), new { id = presupuesto.IdPresupuesto }, presupuesto);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al agregar el presupuesto.");
            }
        }
        [HttpPost("{idPresupuesto}/agregarProductoDetalle")]
        public async Task<IActionResult> AddProductoDetalle([FromRoute] int idPresupuesto, [FromQuery] int idProducto, [FromQuery] int cantidad)
        {
            if (idPresupuesto <= 0 || idProducto <= 0 || cantidad <= 0)
            {
                return BadRequest("Los parámetros del detalle del producto no pueden ser nulos o negativos.");
            }

            try
            {
                var resultado = await _repoPresupuesto.AddProductoDetalleAsync(idPresupuesto, idProducto, cantidad);
                if (!resultado)
                {
                    return BadRequest("No se pudo agregar el detalle del producto.");
                }
                return Ok("Detalle del producto agregado exitosamente.");
            }
            catch (Exception ex)  // ✅ Cambiar para ver el error específico
            {
                return StatusCode(500, $"Error específico: {ex.Message}");  // ✅ Ver error real
            }
        }
    }
}