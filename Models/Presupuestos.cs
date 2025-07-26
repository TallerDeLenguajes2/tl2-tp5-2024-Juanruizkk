using System.Collections.Generic;

namespace TiendaAPI.Models
{
    public class Presupuestos
    {
        public int IdPresupuesto { get; set; }
        public string NombreDestinatario { get; set; } = "";
        public DateOnly FechaCreacion { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        public List<PresupuestoDetalle> Detalles { get; set; } = new List<PresupuestoDetalle>();

        public int MontoPresupuesto()
        {
            return (int)Detalles.Sum(detalle => detalle.Producto.Precio * detalle.Cantidad);
        }

        public decimal MontoPresupuestoConIva()
        {
            return MontoPresupuesto() * 1.21m;
        }

        public int CantidadProductos()
        {
            return Detalles.Sum(d => d.Cantidad);
        }
    }
}