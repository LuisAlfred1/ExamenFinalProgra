using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comprobación.Models
{
    public class Producto
    {
        /*Id*/
        [Key]
        [Column("producto_id")]
        public int IdProdcuto { get; set; }
        /*Codigo Producto*/
        [Column("codigo")]
        [StringLength(50, ErrorMessage = "No mayor a 50 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string CodigoProducto { get; set; }
        /*Nombre*/
        [Column("nombre")]
        [StringLength(100, ErrorMessage = "No mayor a 100 caracteres")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public string NombreProducto { get; set; }
        /*Descripción*/
        [Column("descripcion")]
        public string DescripcionProducto { get; set; }
        /*Precio*/
        [Column("precio")]
        [Range(0.1, 10000.01, ErrorMessage = "Desde 0.1 hasta 10000.01")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public decimal PrecioProducto { get; set; }
        /*Stock*/
        [Column("stock")]
        [Range(1, 10000, ErrorMessage = "Desde 1 hasta 10000")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public int StockProducto { get; set; }
        /*foto*/
        [Column("foto_url")]
        public string? FotoProducto { get; set; }

        /*Variables de relaciones de producto hacia los otros*/
        public ICollection<DetallesVentas>? DetallesVentas { get; set; }
        /*Variables de relaciones de otros hacia producto*/

    }
}
