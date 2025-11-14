using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.DbFirst;
using ProductService.Entities.DbFirst;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly ProductDbFirstContext _db;

        public BrandsController(ProductDbFirstContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<brand>>> GetAll()
        {
            var items = await _db.brands
                .AsNoTracking()
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<brand>> GetById(long id)
        {
            var item = await _db.brands
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<brand>> Create([FromBody] brand input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            // Ensure ID is not preset for creation
            input.id = 0;

            _db.brands.Add(input);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = input.id }, input);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] brand input)
        {
            if (id != input.id)
                return BadRequest(new { message = "ID mismatch." });

            var exists = await _db.brands.AnyAsync(x => x.id == id);
            if (!exists) return NotFound();

            // Track only scalar properties for update
            _db.Entry(input).State = EntityState.Modified;
            _db.Entry(input).Property(e => e.id).IsModified = false;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.brands.AnyAsync(x => x.id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.brands.FirstOrDefaultAsync(x => x.id == id);
            if (entity == null) return NotFound();

            _db.brands.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
