using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MicroMarket.Contexto;
using MicroMarket.Models;

using Microsoft.AspNetCore.Identity;

namespace MicroMarket.Controllers
{
    public class VendedorsController : Controller
    {
        private readonly MyContext _context;
        private readonly IPasswordHasher<Vendedor> _passwordHasher;

        public VendedorsController(MyContext context, IPasswordHasher<Vendedor> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // GET: Vendedors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Vendedores.ToListAsync());
        }

        // GET: Vendedors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores
                .FirstOrDefaultAsync(m => m.VendedorId == id);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(vendedor);
        }

        // GET: Vendedors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vendedors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
        [Bind("VendedorId,Nombre,Email,Telefono,Direccion,Rol")] Vendedor vendedor,
        string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    ModelState.AddModelError("PasswordHash", "La contraseña es obligatoria.");
                }

                ModelState.Remove("PasswordHash");

                if (ModelState.IsValid)
                {
                    vendedor.PasswordHash = _passwordHasher.HashPassword(vendedor, password);

                    _context.Add(vendedor);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }

                return View(vendedor);
            }

        // GET: Vendedors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores.FindAsync(id);
            if (vendedor == null)
            {
                return NotFound();
            }
            return View(vendedor);
        }

        // POST: Vendedors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
        int id,
        [Bind("VendedorId,Nombre,Email,Telefono,Direccion,Rol")] Vendedor vendedor,
        string? nuevaPassword)
        {
            if (id != vendedor.VendedorId)
            {
                return NotFound();
            }

            ModelState.Remove("PasswordHash");

            if (ModelState.IsValid)
            {
                var vendedorDb = await _context.Vendedores.FindAsync(id);

                if (vendedorDb == null)
                {
                    return NotFound();
                }

                vendedorDb.Nombre = vendedor.Nombre;
                vendedorDb.Email = vendedor.Email;
                vendedorDb.Telefono = vendedor.Telefono;
                vendedorDb.Direccion = vendedor.Direccion;
                vendedorDb.Rol = vendedor.Rol;

                if (!string.IsNullOrWhiteSpace(nuevaPassword))
                {
                    vendedorDb.PasswordHash = _passwordHasher.HashPassword(vendedorDb, nuevaPassword);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(vendedor);
        }

        // GET: Vendedors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendedor = await _context.Vendedores
                .FirstOrDefaultAsync(m => m.VendedorId == id);
            if (vendedor == null)
            {
                return NotFound();
            }

            return View(vendedor);
        }

        // POST: Vendedors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendedor = await _context.Vendedores.FindAsync(id);

            if (vendedor != null)
            {
                if (vendedor.Rol == TipoRol.Administrador)
                {
                    var administradores = await _context.Vendedores
                        .CountAsync(v => v.Rol == TipoRol.Administrador);

                    if (administradores <= 1)
                    {
                        TempData["Error"] = "No puedes eliminar el último administrador.";
                        return RedirectToAction(nameof(Index));
                    }
                }

                _context.Vendedores.Remove(vendedor);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VendedorExists(int id)
        {
            return _context.Vendedores.Any(e => e.VendedorId == id);
        }
    }
}
