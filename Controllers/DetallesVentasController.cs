using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Comprobación.Data;
using Comprobación.Models;

namespace Comprobación.Controllers
{
    public class DetallesVentasController : Controller
    {
        private readonly AppDbContext _context;

        public DetallesVentasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var appDbContext = _context.Detalle_Ventas.Include(d => d.Producto).Include(d => d.Venta);

            // Si se proporciona un IdVenta, busca los detalles de esa venta
            Venta ventaSeleccionada = null;
            if (id.HasValue)
            {
                ventaSeleccionada = await _context.Ventas
                    .Include(v => v.Cliente)
                    .Include(v => v.Empleado)
                    .FirstOrDefaultAsync(v => v.IdVenta == id);
            }

            // Crea un ViewModel que combine la lista de DetallesVentas y la venta seleccionada
            var viewModel = new DetallesVentasViewModel
            {
                DetallesVentas = await appDbContext.ToListAsync(),
                VentaSeleccionada = ventaSeleccionada
            };

            return View(viewModel);
        }


        // GET: DetallesVentas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVentas = await _context.Detalle_Ventas
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .FirstOrDefaultAsync(m => m.DetalleVentaId == id);
            if (detallesVentas == null)
            {
                return NotFound();
            }

            return View(detallesVentas);
        }

        // GET: DetallesVentas/Create
        public IActionResult Create()
        {
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProdcuto", "CodigoProducto");
            ViewData["VentaId"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta");
            return View();
        }

        // POST: DetallesVentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetalleVentaId,VentaId,ProductoId,Cantidad,PrecioUnitario,Subtotal")] DetallesVentas detallesVentas)
        {
            if (ModelState.IsValid)
            {
                _context.Add(detallesVentas);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProdcuto", "CodigoProducto", detallesVentas.ProductoId);
            ViewData["VentaId"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesVentas.VentaId);
            return View(detallesVentas);
        }

        // GET: DetallesVentas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVentas = await _context.Detalle_Ventas.FindAsync(id);
            if (detallesVentas == null)
            {
                return NotFound();
            }
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProdcuto", "CodigoProducto", detallesVentas.ProductoId);
            ViewData["VentaId"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesVentas.VentaId);
            return View(detallesVentas);
        }

        // POST: DetallesVentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetalleVentaId,VentaId,ProductoId,Cantidad,PrecioUnitario,Subtotal")] DetallesVentas detallesVentas)
        {
            if (id != detallesVentas.DetalleVentaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(detallesVentas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DetallesVentasExists(detallesVentas.DetalleVentaId))
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
            ViewData["ProductoId"] = new SelectList(_context.Productos, "IdProdcuto", "CodigoProducto", detallesVentas.ProductoId);
            ViewData["VentaId"] = new SelectList(_context.Ventas, "IdVenta", "IdVenta", detallesVentas.VentaId);
            return View(detallesVentas);
        }

        // GET: DetallesVentas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var detallesVentas = await _context.Detalle_Ventas
                .Include(d => d.Producto)
                .Include(d => d.Venta)
                .FirstOrDefaultAsync(m => m.DetalleVentaId == id);
            if (detallesVentas == null)
            {
                return NotFound();
            }

            return View(detallesVentas);
        }

        // POST: DetallesVentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var detallesVentas = await _context.Detalle_Ventas.FindAsync(id);
            if (detallesVentas != null)
            {
                _context.Detalle_Ventas.Remove(detallesVentas);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DetallesVentasExists(int id)
        {
            return _context.Detalle_Ventas.Any(e => e.DetalleVentaId == id);
        }
        /*
        public async Task<IActionResult> Pos()
        {
            var productos = await _context.Productos.ToListAsync(); // Recuperar todos los productos
            return View();
        }*/

    }
}
