using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catwork.Api.Controllers
{
    [Produces("application/json")]
    [Route("api/CatworkUsers/{userId}/Image")]
    public class CatworkUserImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IServiceBusService _serviceBusService;

        public CatworkUserImagesController(ApplicationDbContext context, 
            IBlobStorageService blobStorageService,
            IServiceBusService serviceBusService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
            _serviceBusService = serviceBusService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] string userId, [FromBody] ProfileImageDto dto)
        {
            // Angefragter User wird aus der DB geladen.
            var catworkUser = await _context.CatworkUsers.SingleOrDefaultAsync(m => m.UserId == userId);

            if (catworkUser == null)
            {
                return NotFound();
            }

            // Base64 encodiertes Bild wird in ByteArray umgewandelt
            byte[] imageByteArray = Convert.FromBase64String(dto.Base64);

            // Memorystream wird aus ByteArray erstellt und an den BlobStorageService übergeben.
            // Die URL wird in der DB gespeichert.
            using (var ms = new MemoryStream(imageByteArray))
            {
                catworkUser.ProfilePicture =
                    await _blobStorageService.WriteProfileImageToBlob(ms, userId);
            }

            _context.Update(catworkUser);
            await _context.SaveChangesAsync();

            // Die neue URL wird in den Servicebus geschrieben.
            await _serviceBusService.SendMessageAsync(catworkUser.ProfilePicture);

            return Ok();
        }
    }
}