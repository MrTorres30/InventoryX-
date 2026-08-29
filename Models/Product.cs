namespace InventoryX.Models
{
    public class Product
    {
        int Id {get; set;}
        string Name {get; set;} = String.Empty;
        string SKU {get; set;} = String.Empty;
        decimal Price {get; set;}
        int Stock {get; set;}
        string Category {get; set;} = string.Empty;
        DateTime CreatedAt {get; set;}
    }
}