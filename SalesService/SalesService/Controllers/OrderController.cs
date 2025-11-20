using Microsoft.AspNetCore.Mvc;
using SalesService.BLL.DTOs.Orders;
using SalesService.BLL.Services.Order;
using SalesService.Entities.Entities;
using SalesService.Entities.Enums;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrderService orderService, ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // POST: api/orders
        [HttpPost]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            if (request is null) return BadRequest("Body required.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var items = request.Items.Select(i => new OrderItemInput(i.VariantId, i.Quantity, i.UnitPrice));
                var order = await _orderService.CreateOrderAsync(request.CustomerId, request.DealerId, items);
                var dto = Map(order);
                return CreatedAtRoute(nameof(GetById), new { id = order.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/orders/{id}?includeDetails=true
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeDetails = false)
        {
            var order = await _orderService.GetByIdAsync(id, includeDetails);
            if (order is null) return NotFound();
            return Ok(Map(order, includeDetails));
        }

        // GET: api/orders/customer/{customerId}?includeDetails=true
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByCustomer(Guid customerId, [FromQuery] bool includeDetails = false)
        {
            var orders = await _orderService.GetByCustomerAsync(customerId, includeDetails);
            if (orders.Count == 0) return NotFound();
            return Ok(orders.Select(o => Map(o, includeDetails)));
        }

        // PATCH: api/orders/{id}/status
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (request is null) return BadRequest("Body required.");
            if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
                return BadRequest("Invalid status.");

            try
            {
                await _orderService.UpdateStatusAsync(id, request.Status);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Mapping
        private static OrderDto Map(Orders o, bool includeDetails = false) =>
            new(
                o.Id,
                o.CustomerId,
                o.DealerId,
                o.Status,
                o.TotalAmount,
                o.CreatedAt,
                includeDetails
                    ? o.Items.Select(i => new OrderItemDto(i.Id, i.VariantId, i.Quantity, i.UnitPrice)).ToList()
                    : null
            );
    }
}