using Comprobación.Data;
using Comprobación.DTOs;
using Comprobación.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Comprobación.Controllers
{
    public class PosController : Controller
    {
        private readonly AppDbContext _context;

        public PosController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> POS(string? filtroNombre, string? filtroCodigo)
        {
            var productosQuery = _context.Productos.AsQueryable();

            // Aplicar filtros si existen
            if (!string.IsNullOrEmpty(filtroNombre))
            {
                productosQuery = productosQuery.Where(p => p.NombreProducto.Contains(filtroNombre));
            }

            if (!string.IsNullOrEmpty(filtroCodigo))
            {
                productosQuery = productosQuery.Where(p => p.CodigoProducto.Contains(filtroCodigo));
            }

            var productos = await productosQuery.ToListAsync();
            var clientes = await _context.Clientes.ToListAsync(); // Para el dropdown de clientes
            var empleados = await _context.Empleados.ToListAsync();
            ViewBag.Clientes = clientes;
            ViewBag.Empleados = empleados;

            // Pasar los valores de los filtros actuales a la vista
            ViewBag.FiltroNombre = filtroNombre;
            ViewBag.FiltroCodigo = filtroCodigo;

            return View(productos);
        }
        /*Nuevo*/
        [HttpPost]

        public async Task<IActionResult> CrearVenta([FromBody] VentaDTO ventaDto)
        {
            if (ventaDto == null || ventaDto.Detalles == null || !ventaDto.Detalles.Any())
            {
                return BadRequest("Datos no válidos de la venta");
            }

            // Validar si el cliente existe
            var cliente = await _context.Clientes.FindAsync(ventaDto.IdClienteEnVenta);
            if (cliente == null)
            {
                return BadRequest("El cliente seleccionado no existe");
            }

            // Validar si el empleado existe
            var empleado = await _context.Empleados.FindAsync(ventaDto.IdEmpleadoEnVenta);
            if (empleado == null)
            {
                return BadRequest("El empleado seleccionado no existe.");
            }

            // Crear nueva venta
            Venta nuevaVenta = new Venta
            {
                FechaVenta = DateTime.Now,
                IdClienteEnVenta = ventaDto.IdClienteEnVenta,
                IdEmpleadoEnVenta = ventaDto.IdEmpleadoEnVenta, // Asociar al empleado
                TotalVenta = ventaDto.Detalles.Sum(d => d.Cantidad * d.PrecioUnitario)
            };

            _context.Ventas.Add(nuevaVenta);
            await _context.SaveChangesAsync(); // Guardar para obtener el Id de la nueva venta

            foreach (var detalleDto in ventaDto.Detalles)
            {
                // Validar si el producto existe
                var producto = await _context.Productos.FindAsync(detalleDto.ProductoId);
                if (producto == null)
                {
                    return BadRequest($"El producto con ID {detalleDto.ProductoId} no existe.");
                }

                // Validar si hay suficiente stock
                if (producto.StockProducto < detalleDto.Cantidad)
                {
                    return BadRequest($"No hay suficiente stock para el producto {producto.NombreProducto}.");
                }

                // Crear el detalle de la venta
                var detalle = new DetallesVentas
                {
                    VentaId = nuevaVenta.IdVenta,
                    ProductoId = detalleDto.ProductoId,
                    Cantidad = detalleDto.Cantidad,
                    PrecioUnitario = detalleDto.PrecioUnitario,
                    Subtotal = detalleDto.Cantidad * detalleDto.PrecioUnitario
                };
                _context.Detalle_Ventas.Add(detalle);

                // Actualizar stock del producto
                producto.StockProducto -= detalleDto.Cantidad;
            }

            await _context.SaveChangesAsync(); // Guardar los detalles y los cambios en los productos

            return Ok(new { VentaId = nuevaVenta.IdVenta });
        }

        /*Factura*/
        [HttpGet]
        public IActionResult ImprimirFacturaPorVenta(long idVenta)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            // Obtener la información de la venta, incluyendo cliente, productos y detalles
            var venta = _context.Ventas
                .Include(v => v.Cliente)
                .Include(v=> v.Empleado)
                .Include(v => v.DetallesVentas)
                    .ThenInclude(dv => dv.Producto)
                .FirstOrDefault(v => v.IdVenta == idVenta);

            if (venta == null)
            {
                return NotFound("No se encontró la venta especificada.");
            }

            if (venta.DetallesVentas == null || !venta.DetallesVentas.Any())
            {
                return BadRequest("La venta no tiene productos asociados.");
            }


            // Ruta del logo
            var logo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/icons/apple-touch-icon-57x57.png");
            if (!System.IO.File.Exists(logo))
            {
                return BadRequest("El logo de la empresa no se encuentra en la ruta especificada.");
            }

            var culturaMoneda = new System.Globalization.CultureInfo("es-GT");

            // Crear el documento de la factura
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);

                    // Encabezado de la factura
                    page.Header().Row(header =>
                    {
                        header.RelativeItem().Text("Factura de Venta").FontSize(24).Bold().AlignCenter();
                        header.ConstantItem(80).Image(logo, ImageScaling.FitArea);
                    });

                    page.Content().Column(column =>
                    {
                        // Información básica de la factura
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Fecha: {venta.FechaVenta.ToString("dd/MM/yyyy")}");
                            row.RelativeItem().Text($"No. Factura: {venta.IdVenta}");
                        });

                        // Información del cliente
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Cliente: {venta.Cliente?.NombreCliente ?? "Cliente no especificado"}");
                            row.RelativeItem().Text($"NIT: {venta.Cliente?.NitCliente ?? "CF"}");

                        });

                        column.Item().Row(row =>
                        {
                        row.RelativeItem().Text($"Empleado: {venta.Empleado?.NombreEmpleado}");
                        });

                    column.Item().PaddingVertical(1, Unit.Centimetre);

                        // Tabla de productos vendidos
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Producto
                                columns.ConstantColumn(60); // Cantidad
                                columns.ConstantColumn(100); // Precio Unitario
                                columns.ConstantColumn(100); // Subtotal
                            });

                            // Encabezado de la tabla
                            table.Header(header =>
                            {
                                header.Cell().Element(EstiloCelda).Text("Producto").FontSize(12).Bold();
                                header.Cell().Element(EstiloCelda).Text("Cantidad").FontSize(12).Bold();
                                header.Cell().Element(EstiloCelda).Text("Precio Unitario").FontSize(12).Bold();
                                header.Cell().Element(EstiloCelda).Text("Subtotal").FontSize(12).Bold();

                                static IContainer EstiloCelda(IContainer container)
                                {
                                    return container.Background("#e0e0e0").Border(1).BorderColor("#e0e0e0").Padding(5).AlignCenter();
                                }
                            });

                            // Contenido de la tabla
                            foreach (var detalle in venta.DetallesVentas)
                            {
                                table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).Text(detalle.Producto?.NombreProducto ?? "Producto no especificado");
                                table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignCenter().Text(detalle.Cantidad.ToString());
                                table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight().Text(detalle.PrecioUnitario.ToString("C", culturaMoneda));
                                table.Cell().Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight().Text((detalle.Cantidad * detalle.PrecioUnitario).ToString("C", culturaMoneda));
                            }


                            // Total de la venta
                            table.Cell().ColumnSpan(3).Background("#f0f0f0").Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight()
                                .Text("TOTAL").FontSize(12).Bold();
                            table.Cell().Background("#f0f0f0").Border(1).BorderColor("#c0c0c0").Padding(5).AlignRight()
                                .Text(venta.TotalVenta.ToString("C", culturaMoneda));
                        });
                    });

                    // Pie de página
                    page.Footer().Text($"Sistema POS - 2024 - Facturación Electrónica").FontSize(10).AlignCenter();
                });
            });

            // Generar el PDF en un MemoryStream
            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;

            // Opcional: verificar si el archivo tiene contenido
            if (stream.Length == 0)
            {
                return StatusCode(500, "La factura no pudo generarse. Verifique los datos de la venta.");
            }

            return File(stream, "application/pdf", $"Factura_{idVenta}.pdf");

        }

    }
}