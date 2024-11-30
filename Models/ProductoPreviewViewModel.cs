namespace Comprobación.Models
{
    public class ProductoPreviewViewModel
    {
        public int IdProducto { get; set; }
        public string CodigoProducto { get; set; }
        public string NombreProducto { get; set; }
        public decimal PrecioProducto { get; set; }
        public string FotoProducto { get; set; }
        public int StockProducto { get; set; }
        public int CantidadSeleccionada { get; set; } // Para seleccionar la cantidad
        public decimal Total { get; set; } // Total calculado
    }
}
