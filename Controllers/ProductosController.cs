using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Comprobación.Data;
using Comprobación.Models;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using QuestPDF.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace Comprobación.Controllers
{
    public class ProductosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ProductosController(AppDbContext context,Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }

        // GET: Productos

        public async Task<IActionResult> Index(string? filtroNombre, string? filtroCodigo)
        {
            // Consulta base para obtener todos los productos
            var productosQuery = _context.Productos.AsQueryable();

            // Verificar si hay productos en la base de datos
            if (!await productosQuery.AnyAsync())
            {
                ViewData["Mensaje"] = "No hay productos registrados en la base de datos.";
                return View(new List<Producto>());
            }

            // Filtrar por nombre de producto
            if (!string.IsNullOrEmpty(filtroNombre))
            {
                productosQuery = productosQuery.Where(p => p.NombreProducto.Contains(filtroNombre));
                if (!await productosQuery.AnyAsync())
                {
                    ViewData["Mensaje"] = $"No se encontraron productos con el nombre: {filtroNombre}";
                }
            }

            // Filtrar por código de producto
            if (!string.IsNullOrEmpty(filtroCodigo))
            {
                productosQuery = productosQuery.Where(p => p.CodigoProducto.Contains(filtroCodigo));
                if (!await productosQuery.AnyAsync())
                {
                    ViewData["Mensaje"] = $"No se encontraron productos con el código: {filtroCodigo}";
                }
            }

            ViewData["filtroNombre"] = filtroNombre;
            ViewData["filtroCodigo"] = filtroCodigo;

            // Devolver la lista filtrada (o completa si no hay filtros)
            return View(await productosQuery.ToListAsync());
        }

        // GET: Productos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProdcuto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdProdcuto,CodigoProducto,NombreProducto,DescripcionProducto,PrecioProducto,StockProducto")] Producto producto, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                    };
                    var uploadResults = await _cloudinary.UploadAsync(uploadParams);
                    producto.FotoProducto = uploadResults.SecureUrl.ToString();
                }

                _context.Add(producto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            return View(producto);
        }

        // POST: Productos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdProdcuto,CodigoProducto,NombreProducto,DescripcionProducto,PrecioProducto,StockProducto")] Producto producto, IFormFile? imageFile)
        {
            if (id != producto.IdProdcuto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
                            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
                        };
                        var uploadResults = await _cloudinary.UploadAsync(uploadParams);
                        producto.FotoProducto = uploadResults.SecureUrl.ToString();
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.IdProdcuto))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.IdProdcuto == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProdcuto == id);
        }

        /*Nuevo*/

        public IActionResult PreviewPurchase(int id)
        {
            // Buscar el producto por su ID
            var producto = _context.Productos.FirstOrDefault(p => p.IdProdcuto == id);
            if (producto == null)
            {
                return NotFound();
            }

            // Crear un modelo temporal con los datos del producto
            var productoPreview = new ProductoPreviewViewModel
            {
                IdProducto = producto.IdProdcuto,
                CodigoProducto = producto.CodigoProducto,
                NombreProducto = producto.NombreProducto,
                PrecioProducto = producto.PrecioProducto,
                FotoProducto = producto.FotoProducto,
                StockProducto = producto.StockProducto
            };

            return View(productoPreview);
        }


        [HttpPost]
        public IActionResult ConfirmPurchase(int IdProducto, int CantidadSeleccionada)
        {
            var producto = _context.Productos.FirstOrDefault(p => p.IdProdcuto == IdProducto);
            if (producto == null || CantidadSeleccionada <= 0 || CantidadSeleccionada > producto.StockProducto)
            {
                return BadRequest("Datos inválidos o cantidad fuera de rango.");
            }

            // Calcular el total
            decimal total = producto.PrecioProducto * CantidadSeleccionada;

            // Redirigir a la acción Create del controlador de ventas con el total
            return RedirectToAction("Create", "Ventas", new { totalVenta = total });
        }
    }
}
