using Microsoft.EntityFrameworkCore;
using NerdySoftTest.Models;

namespace NerdySoftTest.AppDbContext
{
    public class AnnouncementDbContext : DbContext
    {
        public AnnouncementDbContext(DbContextOptions<AnnouncementDbContext> options)
                : base(options)
        {
        }

        public DbSet<Announcement> Announcements { get; set; }
    }
}
