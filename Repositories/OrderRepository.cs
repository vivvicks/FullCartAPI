using FullCartApi.DTO;
using FullCartApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FullCartApi.Repositories
{
    public class OrderRepository
    {
        private readonly EcommerceDbContext _context;

        public OrderRepository(EcommerceDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            return _context.Orders.ToList();
        }

        public Order GetOrderById(int orderId)
        {
            return _context.Orders.FirstOrDefault(o => o.OrderId == orderId);
        }

        public void CreateOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }

        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
            _context.SaveChanges();
        }

        public IEnumerable<OrderItem> GetOrderItemsByOrderId(int orderId)
        {
            return _context.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerId == customerId);
        }

        public void CreateOrder(int customerId)
        {
            var customer = GetCustomerById(customerId);
            var cartItems = _context.CartItems.Include(ci => ci.Item).Where(ci => ci.CustomerId == customerId).ToList();

            if (customer != null && cartItems.Any())
            {
                // Calculate total price based on items in the cart
                decimal totalPrice = cartItems.Sum(ci => ci.Quantity * ci.Item.Price);

                // Create a new order
                var newOrder = new Order
                {
                    CustomerId = customerId,
                    TotalPrice = totalPrice,
                    Status = "order created"
                };

                // Create order items based on items in the cart
                foreach (var cartItem in cartItems)
                {
                    var orderItem = new OrderItem
                    {
                        Order = newOrder,
                        ItemId = cartItem.ItemId,
                        Quantity = cartItem.Quantity
                    };

                    _context.OrderItems.Add(orderItem);
                }

                // Remove items from the cart
                _context.CartItems.RemoveRange(cartItems);

                _context.Orders.Add(newOrder);
                _context.SaveChanges();
            }
        }

        public IEnumerable<Order> GetOrdersByCustomerAndStatus(int customerId, string status)
        {
            // Parse the status string to the OrderStatus enum (assuming OrderStatus is an enum)
            if (Enum.TryParse<OrderStatus>(status, out var orderStatus))
            {
                return _context.Orders
                    .Where(o => o.CustomerId == customerId && o.Status == status)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Item)
                            .ThenInclude(item => item.Brand) // Include the Brand navigation property
                                .ThenInclude(brand => brand.Products) // Include the Product navigation property
                            .ThenInclude(item => item.Category) // Include the Category navigation property
                                .ThenInclude(category => category.Products) // Include the Product navigation property
                    .ToList();
            }
            else
            {
                // Handle the case where status is not a valid OrderStatus enum value
                throw new ArgumentException("Invalid order status.");
            }
        }

        public void CancelOrder(int orderId)
        {
            var order = _context.Orders.Find(orderId);

            if (order != null)
            {
                // Check if the order is in a cancellable state (e.g., not already canceled or delivered)
                if (order.Status != "order canceled" && order.Status != "order delivered")
                {
                    order.Status = "order canceled";
                    _context.SaveChanges();
                }
                else
                {
                    throw new InvalidOperationException("Order cannot be canceled in the current state.");
                }
            }
            else
            {
                throw new ArgumentException($"Order with ID {orderId} not found.");
            }
        }
    }

}
