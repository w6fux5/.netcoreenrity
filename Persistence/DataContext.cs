using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions options)
            : base(options) { }

        public DbSet<Activity> Tbl_Activity { get; set; }

        public DbSet<ActivityAttendee> Tbl_ActivityAttendee { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 將 ActivityAttendee.AppUserId和 ActivityAttendee.ActivityId 合起來變成primary key
            builder.Entity<ActivityAttendee>(
                x => x.HasKey(aa => new { aa.AppUserId, aa.ActivityId })
            );

            builder
                .Entity<ActivityAttendee>()
                .HasOne(u => u.AppUser)
                .WithMany(a => a.Activity)
                .HasForeignKey(aa => aa.AppUserId);

            builder
                .Entity<ActivityAttendee>()
                .HasOne(u => u.Activity)
                .WithMany(a => a.Attendee)
                .HasForeignKey(aa => aa.ActivityId);
        }
    }
}
