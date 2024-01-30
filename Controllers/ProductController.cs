using FullCartApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullCartApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        private readonly ItemRepository _itemRepository;
        private readonly OrderRepository _orderRepository;

        public ProductController(ProductRepository productRepository, ItemRepository itemRepository, OrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _itemRepository = itemRepository;
            _orderRepository = orderRepository;
        }

        [HttpGet("GetProductsByCategory/{categoryId}")]
        public IActionResult GetProductsByCategory(int categoryId)
        {
            try
            {
                var products = _productRepository.GetProductsByCategory(categoryId);

                if (products == null || !products.Any())
                {
                    return NotFound($"No products found for category with ID {categoryId}");
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("CreateOrder/{customerId}")]
        public IActionResult CreateOrder(int customerId)
        {
            try
            {
                _orderRepository.CreateOrder(customerId);

                return Ok("Order created successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("GetOrdersByCustomer/{customerId}")]
        public IActionResult GetOrdersByCustomer(int customerId)
        {
            try
            {
                var orders = _orderRepository.GetOrdersByCustomerAndStatus(customerId,DTO.OrderStatus.Processing.ToString());

                if (orders == null || !orders.Any())
                {
                    return NotFound($"No orders found for customer with ID {customerId}");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("SearchItems")]
        public IActionResult SearchItems(string searchTerm, bool sortByLowToHigh = false)
        {
            try
            {
                var items = _itemRepository.SearchItems(searchTerm, sortByLowToHigh);

                if (items == null || !items.Any())
                {
                    return NotFound("No items found.");
                }

                return Ok(items);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("CancelOrder/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                _orderRepository.CancelOrder(orderId);

                return Ok("Order canceled successfully.");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }
    }

}
