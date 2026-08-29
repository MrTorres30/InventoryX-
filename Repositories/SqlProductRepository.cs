using InventoryX.Models;
using Microsoft.Data.SqlClient;
namespace InventoryX.Repositories;
public class SqlProductRepository : IProductRepository 
{
    private readonly string _connectionString;
    public SqlProductRepository(string connectionString)
    {
        _connectionString = connectionString;
    }
    public void Add(Product product)
    {
        const string query = @"
            INSERT INTO Products (Name, SKU, Price, Stock, Category, CreatedAt)
            VALUES (@Name, @SKU, @Price, @Stock, @Category, @CreatedAt);
            SELECT SCOPE_IDENTITY();";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@SKU", product.SKU);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Stock", product.Stock);
        command.Parameters.AddWithValue("@Category", product.Category);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
        
        connection.Open();
        var generatedId = command.ExecuteScalar();
        if (generatedId != null && int.TryParse(generatedId.ToString(), out int id))
        {
            product.Id = id;
        }
    }   
    public IReadOnlyList<Product> GetAll() => throw new NotImplementedException();
    public Product? GetById(int id) => throw new NotImplementedException();
    public Product? GetBySku(string sku) => throw new NotImplementedException();
    public bool Update(Product product) => throw new NotImplementedException();
    public bool Delete(int id) => throw new NotImplementedException();
    public bool UpdateStock(int id, int newStock) => throw new NotImplementedException();
    public IReadOnlyList<Product> GetLowStock(int threshold) => throw new NotImplementedException();
}