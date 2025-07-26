using Microsoft.AspNetCore.Mvc;
using TiendaAPI.Models;
using TiendaAPI.Repositories;

namespace TiendaAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductosRepository _repoProducto;

        public ProductoController(IProductosRepository repoProducto)
        {
            _repoProducto = repoProducto;
        }

        // Ejemplo de un método que usa el repositorio
        [HttpGet("listarProductos")]
        public async Task<IActionResult> GetProductos()
        {
            try
            {
                var productos = await _repoProducto.GetAllAsync();
                if (productos == null || !productos.Any())
                {
                    return NotFound("No se encontraron productos.");
                }
                return Ok(productos);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al listar los productos.");
            }
        }

        [HttpPost("agregarProducto")]
        public async Task<IActionResult> AddProducto([FromBody] Productos producto)
        {
            if (producto == null)
            {
                return BadRequest("El producto no puede ser nulo.");
            }

            try
            {
                await _repoProducto.AddAsync(producto);
                return CreatedAtAction(nameof(GetProductos), new { id = producto.Id }, producto);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al agregar el producto.");
            }
        }

        [HttpPut("actualizarProducto/{idProd}")]
        public async Task<IActionResult> UpdateProducto(int idProd, [FromBody] Productos producto)
        {
            if (producto == null || idProd <= 0)
            {
                return BadRequest("Datos inválidos para actualizar el producto.");
            }

            try
            {
                await _repoProducto.UpdateAsync(idProd, producto);
                return NoContent(); /* Devuelvo 204 No Content */
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al actualizar el producto.");
            }
        }
        [HttpGet("producto/{id}")]
        public async Task<IActionResult> GetProducto(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            try
            {
                var producto = await _repoProducto.GetByIdAsync(id);
                if (producto == null)
                {
                    return NotFound("Producto no encontrado.");
                }
                return Ok(producto);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al obtener el producto.");
            }
        }

        [HttpDelete("eliminarProducto/{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID inválido.");
            }

            try
            {
                await _repoProducto.DeleteAsync(id);
                return NoContent(); /* Devuelvo 204 No Content */
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Error interno del servidor al eliminar el producto.");
            }
        }
    }
}