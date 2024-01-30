using FullCartApi.Models;
using OfficeOpenXml;

namespace FullCartApi.Repositories
{
    public class ItemRepository
    {
        private readonly EcommerceDbContext _context;

        public ItemRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public Item GetItemById(int itemId)
        {
            return _context.Items.FirstOrDefault(i => i.ItemId == itemId);
        }

        public void AddItem(Item newItem)
        {
            // Validate required fields
            if (string.IsNullOrEmpty(newItem.Name) || string.IsNullOrEmpty(newItem.Description) || newItem.Price <= 0)
            {
                throw new ArgumentException("Name, description, and price are required.");
            }

            _context.Items.Add(newItem);
            _context.SaveChanges();
        }

        public void AddItemsBulk(IFormFile file)
        {
            // Implement logic to read items from Excel file and add them to the database
            // You can use libraries like EPPlus, ClosedXML, or ExcelDataReader for reading Excel files

            // Example using EPPlus:
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var newItem = new Item
                    {
                        Name = worksheet.Cells[row, 1].Value?.ToString(),
                        Description = worksheet.Cells[row, 2].Value?.ToString(),
                        // ... Other properties
                    };

                    _context.Items.Add(newItem);
                }

                _context.SaveChanges();
            }
        }

        public void UpdateItem(Item updatedItem)
        {
            var existingItem = _context.Items.Find(updatedItem.ItemId);

            if (existingItem != null)
            {
                // Update existing item properties
                existingItem.Name = updatedItem.Name;
                existingItem.Description = updatedItem.Description;
                existingItem.Price = updatedItem.Price;
                existingItem.BrandId = updatedItem.BrandId;
                existingItem.CategoryId = updatedItem.CategoryId;
                existingItem.QuantityAvailable = updatedItem.QuantityAvailable;
                existingItem.Image = updatedItem.Image;

                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Item with ID {updatedItem.ItemId} not found.");
            }
        }

        public void UpdateItemsBulk(IFormFile file)
        {
            using (var package = new ExcelPackage(file.OpenReadStream()))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var itemId = int.Parse(worksheet.Cells[row, 1].Value?.ToString() ?? "0");
                    var existingItem = _context.Items.Find(itemId);

                    if (existingItem != null)
                    {
                        // Update existing item properties
                        existingItem.Name = worksheet.Cells[row, 2].Value?.ToString();
                        existingItem.Description = worksheet.Cells[row, 3].Value?.ToString();
                        existingItem.Price = decimal.Parse(worksheet.Cells[row, 4].Value?.ToString() ?? "0");
                        existingItem.BrandId = int.Parse(worksheet.Cells[row, 5].Value?.ToString() ?? "0");
                        existingItem.CategoryId = int.Parse(worksheet.Cells[row, 6].Value?.ToString() ?? "0");
                        existingItem.QuantityAvailable = int.Parse(worksheet.Cells[row, 7].Value?.ToString() ?? "0");
                        existingItem.Image = worksheet.Cells[row, 8].Value?.ToString();
                    }
                    else
                    {
                        // Log or handle missing item
                    }
                }

                _context.SaveChanges();
            }
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _context.Items.ToList();
        }

        public byte[] GenerateItemsExcelFile()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Items");

                // Add header row
                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Description";
                worksheet.Cells[1, 3].Value = "Price";
                // Add other properties...

                // Populate data
                var items = _context.Items.ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = items[i].Name;
                    worksheet.Cells[i + 2, 2].Value = items[i].Description;
                    worksheet.Cells[i + 2, 3].Value = items[i].Price;
                    // Add other properties...
                }

                return package.GetAsByteArray();
            }
        }

        public IEnumerable<Item> SearchItems(string searchTerm, bool sortByLowToHigh)
        {
            var query = _context.Items.AsQueryable();

            // Search functionality based on the 'Name' property
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(item => item.Name.Contains(searchTerm));
            }

            // Sorting functionality based on low to high pricing
            if (sortByLowToHigh)
            {
                query = query.OrderBy(item => item.Price);
            }

            return query.ToList();
        }

        public void DeleteItem(int itemId)
        {
            var itemToDelete = _context.Items.Find(itemId);

            if (itemToDelete != null)
            {
                _context.Items.Remove(itemToDelete);
                _context.SaveChanges();
            }
            else
            {
                throw new ArgumentException($"Item with ID {itemId} not found.");
            }
        }
    }

}
