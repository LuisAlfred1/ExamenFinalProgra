using System.ComponentModel.DataAnnotations.Schema;
namespace Comprobación.DTOs
{
    public class DetalleVentasDTO
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
