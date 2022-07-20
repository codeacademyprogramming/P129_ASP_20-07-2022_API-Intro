using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P129FirstApi.Data;
using P129FirstApi.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post(Category category)
        {
            if (category.IsMain)
            {
                if (category.Image == null)
                {
                    return BadRequest("Image Is Reuired");
                }
            }
            else
            {
                if (category.ParentId == null)
                {
                    return BadRequest("ParentId Is Reuired");
                }

                if (!await _context.Categories.AnyAsync(c=>c.Id == category.ParentId && c.IsMain))
                {
                    return BadRequest("ParentId Is InCorrect");
                }
            }

            if (await _context.Categories.AnyAsync(c=>!c.IsDeleted && c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                return Conflict($"Category {category.Name} Is Already Exists");
            }

            category.Name = category.Name.Trim();
            category.CreatedAt = DateTime.UtcNow.AddHours(4);

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, category);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _context.Categories.ToListAsync());
        }

        [HttpGet]
        [Route("{id?}")]
        public async Task<IActionResult> Get(int? id)
        {
            Category category = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (category == null) return NotFound("Id Is InCorrect");

            return Ok(category);
        }

        [HttpPut]
        [Route("{id?}")]
        public async Task<IActionResult> Put(int? id,Category category)
        {
            if (id == null) return BadRequest("id Is required");

            if (category.Id != id) return BadRequest("Id Is Not Mathed By Category Object");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            if (category.IsMain)
            {
                if (category.Image == null)
                {
                    return BadRequest("Image Is Reuired");
                }
            }
            else
            {
                if (category.ParentId == null)
                {
                    return BadRequest("ParentId Is Reuired");
                }

                if (id == category.ParentId) return BadRequest("Id Is Same ParentId");

                if (!await _context.Categories.AnyAsync(c => c.Id == category.ParentId && c.IsMain))
                {
                    return BadRequest("ParentId Is InCorrect");
                }
            }

            if (await _context.Categories.AnyAsync(c => !c.IsDeleted && c.Id != id &&  c.Name.ToLower() == category.Name.Trim().ToLower()))
            {
                return Conflict($"Category {category.Name} Is Already Exists");
            }

            dbCategory.Name = category.Name.Trim();
            dbCategory.IsMain = category.IsMain;
            dbCategory.ParentId = category.ParentId;
            dbCategory.Image = category.Image;
            dbCategory.UpdatedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete]
        [Route("{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return BadRequest("id Is required");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            dbCategory.IsDeleted = true;
            dbCategory.DeletedAt = DateTime.UtcNow.AddHours(4);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpOptions]
        [Route("restore/{id?}")]
        public async Task<IActionResult> Restore(int? id)
        {
            if (id == null) return BadRequest("id Is required");

            Category dbCategory = await _context.Categories.FirstOrDefaultAsync(c => c.IsDeleted && c.Id == id);

            if (dbCategory == null) return NotFound("Id Is InCorrect");

            dbCategory.IsDeleted = false;
            dbCategory.DeletedAt = null;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
