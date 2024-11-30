using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Comprobación.Models
{
    public class DetallesVentas
    {
        /* Id */
        [Key]
        [Column("detalle_venta_id")]
        public int DetalleVentaId { get; set; }

        /* Foreign Key: venta_id */
        [Column("venta_id")]
        public int VentaId { get; set; }

        /* Foreign Key: producto_id */
        [Column("producto_id")]
        public int ProductoId { get; set; }

        /* Cantidad */
        [Column("cantidad")]
        [Range(1, 1000, ErrorMessage = "Debe ser un entero entre 1 y 1000.")]
        [Required(ErrorMessage = "La cantidad no puede estar vacía.")]
        public int Cantidad { get; set; }

        /* Precio Unitario */
        [Column("precio_unitario")]
        [Range(0.1, 1000.1, ErrorMessage = "Debe ser un valor entre 0.1 y 1000.1.")]
        [Required(ErrorMessage = "El precio unitario no puede estar vacío.")]
        public decimal PrecioUnitario { get; set; }

        /* Subtotal */
        [Column("subtotal")]
        [Range(0.01, 10000, ErrorMessage = "Debe ser un valor entre 0.01 y 10000.")]
        [Required(ErrorMessage = "El subtotal no puede estar vacío.")]
        public decimal Subtotal { get; set; }

        /* Navigation Properties */
        [ForeignKey("VentaId")]
        public Venta? Venta { get; set; }

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}


