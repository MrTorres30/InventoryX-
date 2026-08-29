using InventoryX.Models;

namespace InventoryX.Repository
{
    public interface InventoryX
    {
        void Add(Product product);
        IReadOnlyList<Product> GetAll();
        Product? GetById (int id);
        Product? GetBySku (string sku);
        bool Update (Product product);
        bool Delete (int id);
        bool UpdateStock(int id, int newStock);
        IReadOnlyList<Product> GetLowStock (int threshold);
    }

}