using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NerdySoftTest.AppDbContext;
using NerdySoftTest.Models;

namespace NerdySoftTest.Controllers
{
    [Route("api/announcement")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly AnnouncementDbContext _context;

        public AnnouncementController(AnnouncementDbContext context)
        {
            _context = context;
        }


        //endpoint where we can create new announcement
        [HttpPost("add-announcement")]
        public async Task<ActionResult<Announcement>> CreateAnnouncementAsync(AnnouncementCreateModel model)
        {
            var announcement = new Announcement
            {               
                Title = model.Title,
                Description = model.Description,
                DateAdded = DateTime.UtcNow,
            };

            _ = await _context.Announcements.AddAsync(announcement);
            _ = await _context.SaveChangesAsync();

            return announcement;
        }

        //endpoint where we can update title and description for announcement
        [HttpPut("update-announcement")]
        public async Task<ActionResult<Announcement>> UpdateAnnouncementAsync(int id, AnnouncementCreateModel updateModel)
        {
            var announcement = await _context.Announcements.FirstOrDefaultAsync(x => x.Id == id);
            if(announcement == null)
            {
                return NotFound();
            }
            announcement.Title = updateModel.Title;
            announcement.Description = updateModel.Description;

            await _context.SaveChangesAsync();

            return announcement;
        }

        //endpoint where we can retreive announcement by entered id
        [HttpGet("get-announcement")]
        public async Task<ActionResult> GetAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
            {
                return NotFound();
            }

            var titleWords = announcement.Title.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var descriptionWords = announcement.Description.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var similarAnnouncements = _context.Announcements
                .AsEnumerable()
                .Where(a => a.Id != id && (titleWords.Any(t => a.Title.Contains(t, StringComparison.OrdinalIgnoreCase))
                || descriptionWords.Any(d => a.Description.Contains(d, StringComparison.OrdinalIgnoreCase))))
                .Take(3)
                .ToList();

            var result = new
            {
                Announcement = announcement,
                SimilarAnnouncements = similarAnnouncements
            };

            return Ok(result);
        }

        //endpoint where we can retreive all announcements that we created
        [HttpGet("get-all-announcements")]
        public IEnumerable<Announcement> GetAnnouncements()
        {
            return _context.Announcements;
        }

        //endpoint where we can delete announcement by id
        [HttpDelete("delete-announcement")]
        public async Task<ActionResult> DeleteAnnouncement(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement == null)
            {
                return NotFound();
            }

            _context.Announcements.Remove(announcement);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
