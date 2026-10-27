using System.Collections.Generic;

namespace APIWeb.Models
{
    // Datos del producto resumidos para mostrar dentro del proveedor
    public class ProductoResumenDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal? UnitPrice { get; set; }
        public int? UnitsInStock { get; set; }
        public string? CategoryName { get; set; }
        public bool? Discontinued { get; set; }
    }

    // Consulta personalizada Master-Detail: Proveedor + Estadísticas + Lista de Productos
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
        public decimal ValorInventario { get; set; } // Suma estimada de su catálogo
        public List<ProductoResumenDto> Productos { get; set; } = new();
    }
}
