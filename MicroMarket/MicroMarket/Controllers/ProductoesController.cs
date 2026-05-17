using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Contexto;
using MicroMarket.Models;

using Microsoft.AspNetCore.Hosting;

namespace MicroMarket.Controllers
{
    public class ProductoesController : Controller
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductoesController(MyContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Productoes
        public async Task<IActionResult> Index(string? tipoProductoBusqueda)
        {
            var productos = from p in _context.Productos
                           select p;

            if (!string.IsNullOrEmpty(tipoProductoBusqueda))
            {
                productos = productos.Where(p => p.TipoProducto != null && p.TipoProducto.Contains(tipoProductoBusqueda));
            }

            return View(await productos.ToListAsync());
        }

        // GET: Productoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
        
            var producto = await _context.Productos
                .Include(p => p.DetallesVentas)
                .FirstOrDefaultAsync(m => m.ProductoId == id);

            if (producto == null) return NotFound();


            return View(producto);
        }

        // GET: Productoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Productoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductoId,Precio,Descripcion,Stock,StockMinimo,StockMaximo,TipoProducto,FechaVencimiento,UrlFoto,FotoFile")] Producto producto)
        {
            if (producto.FotoFile != null && producto.FotoFile.Length > 0)
            {
                var extension = Path.GetExtension(producto.FotoFile.FileName).ToLowerInvariant();
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("FotoFile", "Solo se permiten imágenes JPG, PNG o WEBP.");
                }
                else
                {
                    var carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "fotos");

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await producto.FotoFile.CopyToAsync(stream);
                    }

                    producto.UrlFoto = $"/fotos/{nombreArchivo}";
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(producto);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        // GET: Productoes/Edit/5
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

        // POST: Productoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductoId,Precio,Descripcion,Stock,StockMinimo,StockMaximo,TipoProducto,FechaVencimiento,UrlFoto,FotoFile")] Producto producto)
        {
            if (id != producto.ProductoId)
            {
                return NotFound();
            }

            if (producto.FotoFile != null && producto.FotoFile.Length > 0)
            {
                var extension = Path.GetExtension(producto.FotoFile.FileName).ToLowerInvariant();
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };

                if (!extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("FotoFile", "Solo se permiten imágenes JPG, PNG o WEBP.");
                }
                else
                {
                    var carpeta = Path.Combine(_webHostEnvironment.WebRootPath, "fotos");

                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }

                    var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                    var rutaArchivo = Path.Combine(carpeta, nombreArchivo);

                    using (var stream = new FileStream(rutaArchivo, FileMode.Create))
                    {
                        await producto.FotoFile.CopyToAsync(stream);
                    }

                    producto.UrlFoto = $"/fotos/{nombreArchivo}";
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductoExists(producto.ProductoId))
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

        // GET: Productoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.ProductoId == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productoes/Delete/5
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
            return _context.Productos.Any(e => e.ProductoId == id);
        }
    }
}
