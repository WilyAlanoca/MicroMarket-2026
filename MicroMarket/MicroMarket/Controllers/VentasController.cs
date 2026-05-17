using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Contexto;
using MicroMarket.Models;
using MicroMarket.Models.ViewModels;
using System.Text.Json;

namespace MicroMarket.Controllers
{
    public class VentasController : Controller
    {
        private readonly MyContext _context;

        public VentasController(MyContext context)
        {
            _context = context;
        }

        // GET: Ventas
        public async Task<IActionResult> Index()
        {
            var myContext = _context.Ventas.Include(v => v.Cliente);
            return View(await myContext.ToListAsync());
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
                .Include(v => v.Detalles)
                    .ThenInclude(d => d.Producto)
                .FirstOrDefaultAsync(m => m.VentaId == id);

            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        // GET: Ventas/Create
        public IActionResult Create()
        {
            CargarDatosCreate();

            var model = new VentaCreateViewModel
            {
                Detalles = new List<DetalleVentaInput>
        {
            new DetalleVentaInput()
        }
            };

            return View(model);
        }

        // POST: Ventas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VentaCreateViewModel model)
        {
            model.Detalles = model.Detalles
                .Where(d => d.ProductoId > 0 && d.Cantidad > 0)
                .ToList();

            if (!model.Detalles.Any())
            {
                ModelState.AddModelError("", "Debe agregar al menos un producto a la venta.");
            }

            if (!ModelState.IsValid)
            {
                CargarDatosCreate(model.ClienteId);
                return View(model);
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var venta = new Venta
                {
                    ClienteId = model.ClienteId,
                    FechaEmision = DateTime.Now,
                    Estado = "Emitida",
                    MontoTotal = 0
                };

                foreach (var item in model.Detalles)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);

                    if (producto == null)
                    {
                        ModelState.AddModelError("", "Uno de los productos seleccionados no existe.");
                        continue;
                    }

                    if (producto.Stock < item.Cantidad)
                    {
                        ModelState.AddModelError("", $"Stock insuficiente para {producto.Descripcion}. Stock disponible: {producto.Stock}.");
                        continue;
                    }

                    var subtotal = producto.Precio * item.Cantidad;

                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = producto.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = producto.Precio,
                        SubTotal = subtotal
                    });

                    producto.Stock -= item.Cantidad;
                    venta.MontoTotal += subtotal;

                    if (producto.Stock <= producto.StockMinimo)
                    {
                        TempData["StockMinimo"] += $"El producto {producto.Descripcion} llegó al stock mínimo. ";
                    }
                }

                if (!ModelState.IsValid)
                {
                    await transaction.RollbackAsync();
                    CargarDatosCreate(model.ClienteId);
                    return View(model);
                }

                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return RedirectToAction(nameof(Details), new { id = venta.VentaId });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private void CargarDatosCreate(int? clienteId = null)
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Info", clienteId);
            ViewData["Productos"] = new SelectList(_context.Productos, "ProductoId", "Descripcion");

            ViewData["ClientesJson"] = _context.Clientes
                .Select(c => new
                {
                    c.ClienteId,
                    c.NumeroDocumento,
                    c.Complemento,
                    c.NombreRazonSocial,
                    c.Email
                })
                .ToList();
                ViewData["ProductosJson"] = _context.Productos
                .Select(p => new
                {
                    p.ProductoId,
                    p.Descripcion,
                    p.Precio,
                    p.Stock,
                    p.StockMinimo
                })
                .ToList();
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Complemento", venta.ClienteId);
            return View(venta);
        }

        // POST: Ventas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VentaId,FechaEmision,ClienteId,MontoTotal,Estado")] Venta venta)
        {
            if (id != venta.VentaId)
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
                    if (!VentaExists(venta.VentaId))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "ClienteId", "Complemento", venta.ClienteId);
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
                .FirstOrDefaultAsync(m => m.VentaId == id);
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
            return _context.Ventas.Any(e => e.VentaId == id);
        }
    }
}
