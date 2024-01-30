using FullCartApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using FullCartApi.DTO;

namespace FullCartApi.Repositories
{
    public class ProductRepository
    {
        private readonly EcommerceDbContext _context;

        public ProductRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products.ToList();
        }

        public Product GetProductById(int productId)
        {
            return _context.Products.FirstOrDefault(p => p.ProductId == productId);
        }

        public void AddProduct(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public IEnumerable<Brand> GetAllBrands()
        {
            return _context.Brands.ToList();
        }

        // Additional methods for bulk operations, category/brand management, etc., can be added as needed.

        // Example method for retrieving products based on category
        public IEnumerable<Product> GetProductsByCategory(int categoryId)
        {
            return _context.Products.Where(p => p.CategoryId == categoryId).ToList();
        }

        // Example method for retrieving products based on brand
        public IEnumerable<Product> GetProductsByBrand(int brandId)
        {
            return _context.Products.Where(p => p.BrandId == brandId).ToList();
        }

        // Example method for handling bulk addition of products
        public void AddBulkProducts(IEnumerable<Product> products)
        {
            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public Item GetItemById(int itemId)
        {
            return _context.Items.FirstOrDefault(i => i.ItemId == itemId);
        }

        public void AddToCart(int customerId, int itemId, int quantity)
        {
            var customer = GetCustomerById(customerId);
            var item = GetItemById(itemId);

            if (customer != null && item != null)
            {
                var existingCartItem = _context.CartItems.FirstOrDefault(ci => ci.CustomerId == customerId && ci.ItemId == itemId);

                if (existingCartItem != null)
                {
                    // Item already exists in the cart; update quantity
                    existingCartItem.Quantity += quantity;
                }
                else
                {
                    // Item doesn't exist in the cart; create a new cart item
                    var newCartItem = new CartItem
                    {
                        CustomerId = customerId,
                        ItemId = itemId,
                        Quantity = quantity
                    };

                    _context.CartItems.Add(newCartItem);
                }

                _context.SaveChanges();
            }
        }

        public void RemoveFromCart(int customerId, int itemId)
        {
            var cartItem = _context.CartItems.FirstOrDefault(ci => ci.CustomerId == customerId && ci.ItemId == itemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
        }

        public void AddBrand(BrandWithImage brandWithImage)
        {
            // Validate other properties...
            if (string.IsNullOrEmpty(brandWithImage.Name))
            {
                throw new ArgumentException("Brand Name is required.");
            }

            // Save the image to server/cloud storage and get the file path
            var imagePath = SaveImageToStorage(brandWithImage.ImageFile); // Replace with your logic

            var newBrand = new Brand
            {
                Name = brandWithImage.Name,
                Image = imagePath
            };

            _context.Brands.Add(newBrand);
            _context.SaveChanges();
        }

        public void AddCategory(CategoryWithImage categoryWithImage)
        {
            // Validate other properties...
            if (string.IsNullOrEmpty(categoryWithImage.Name))
            {
                throw new ArgumentException("Category Name is required.");
            }

            // Save the image to server/cloud storage and get the file path
            var imagePath = SaveImageToStorage(categoryWithImage.ImageFile);

            var newCategory = new Category
            {
                Name = categoryWithImage.Name,
                Image = imagePath
            };

            _context.Categories.Add(newCategory);
            _context.SaveChanges();
        }

        public string SaveImageToStorage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                throw new ArgumentException("Image file is required.");
            }

            // Server Storage
            var serverStoragePath = "Images/";

            // Check if the directory exists, and create it if it doesn't
            if (!Directory.Exists(serverStoragePath))
            {
                Directory.CreateDirectory(serverStoragePath);
            }

            var imagePath = $"{Guid.NewGuid().ToString()}{Path.GetExtension(imageFile.FileName)}";
            var fullPath = Path.Combine(serverStoragePath, imagePath);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                imageFile.CopyTo(stream);
            }

            // Azure Blob Storage
            // Uncomment and configure if you want to use Azure Blob Storage
            // var connectionString = _configuration.GetConnectionString("AzureBlobStorage");
            // var containerName = "your-container-name";
            // var blobServiceClient = new BlobServiceClient(connectionString);
            // var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
            // var blobClient = blobContainerClient.GetBlobClient(imagePath);
            // blobClient.Upload(imageFile.OpenReadStream(), true);

            return fullPath; // Return the file path (or blob URL for Azure) to be saved in the database
        }

        public void UpdateCategory(int categoryId, CategoryUpdate categoryUpdate)
        {
            var existingCategory = _context.Categories.Find(categoryId);

            if (existingCategory == null)
            {
                throw new ArgumentException($"Category with ID {categoryId} not found.");
            }

            // Validate other properties...
            if (string.IsNullOrEmpty(categoryUpdate.Name))
            {
                throw new ArgumentException("Category Name is required.");
            }

            // Update the category name
            existingCategory.Name = categoryUpdate.Name;

            // Update the image if a new one is provided
            if (categoryUpdate.NewImageFile != null)
            {
                // Save the new image to storage and get the file path
                var newImagePath = SaveImageToStorage(categoryUpdate.NewImageFile);

                // Delete the existing image file if it exists
                if (!string.IsNullOrEmpty(existingCategory.Image))
                {
                    var existingImagePath = Path.Combine(Directory.GetCurrentDirectory(), existingCategory.Image);
                    if (File.Exists(existingImagePath))
                    {
                        File.Delete(existingImagePath);
                    }
                }

                existingCategory.Image = newImagePath;
            }

            _context.SaveChanges();
        }

        public void UpdateBrand(int brandId, BrandUpdate brandUpdate)
        {
            var existingBrand = _context.Brands.Find(brandId);

            if (existingBrand == null)
            {
                throw new ArgumentException($"Brand with ID {brandId} not found.");
            }

            // Validate other properties...
            if (string.IsNullOrEmpty(brandUpdate.Name))
            {
                throw new ArgumentException("Brand Name is required.");
            }

            // Update the brand name
            existingBrand.Name = brandUpdate.Name;

            // Update the image if a new one is provided
            if (brandUpdate.NewImageFile != null)
            {
                // Save the new image to storage and get the file path
                var newImagePath = SaveImageToStorage(brandUpdate.NewImageFile);

                // Delete the existing image file if it exists
                if (!string.IsNullOrEmpty(existingBrand.Image))
                {
                    var existingImagePath = Path.Combine(Directory.GetCurrentDirectory(), existingBrand.Image);
                    if (File.Exists(existingImagePath))
                    {
                        File.Delete(existingImagePath);
                    }
                }

                existingBrand.Image = newImagePath;
            }

            _context.SaveChanges();
        }

    }

}
