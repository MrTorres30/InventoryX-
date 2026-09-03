using System.Data.Common;
using InventoryX.Repositories;
using InventoryX.Services;
using InventoryX.UI;

const string connectionString  =
"Server=.;Database=InventoryXDb;Trusted_Connection=True;TrustServerCertificate=True;";

IProductRepository repository = new SqlProductRepository(connectionString);
var inventoryService          = new InventoryService(repository);
var ui                        = new ConsoleInterface(inventoryService);

ui.Run();