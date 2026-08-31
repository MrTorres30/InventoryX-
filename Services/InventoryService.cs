using InventoryX.Models;
using InventoryX.Repositories;
namespace InventoryX.Services;
public class InventoryService
{
    private readonly IProductRepository _repository;
    public InventoryService(IProductRepository repository)
    {
        _repository = repository;
    }
    public (bool Success, string Message, Product? Product) CreateProduct(
        string name, string sku, decimal price, int stock, string category)
    {
        if (string.IsNullOrWhiteSpace(name))
            return (false, "El nombre del producto no puede estar vacío.", null);
        if (string.IsNullOrWhiteSpace(sku))
            return (false, "El SKU no puede estar vacío.", null);
        if (price <= 0)
            return (false, "El precio debe ser mayor a 0.", null);
        if (stock < 0)
            return (false, "El stock inicial no puede ser negativo.", null);
        // Validar que el SKU no esté duplicado en la base de datos
        var existingProduct = _repository.GetBySku(sku.Trim().ToUpper());
        if (existingProduct != null)
            return (false, $"Ya existe un producto con el SKU '{sku.ToUpper()}'.", null);
        var product = new Product
        {
            Name      = name.Trim(),
            SKU       = sku.Trim().ToUpper(),
            Price     = price,
            Stock     = stock,
            Category  = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim(),
            CreatedAt = DateTime.UtcNow
        };
        _repository.Add(product);
        return (true, "Producto registrado exitosamente en la base de datos.", product);
    }
    public IReadOnlyList<Product> GetAllProducts()
    {
        return _repository.GetAll();
    }
    public Product? GetProductById(int id)
    {
        return _repository.GetById(id);
    }
    public Product? GetProductBySku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
            return null;
        return _repository.GetBySku(sku.Trim().ToUpper());
    }
    public (bool Success, string Message) UpdateProduct(int id, string name, decimal price, string category)
    {
        var product = _repository.GetById(id);
        if (product is null)
            return (false, "No se encontró ningún producto con ese ID.");
        if (string.IsNullOrWhiteSpace(name))
            return (false, "El nombre no puede estar vacío.");
        if (price <= 0)
            return (false, "El precio debe ser mayor a 0.");
        product.Name     = name.Trim();
        product.Price    = price;
        product.Category = string.IsNullOrWhiteSpace(category) ? "General" : category.Trim();
        bool updated = _repository.Update(product);
        return updated 
            ? (true, "Producto actualizado correctamente.") 
            : (false, "No se pudo actualizar el producto.");
    }
    public (bool Success, string Message) AdjustStock(int id, int amount)
    {
        var product = _repository.GetById(id);
        if (product is null)
            return (false, "No se encontró ningún producto con ese ID.");
        int newStock = product.Stock + amount;
        if (newStock < 0)
            return (false, $"Stock insuficiente. Stock actual: {product.Stock}, intento de ajuste: {amount}.");
        bool updated = _repository.UpdateStock(id, newStock);
        return updated 
            ? (true, $"Stock actualizado con éxito. Nuevo stock: {newStock}.") 
            : (false, "No se pudo actualizar el stock en la base de datos.");
    }
    public (bool Success, string Message) DeleteProduct(int id)
    {
        var product = _repository.GetById(id);
        if (product is null)
            return (false, "No se encontró ningún producto con ese ID.");
        bool deleted = _repository.Delete(id);
        return deleted 
            ? (true, $"Producto '{product.Name}' eliminado de la base de datos.") 
            : (false, "No se pudo eliminar el producto.");
    }
    public IReadOnlyList<Product> GetLowStockAlerts(int threshold = 5)
    {
        return _repository.GetLowStock(threshold);
    }
}