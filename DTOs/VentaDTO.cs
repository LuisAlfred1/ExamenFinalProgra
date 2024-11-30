namespace Comprobación.DTOs
{
    public class VentaDTO
    {
        public int IdClienteEnVenta { get; set; }
        public int IdEmpleadoEnVenta { get; set; }
        public List<DetalleVentasDTO> Detalles { get; set; }
    }
}
