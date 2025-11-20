using Microsoft.AspNetCore.Mvc;
using SalesService.BLL.DTOs.Customers;
using SalesService.BLL.Services.Customers;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        // POST: api/customers
        [HttpPost]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
        {
            if (request is null) return BadRequest("Body required.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var customer = await _customerService.CreateAsync(request.FullName, request.Phone, request.Email);
                var dto = Map(customer);
                return CreatedAtRoute(nameof(GetById), new { id = customer.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/customers/{id}?includeOrders=true
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeOrders = false)
        {
            var customer = await _customerService.GetByIdAsync(id, includeOrders);
            if (customer is null) return NotFound();
            return Ok(Map(customer, includeOrders));
        }

        // GET: api/customers/search?term=abc
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<CustomerDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search([FromQuery] string? term)
        {
            var items = await _customerService.SearchAsync(term);
            return Ok(items.Select(c => Map(c, includeOrders: false)));
        }

        private static CustomerDto Map(SalesService.Entities.Entities.Customer c, bool includeOrders = false) =>
            new(
                c.Id,
                c.FullName,
                c.Phone,
                c.Email,
                includeOrders
                    ? c.Orders.Select(o => new CustomerOrderSummary(o.Id, o.TotalAmount, o.Status, o.CreatedAt)).ToList()
                    : null
            );
    }
}