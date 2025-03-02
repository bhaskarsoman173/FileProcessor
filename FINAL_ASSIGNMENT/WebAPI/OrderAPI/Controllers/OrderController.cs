using Microsoft.AspNetCore.Mvc;
using OrderAPI.Services;
 
namespace OrderAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("{orderNumber}")]
        public async Task<IActionResult> GetOrder(string orderNumber)
        {
            // Validate that the order number is not null or empty
            if (string.IsNullOrEmpty(orderNumber))
            {
                return BadRequest("Order number is required.");
            }

            var order = await _orderService.GetOrderAsync(orderNumber);

            if (order == null) return NotFound($"Order {orderNumber} doesn't exist");

            return Ok(order);
        }
    }
}
