using FullCartApi.DTO;
using FullCartApi.Models;
using FullCartApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullCartApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProductAdminController : ControllerBase
    {
        private readonly ItemRepository _itemRepository;
        private readonly ProductRepository _productRepository;

        public ProductAdminController(ItemRepository itemRepository, ProductRepository productRepository)
        {
            _itemRepository = itemRepository;
            _productRepository = productRepository;
        }

        [HttpPost("AddItem")]
        public IActionResult AddItem([FromBody] Item newItem)
        {
            try
            {
                _itemRepository.AddItem(newItem);
                return Ok("Item added successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AddItemsBulk")]
        public IActionResult AddItemsBulk([FromForm] IFormFile file)
        {
            try
            {
                _itemRepository.AddItemsBulk(file);
                return Ok("Items added in bulk successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("UpdateItem")]
        public IActionResult UpdateItem([FromBody] Item updatedItem)
        {
            try
            {
                _itemRepository.UpdateItem(updatedItem);
                return Ok("Item updated successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("UpdateItemsBulk")]
        public IActionResult UpdateItemsBulk([FromForm] IFormFile file)
        {
            try
            {
                _itemRepository.UpdateItemsBulk(file);
                return Ok("Items updated in bulk successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetAllItems")]
        public IActionResult GetAllItems()
        {
            try
            {
                var items = _itemRepository.GetAllItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("DownloadItemsExcel")]
        public IActionResult DownloadItemsExcel()
        {
            try
            {
                var excelFile = _itemRepository.GenerateItemsExcelFile();

                // Set the file content type and provide a downloadable file name
                return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Items.xlsx");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("DeleteItem/{itemId}")]
        public IActionResult DeleteItem(int itemId)
        {
            try
            {
                _itemRepository.DeleteItem(itemId);
                return Ok("Item deleted successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AddBrand")]
        public IActionResult AddBrand([FromForm] BrandWithImage brandWithImage)
        {
            try
            {
                _productRepository.AddBrand(brandWithImage);
                return Ok("Brand added successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("AddCategory")]
        public IActionResult AddCategory([FromForm] CategoryWithImage categoryWithImage)
        {
            try
            {
                _productRepository.AddCategory(categoryWithImage);
                return Ok("Category added successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("UpdateCategory/{categoryId}")]
        public IActionResult UpdateCategory(int categoryId, [FromForm] CategoryUpdate categoryUpdate)
        {
            try
            {
                _productRepository.UpdateCategory(categoryId, categoryUpdate);
                return Ok("Category updated successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPut("UpdateBrand/{brandId}")]
        public IActionResult UpdateBrand(int brandId, [FromForm] BrandUpdate brandUpdate)
        {
            try
            {
                _productRepository.UpdateBrand(brandId, brandUpdate);
                return Ok("Brand updated successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        // Other actions...
    }

}
