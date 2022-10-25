using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TvMazeTechTest.Data;
using TvMazeTechTest.Models;

namespace TvMazeTechTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowsController : ControllerBase
    {
        private readonly ShowsContext _context;

        public ShowsController(ShowsContext context)
        {
            _context = context;
        }

        // GET: api/Shows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Shows>>> GetShows(string sortOrder = "asc", int? pageSize = 25, int? page = 1)
        {
            if(sortOrder == "asc")
                return await _context.Shows.OrderBy(x => x.Premiered).Skip((page.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault()).Take(pageSize.GetValueOrDefault()).ToListAsync();             
            else
                return await _context.Shows.OrderByDescending(x => x.Premiered).Skip((page.GetValueOrDefault() - 1) * pageSize.GetValueOrDefault()).Take(pageSize.GetValueOrDefault()).ToListAsync();
        }

        // GET: api/Shows/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Shows>> GetShows(int id)
        {
            var shows = await _context.Shows.FindAsync(id);

            if (shows == null)
            {
                return NotFound();
            }

            return shows;
        }

        // PUT: api/Shows/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShows(int id, Shows shows)
        {
            if (id != shows.ID)
            {
                return BadRequest();
            }

            _context.Entry(shows).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShowsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Shows
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Shows>> PostShows(Shows shows)
        {
            _context.Shows.Add(shows);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShows", new { id = shows.ID }, shows);
        }

        // DELETE: api/Shows/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShows(int id)
        {
            var shows = await _context.Shows.FindAsync(id);
            if (shows == null)
            {
                return NotFound();
            }

            _context.Shows.Remove(shows);            
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ShowsExists(int id)
        {
            return _context.Shows.Any(e => e.ID == id);
        }
    }
}
