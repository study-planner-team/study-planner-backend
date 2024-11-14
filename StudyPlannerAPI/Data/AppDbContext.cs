using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<StudyPlan> StudyPlans { get; set; }
        public virtual DbSet<StudyTopic> StudyTopics { get; set; }
        public virtual DbSet<StudySession> StudySessions { get; set; }
        public virtual DbSet<StudyPlanMembers> StudyPlanMembers { get; set; }
        public virtual DbSet<StudyMaterial> StudyMaterials { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // StudySession configuration
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.StudyPlan)
                .WithMany(sp => sp.StudySessions)
                .HasForeignKey(ss => ss.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete for sessions when the Study Plan is deleted

            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.StudySessions)
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Restrict delete to avoid deleting users when sessions are deleted

            // StudyPlan configuration
            modelBuilder.Entity<StudyPlan>()
                .HasOne(sp => sp.User)
                .WithMany() // No navigation property needed
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete Study Plans when the User is deleted

            // StudyPlanMembers configuration
            modelBuilder.Entity<StudyPlanMembers>()
                .HasOne(m => m.User)
                .WithMany() // No navigation property needed
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction); // NoAction to avoid cascading issues when deleting users

            modelBuilder.Entity<StudyPlanMembers>()
                .HasOne(m => m.StudyPlan)
                .WithMany() // No navigation property needed
                .HasForeignKey(m => m.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete StudyPlanMembers when the Study Plan is deleted

            // StudyTopic configuration
            modelBuilder.Entity<StudyTopic>()
                .HasOne(st => st.StudyPlan)
                .WithMany(sp => sp.StudyTopics)
                .HasForeignKey(st => st.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete Study Topics when the Study Plan is deleted

            // StudyMaterial configuration
            modelBuilder.Entity<StudyMaterial>()
                .HasOne(sm => sm.StudyTopic)
                .WithMany(st => st.StudyMaterials)
                .HasForeignKey(sm => sm.StudyTopicId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete Study Materials when the Study Topic is deleted

            // StudySession configuration
            /*
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.StudyTopic)
                .WithMany() // Add a navigation property if needed
                .HasForeignKey(ss => ss.StudyTopicId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete Study Sessions when the Study Topic is deleted
            */
        }

    }
}
