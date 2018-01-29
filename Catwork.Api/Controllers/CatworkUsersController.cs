using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Catwork.Api.DbModels;

namespace Catwork.Api.Controllers
{
    /// <summary>
    /// Controller, der den Endpoint für die CatworkUser bereitstellt.
    /// </summary>
    [Produces("application/json")]
    [Route("api/CatworkUsers")]
    public class CatworkUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobStorageService _blobStorageService;

        public CatworkUsersController(ApplicationDbContext context, IBlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: api/CatworkUsers
        [HttpGet]
        public IEnumerable<CatworkUser> GetCatworkUsers()
        {
            return _context.CatworkUsers.Where(c => c.FirstName == "TEST"); 
        }

        // GET: api/CatworkUsers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCatworkUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var catworkUser = await _context.CatworkUsers.SingleOrDefaultAsync(m => m.UserId == id);

            if (catworkUser == null)
            {
                return NotFound();
            }

            return Ok(catworkUser);
        }

        // PUT: api/CatworkUsers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCatworkUser([FromRoute] string id, [FromBody] CatworkUser catworkUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != catworkUser.UserId)
            {
                return BadRequest();
            }

            _context.Entry(catworkUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatworkUserExists(id))
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

        // POST: api/CatworkUsers
        [HttpPost]
        public async Task<IActionResult> PostCatworkUser([FromBody] CatworkUser catworkUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CatworkUsers.Add(catworkUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCatworkUser", new { id = catworkUser.UserId }, catworkUser);
        }

        // DELETE: api/CatworkUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCatworkUser([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var catworkUser = await _context.CatworkUsers.SingleOrDefaultAsync(m => m.UserId == id);
            if (catworkUser == null)
            {
                return NotFound();
            }

            _context.CatworkUsers.Remove(catworkUser);
            await _context.SaveChangesAsync();

            return Ok(catworkUser);
        }

        private bool CatworkUserExists(string id)
        {
            return _context.CatworkUsers.Any(e => e.UserId == id);
        }
    }
}