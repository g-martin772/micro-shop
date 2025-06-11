using Microsoft.AspNetCore.Mvc;
using OrderService.Api.Data;
using OrderService.Api.Model;
using System.Net;

namespace OrderService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderServiceContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public OrderController(
            OrderServiceContext context,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Erstellt eine neue Bestellung für den angegebenen Benutzer mit den übergebenen Produkt-IDs.
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder(
            [FromQuery] string userId,
            [FromBody] List<int> productIds)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("userId is required.");
            if (productIds == null || !productIds.Any())
                return BadRequest("At least one productId must be provided.");

            // 2) Validate each product exists
            var client = _httpClientFactory.CreateClient("ProductService");
            foreach (var pid in productIds)
            {
                var resp = await client.GetAsync($"api/Product/{pid}");
                if (resp.StatusCode == HttpStatusCode.NotFound)
                {
                    return BadRequest($"Product with ID {pid} was not found.");
                }
                if (!resp.IsSuccessStatusCode)
                {
                    return StatusCode((int)resp.StatusCode, 
                        $"Error contacting ProductService for ID {pid}.");
                }
            }

            // 3) All products exist — proceed to create order
            var order = new Order
            {
                UserId = userId,
                Status = "Created"
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var orderHasProducts = productIds
                .Select(pid => new OrderHasProduct
                {
                    OrderId = order.Id,
                    ProductId = pid
                })
                .ToList();

            _context.OrderHasProducts.AddRange(orderHasProducts);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOrderStatus),
                new { orderId = order.Id },
                new { orderId = order.Id });
        }

        [HttpGet("{orderId}/status")]
        public IActionResult GetOrderStatus(int orderId)
        {
            var order = _context.Orders.Find(orderId);
            if (order == null)
                return NotFound($"Bestellung mit ID {orderId} wurde nicht gefunden.");

            return Ok(new { orderId = order.Id, status = order.Status });
        }
    }
}