using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIWeb.Data;
using APIWeb.Models;

namespace APIWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ContextoBD _context;

        public SuppliersController(ContextoBD context)
        {
            _context = context;
        }

        // GET: api/Suppliers (público)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetSuppliers()
        {
            return await _context.Suppliers.ToListAsync();
        }

        // GET: api/Suppliers/con-productos (Consulta personalizada protegida con JWT)
        // Master-Detail: Trae los proveedores con sus productos asociados y cálculo de inventario
        [HttpGet("con-productos")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ProveedorConProductosDto>>> GetProveedoresConProductos()
        {
            var resultado = await _context.Suppliers
                .Select(s => new ProveedorConProductosDto
                {
                    SupplierId = s.SupplierId,
                    CompanyName = s.CompanyName,
                    ContactName = s.ContactName,
                    ContactTitle = s.ContactTitle,
                    City = s.City,
                    Country = s.Country,
                    Phone = s.Phone,
                    TotalProductos = s.Products.Count(),
                    ValorInventario = s.Products.Sum(p => (p.UnitPrice ?? 0) * (p.UnitsInStock ?? 0)),
                    Productos = s.Products.Select(p => new ProductoResumenDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        UnitPrice = p.UnitPrice,
                        UnitsInStock = p.UnitsInStock,
                        CategoryName = p.Category != null ? p.Category.CategoryName : "Sin categoría",
                        Discontinued = p.Discontinued
                    }).ToList()
                })
                .OrderByDescending(s => s.TotalProductos)
                .ToListAsync();

            return Ok(resultado);
        }
    }
}