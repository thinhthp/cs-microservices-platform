using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductService.Data.DbFirst;
using ProductService.Entities.DbFirst;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/models")]
    public class ModelsController : ControllerBase
    {
        private readonly ProductDbFirstContext _db;

        public ModelsController(ProductDbFirstContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<model>>> GetAll()
        {
            var items = await _db.models
                .AsNoTracking()
                .ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<model>> GetById(long id)
        {
            var item = await _db.models
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<model>> Create([FromBody] model input)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            input.id = 0;
            _db.models.Add(input);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = input.id }, input);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromBody] model input)
        {
            if (id != input.id)
                return BadRequest(new { message = "ID mismatch." });

            var exists = await _db.models.AnyAsync(x => x.id == id);
            if (!exists) return NotFound();

            _db.Entry(input).State = EntityState.Modified;
            _db.Entry(input).Property(e => e.id).IsModified = false;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _db.models.AnyAsync(x => x.id == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _db.models.FirstOrDefaultAsync(x => x.id == id);
            if (entity == null) return NotFound();

            _db.models.Remove(entity);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
