using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StriveUp.Infrastructure.Identity;
using StriveUp.Infrastructure.Models;

namespace StriveUp.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<ActivityLike> ActivityLikes { get; set; }
        public DbSet<ActivityComment> ActivityComments { get; set; }
        public DbSet<Medal> Medals { get; set; }
        public DbSet<MedalEarned> MedalsEarned { get; set; }
        public DbSet<GeoPoint> GeoPoints { get; set; }
        public DbSet<ActivityHr> ActivityHrs { get; set; }
        public DbSet<ActivitySpeed> ActivitySpeeds { get; set; }
        public DbSet<ActivityElevation> ActivityElevations { get; set; }
        public DbSet<ActivityConfig> ActivityConfig { get; set; }
        public DbSet<UserFollower> UserFollowers { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<SynchroProvider> SynchroProviders { get; set; }
        public DbSet<UserSynchro> UserSynchros { get; set; }
        public DbSet<BestEffort> BestEfforts { get; set; }
        public DbSet<SegmentConfig> SegmentConfigs { get; set; }
        public DbSet<ActivitySplit> ActivitySplits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserActivity>()
                .HasIndex(ua => ua.SynchroId)
                .IsUnique();

            modelBuilder.Entity<UserActivity>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserActivities)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

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
                .WithMany(u => u.MedalsEarned)
                .HasForeignKey(me => me.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserFollower>()
                .HasKey(x => new { x.FollowerId, x.FollowedId });

            modelBuilder.Entity<UserFollower>()
                .HasOne(x => x.Follower)
                .WithMany(x => x.Following)
                .HasForeignKey(x => x.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollower>()
                .HasOne(x => x.Followed)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.FollowedId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Actor)
                .WithMany()
                .HasForeignKey(n => n.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BestEffort>(entity =>
            {
                entity.HasKey(be => be.Id);

                entity.HasOne(be => be.User)
                    .WithMany()
                    .HasForeignKey(be => be.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(be => be.UserActivity)
                    .WithMany()
                    .HasForeignKey(be => be.UserActivityId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(be => be.SegmentConfig)
                    .WithMany()
                    .HasForeignKey(be => be.SegmentConfigId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<SegmentConfig>(entity =>
            {
                entity.HasKey(sc => sc.Id);

                entity.HasOne(sc => sc.Activity)
                    .WithMany(a => a.SegmentConfigs)
                    .HasForeignKey(sc => sc.ActivityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}