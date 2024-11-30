using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Comprobación.Data;
using Comprobación.Models;
using static QuestPDF.Helpers.Colors;
using Microsoft.Data.SqlClient;
using Comprobación.DTOs;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Comprobación.Controllers
{
    public class VentasController : Controller
    {
        private readonly AppDbContext _context;

        public VentasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Ventas
        
        public async Task<IActionResult> Index(decimal? total = null)
        {
            ViewData["TotalVenta"] = total;
            var appDbContext = _context.Ventas.Include(v => v.Cliente).Include(v => v.Empleado);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Ventas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create


        [HttpGet]
        [Route("Ventas/Create")]
        public IActionResult Create(decimal? totalVenta = null)
        {
            ViewData["IdClienteEnVenta"] = new SelectList(_context.Clientes, "IdCliente", "ApellidoCliente");
            ViewData["IdEmpleadoEnVenta"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado");

            var venta = new Venta();
            if (totalVenta.HasValue)
            {
                venta.TotalVenta = totalVenta.Value;
            }

            return View(venta);
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdVenta,FechaVenta,IdClienteEnVenta,IdEmpleadoEnVenta,TotalVenta")] Venta venta)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(venta);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    var sqlException = ex.InnerException as SqlException;
                    if (sqlException != null)
                    {
                        ModelState.AddModelError("", $"Error de base de datos: {sqlException.Message}");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Ocurrió un error al guardar los datos.");
                    }
                }
            }
            ViewData["IdClienteEnVenta"] = new SelectList(_context.Clientes, "IdCliente", "ApellidoCliente", venta.IdClienteEnVenta);
            ViewData["IdEmpleadoEnVenta"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", venta.IdEmpleadoEnVenta);
            return View(venta);
        }

        // GET: Ventas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas.FindAsync(id);
            if (venta == null)
            {
                return NotFound();
            }
            ViewData["IdClienteEnVenta"] = new SelectList(_context.Clientes, "IdCliente", "ApellidoCliente", venta.IdClienteEnVenta);
            ViewData["IdEmpleadoEnVenta"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", venta.IdEmpleadoEnVenta);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdVenta,FechaVenta,IdClienteEnVenta,IdEmpleadoEnVenta,TotalVenta")] Venta venta)
        {
            if (id != venta.IdVenta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(venta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VentaExists(venta.IdVenta))
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
            ViewData["IdClienteEnVenta"] = new SelectList(_context.Clientes, "IdCliente", "ApellidoCliente", venta.IdClienteEnVenta);
            ViewData["IdEmpleadoEnVenta"] = new SelectList(_context.Empleados, "IdEmpleado", "ApellidoEmpleado", venta.IdEmpleadoEnVenta);
            return View(venta);
        }

        // GET: Ventas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venta = await _context.Ventas
                .Include(v => v.Cliente)
                .Include(v => v.Empleado)
                .FirstOrDefaultAsync(m => m.IdVenta == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // POST: Ventas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venta = await _context.Ventas.FindAsync(id);
            if (venta != null)
            {
                _context.Ventas.Remove(venta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VentaExists(int id)
        {
            return _context.Ventas.Any(e => e.IdVenta == id);
        }
        
    }
}
