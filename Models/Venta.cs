using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Comprobación.Models
{
    public class Venta
    {
        /*Id venta*/
        [Key]
        [Column("venta_id")]
        public int IdVenta { get; set; }
        /*Fecha de la venta*/
        [Column("fecha")]
        [Required(ErrorMessage = "No puede estar vacio")]
        public DateTime FechaVenta { get; set; }
        /*Relaciones*/
        //cliente_id
        [ForeignKey("Cliente")]
        [Column("cliente_id")]
        public int IdClienteEnVenta { get; set; }
        //empleado_id
        [ForeignKey("Empleado")]
        [Column("empleado_id")]
        public int IdEmpleadoEnVenta { get; set; }
        /*Total Venta*/
        [Column("total")]
        [Range(0.1, 10000.01, ErrorMessage = "Desde 0.1 hasta 10000.01")]
        [Required(ErrorMessage = "No puede ser vacio")]
        public decimal TotalVenta { get; set; }


        /*Variables de relaciones de venta hacia otros*/
        public ICollection<DetallesVentas>? DetallesVentas { get; set; }

        /*Variables de relaciones de otros hacia venta*/
        public Cliente? Cliente { get; set; }
        public Empleado? Empleado { get; set; }

    }
}
