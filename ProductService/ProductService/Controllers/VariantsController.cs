using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.DbFirst;
using ProductService.Entities.DbFirst;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/variants")]
    public class VariantsController : ControllerBase
    {
        private readonly ProductDbFirstContext _db;

        public VariantsController(ProductDbFirstContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<variant>>> GetAll()
        {
            var items = await _db.variants
                .AsNoTracking()
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<variant>> GetById(long id)
        {
            var item = await _db.variants
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<variant>> Create([FromBody] variant input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            input.id = 0;
            _db.variants.Add(input);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.id }, input);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] variant input)
        {
            if (id != input.id)
                return BadRequest(new { message = "ID mismatch." });

            var exists = await _db.variants.AnyAsync(x => x.id == id);
            if (!exists) return NotFound();

            _db.Entry(input).State = EntityState.Modified;
            _db.Entry(input).Property(e => e.id).IsModified = false;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.variants.AnyAsync(x => x.id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.variants.FirstOrDefaultAsync(x => x.id == id);
            if (entity == null) return NotFound();

            _db.variants.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
