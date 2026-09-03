using System.Security.Cryptography.X509Certificates;
using InventoryX.Models;
using InventoryX.Services;
using Microsoft.IdentityModel.Tokens;

namespace InventoryX.UI
{
    public class ConsoleInterface
    {
            private readonly InventoryService _inventoryService;

            public ConsoleInterface (InventoryService inventoryService)
            {
                _inventoryService = inventoryService;
            }

            public void Run()
            {
                bool running = true;


                while (running)
                {
                    ShowMenu();

                    string input  = Console.ReadLine() ?? string.Empty;
                    bool   parsed = int.TryParse(input, out int option);
                    if (!parsed)
                        {
                        Console.WriteLine("\n Opcion invalida. Ingresa un numero del 1 al 8. \n");  

                        }

                        switch (option)
                    {
                        
                        case 1: CreateProduct();        break;
                        case 2: ListAllProducts();       break;
                        case 3: SearchProduct();        break;
                        case 4: AdjustStock();          break;
                        case 5: UpdateProduct();        break;
                        case 6: DeleteProduct();        break;
                        case 7: ShowLowStockAlerts();   break;
                        case 8: running = false;        break;
                        default:
                        Console.WriteLine("\n  Opción fuera de rango. Elige entre 1 y 8.\n");
                        break;
                    }
                    
                    Console.WriteLine("\n Gracias por usar Inventory X,¡Hasta luego!\n");
                }
            }

            private void ShowMenu()
            {
                Console.WriteLine("==================================================");
            Console.WriteLine("          INVENTORY-X : SQL ENGINE                ");
            Console.WriteLine("==================================================");
            Console.WriteLine("1.  Registrar nuevo producto");
            Console.WriteLine("2.  Listar inventario completo");
            Console.WriteLine("3.  Buscar producto (ID / SKU)");
            Console.WriteLine("4.  Ajustar stock (Entrada / Salida)");
            Console.WriteLine("5.   Actualizar producto");
            Console.WriteLine("6.   Eliminar producto");
            Console.WriteLine("7.   Reporte de stock bajo (< 5 unidades)");
            Console.WriteLine("8.  Salir");
            Console.Write("\nElige una opción (1-8): ");


            }

        private void CreateProduct()
        {
            Console.WriteLine("\n--- REGISTRAR NUEVO PRODUCTO ---");
            Console.Write("Nombre: ");
            string name = Console.ReadLine() ?? string.Empty;
            Console.Write("SKU (Código único, ej: PROD-101): ");
            string sku = Console.ReadLine() ?? string.Empty;
            Console.Write("Precio ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("  El precio debe ser un número decimal válido mayor a 0.\n");
                return;
            }
            Console.Write("Stock inicial: ");
            if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
            {
                Console.WriteLine("  El stock inicial debe ser un número entero mayor o igual a 0.\n");
                return;
            }
            Console.Write("Categoría: ");
            string category = Console.ReadLine() ?? string.Empty;
            var result = _inventoryService.CreateProduct(name, sku, price, stock, category);
            if (result.Success)
                Console.WriteLine($"\n {result.Message} (ID asignado por SQL Server: {result.Product?.Id})\n");
            else
                Console.WriteLine($"\n Error: {result.Message}\n");
        }
        private void ListAllProducts()
        {
            Console.WriteLine("\n--- INVENTARIO COMPLETO (SQL SERVER) ---");
            var products = _inventoryService.GetAllProducts();
            PrintProductTable(products);
        }
        private void SearchProduct()
        {
            Console.WriteLine("\n--- BUSCAR PRODUCTO ---");
            Console.WriteLine("1. Buscar por ID");
            Console.WriteLine("2. Buscar por SKU");
            Console.Write("Elige una opción: ");
            string choice = Console.ReadLine() ?? string.Empty;
            Product? product = null;
            if (choice == "1")
            {
                Console.Write("Ingresa el ID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                    product = _inventoryService.GetProductById(id);
                else
                    Console.WriteLine("  ID inválido.");
            }
            else if (choice == "2")
            {
                Console.Write("Ingresa el SKU: ");
                string sku = Console.ReadLine() ?? string.Empty;
                product = _inventoryService.GetProductBySku(sku);
            }
            else
            {
                Console.WriteLine("  Opción de búsqueda no válida.\n");
                return;
            }
            if (product != null)
            {
                Console.WriteLine("\n Producto encontrado:");
                PrintProductTable(new List<Product> { product });
            }
            else
            {
                Console.WriteLine("\n  No se encontró ningún producto con ese criterio.\n");
            }
        }
        private void AdjustStock()
        {
            Console.WriteLine("\n--- AJUSTE DE STOCK ---");
            Console.Write("Ingresa el ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("  El ID debe ser un número.\n");
                return;
            }
            Console.Write("Cantidad a ajustar (+ para entrada, - para salida/venta): ");
            if (!int.TryParse(Console.ReadLine(), out int amount))
            {
                Console.WriteLine("  La cantidad debe ser un número entero.\n");
                return;
            }
            var result = _inventoryService.AdjustStock(id, amount);
            if (result.Success)
                Console.WriteLine($"\n {result.Message}\n");
            else
                Console.WriteLine($"\n {result.Message}\n");
        }
        private void UpdateProduct()
        {
            Console.WriteLine("\n--- ACTUALIZAR PRODUCTO ---");
            Console.Write("Ingresa el ID del producto a modificar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("  El ID debe ser un número.\n");
                return;
            }
            Console.Write("Nuevo Nombre: ");
            string name = Console.ReadLine() ?? string.Empty;
            Console.Write("Nuevo Precio ($): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("  El precio debe ser un número decimal mayor a 0.\n");
                return;
            }
            Console.Write("Nueva Categoría: ");
            string category = Console.ReadLine() ?? string.Empty;
            var result = _inventoryService.UpdateProduct(id, name, price, category);
            if (result.Success)
                Console.WriteLine($"\n {result.Message}\n");
            else
                Console.WriteLine($"\n {result.Message}\n");
        }
        private void DeleteProduct()
        {
            Console.WriteLine("\n--- ELIMINAR PRODUCTO (SQL SERVER) ---");
            Console.Write("Ingresa el ID del producto a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("  El ID debe ser un número.\n");
                return;
            }
            Console.Write("  ¿Estás 100% seguro de borrar este producto de la BD? (s/n): ");
            string confirm = Console.ReadLine() ?? string.Empty;
            if (confirm.Trim().ToLower() != "s")
            {
                Console.WriteLine(" Operación cancelada por el usuario.\n");
                return;
            }
            var result = _inventoryService.DeleteProduct(id);
            if (result.Success)
                Console.WriteLine($"\n {result.Message}\n");
            else
                Console.WriteLine($"\n {result.Message}\n");
        }
        private void ShowLowStockAlerts()
        {
            Console.WriteLine("\n---  REPORTES DE STOCK CRÍTICO (< 5 UNIDADES) ---");
            var alerts = _inventoryService.GetLowStockAlerts(5);
            if (alerts.Count == 0)
            {
                Console.WriteLine(" No hay productos en estado crítico de stock.\n");
                return;
            }
            PrintProductTable(alerts);
        }
        private static void PrintProductTable(IReadOnlyList<Product> products)
        {
            if (products.Count == 0)
            {
                Console.WriteLine("No hay productos para mostrar.\n");
                return;
            }
            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"| {"ID",-4} | {"SKU",-10} | {"NOMBRE",-25} | {"PRECIO",-10} | {"STOCK",-6} | {"CATEGORÍA",-15} |");
            Console.WriteLine(new string('-', 85));
            foreach (var p in products)
            {
                Console.WriteLine($"| {p.Id,-4} | {p.SKU,-10} | {p.Name,-25} | {p.Price,9:C} | {p.Stock,6} | {p.Category,-15} |");
            }
            Console.WriteLine(new string('-', 85));
            Console.WriteLine($"Total registros: {products.Count}\n");
        }
    }
}