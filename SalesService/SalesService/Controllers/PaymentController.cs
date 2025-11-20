using Microsoft.AspNetCore.Mvc;
using SalesService.BLL.DTOs.Payments;
using SalesService.BLL.Services.Payment;
using System.ComponentModel.DataAnnotations;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // POST: api/payments
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest request)
        {
            if (request is null) return BadRequest("Body required.");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            try
            {
                var payment = await _paymentService.CreateAsync(request.OrderId, request.Amount, request.Method);
                var dto = Map(payment);
                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        // GET: api/payments/{id}
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var payment = await _paymentService.GetByIdAsync(id);
            if (payment is null) return NotFound();
            return Ok(Map(payment));
        }

        // GET: api/payments/order/{orderId}
        [HttpGet("order/{orderId:guid}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByOrder(Guid orderId)
        {
            var payment = await _paymentService.GetByOrderAsync(orderId);
            if (payment is null) return NotFound();
            return Ok(Map(payment));
        }

        private static PaymentDto Map(SalesService.Entities.Entities.Payment p) =>
            new(p.Id, p.OrderId, p.Amount, p.Method, p.PaidAt);

        // GET: api/payments
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _paymentService.GetAllAsync();
            return Ok(payments.Select(Map));
        }
    }
}