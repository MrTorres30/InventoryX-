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
    public IReadOnlyList<Product> GetAll()
    {
        var products = new List<Product>();
        const string query = @"
            SELECT Id, Name, SKU, Price, Stock, Category, CreatedAt
            FROM Products
            ORDER BY Id ASC;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        return products.AsReadOnly();
    }
    public Product? GetById(int id)
    {
        const string query = @"
            SELECT Id, Name, SKU, Price, Stock, Category, CreatedAt
            FROM Products
            WHERE Id = @Id;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapProduct(reader);
        }
        return null;
    }
    public Product? GetBySku(string sku)
    {
        const string query = @"
            SELECT Id, Name, SKU, Price, Stock, Category, CreatedAt
            FROM Products
            WHERE SKU = @SKU;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@SKU", sku);
        connection.Open();
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapProduct(reader);
        }
        return null;
    }
    public bool Update(Product product)
    {
        const string query = @"
            UPDATE Products 
            SET Name = @Name, Price = @Price, Category = @Category
            WHERE Id = @Id;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", product.Id);
        command.Parameters.AddWithValue("@Name", product.Name);
        command.Parameters.AddWithValue("@Price", product.Price);
        command.Parameters.AddWithValue("@Category", product.Category);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }
    public bool Delete(int id)
    {
        const string query = "DELETE FROM Products WHERE Id = @Id;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }
    public bool UpdateStock(int id, int newStock)
    {
        const string query = "UPDATE Products SET Stock = @Stock WHERE Id = @Id;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Id", id);
        command.Parameters.AddWithValue("@Stock", newStock);
        connection.Open();
        int rowsAffected = command.ExecuteNonQuery();
        return rowsAffected > 0;
    }
    public IReadOnlyList<Product> GetLowStock(int threshold)
    {
        var products = new List<Product>();
        const string query = @"
            SELECT Id, Name, SKU, Price, Stock, Category, CreatedAt
            FROM Products
            WHERE Stock <= @Threshold
            ORDER BY Stock ASC;";
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@Threshold", threshold);
        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }
        return products.AsReadOnly();
    }
    private static Product MapProduct(SqlDataReader reader)
    {
        return new Product
        {
            Id        = reader.GetInt32(0),
            Name      = reader.GetString(1),
            SKU       = reader.GetString(2),
            Price     = reader.GetDecimal(3),
            Stock     = reader.GetInt32(4),
            Category  = reader.GetString(5),
            CreatedAt = reader.GetDateTime(6)
        };
    }
}