using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<ActivityLike> ActivityLikes { get; set; }
        public DbSet<ActivityComment> ActivityComments { get; set; }
        public DbSet<Medal> Medals { get; set; }
        public DbSet<MedalEarned> MedalsEarned { get; set; }
        public DbSet<GeoPoint> GeoPoints { get; set; }
        public DbSet<ActivityHr> ActivityHrs { get; set; }
        public DbSet<ActivityConfig> ActivityConfig { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ActivityComment>()
                .HasOne(c => c.UserActivity)
                .WithMany(a => a.ActivityComments)
                .HasForeignKey(c => c.UserActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ActivityLike>()
                .HasOne(l => l.UserActivity)
                .WithMany(a => a.ActivityLikes)
                .HasForeignKey(l => l.UserActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedalEarned>()
                .HasOne(me => me.Medal)
                .WithMany()
                .HasForeignKey(me => me.MedalId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<MedalEarned>()
                .HasOne(me => me.Activity)
                .WithMany()
                .HasForeignKey(me => me.ActivityId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<MedalEarned>()
                .HasOne(me => me.User)
                .WithMany()
                .HasForeignKey(me => me.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
