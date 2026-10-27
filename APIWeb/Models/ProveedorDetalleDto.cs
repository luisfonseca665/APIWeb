using System.Collections.Generic;

namespace APIWeb.Models
{
    // Detalle simplificado de un producto para la vista de proveedores
    public class ProductoResumenDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public string? CategoryName { get; set; }
        public bool? Discontinued { get; set; }
    }

    // Datos del proveedor junto con sus estadísticas y la lista de artículos que surte
    public class ProveedorConProductosDto
    {
        public int SupplierId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? Phone { get; set; }
        public int TotalProductos { get; set; }
        public decimal ValorInventario { get; set; }
        public List<ProductoResumenDto> Productos { get; set; } = new();
    }
}
