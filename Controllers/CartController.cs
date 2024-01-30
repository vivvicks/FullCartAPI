using FullCartApi.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FullCartApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ProductRepository _productRepository;

        public CartController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost("AddToCart/{customerId}/{itemId}/{quantity}")]
        public IActionResult AddToCart(int customerId, int itemId, int quantity)
        {
            try
            {
                // Check if the customer and item exist
                var customer = _productRepository.GetCustomerById(customerId);
                var item = _productRepository.GetItemById(itemId);

                if (customer == null || item == null)
                {
                    return NotFound("Customer or item not found.");
                }

                // Check if the item is available in sufficient quantity
                if (item.QuantityAvailable < quantity)
                {
                    return BadRequest("Not enough quantity available.");
                }

                // Add the item to the customer's cart
                _productRepository.AddToCart(customerId, itemId, quantity);

                return Ok($"Item with ID {itemId} added to the cart for customer with ID {customerId}");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("RemoveFromCart/{customerId}/{itemId}")]
        public IActionResult RemoveFromCart(int customerId, int itemId)
        {
            try
            {
                // Check if the customer and item exist
                var customer = _productRepository.GetCustomerById(customerId);
                var item = _productRepository.GetItemById(itemId);

                if (customer == null || item == null)
                {
                    return NotFound("Customer or item not found.");
                }

                // Remove the item from the customer's cart
                _productRepository.RemoveFromCart(customerId, itemId);

                return Ok($"Item with ID {itemId} removed from the cart for customer with ID {customerId}");
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, "Internal Server Error");
            }
        }
    }

}
