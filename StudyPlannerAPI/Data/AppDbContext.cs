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
            // StudyPlan -> StudyTopics (Cascade Delete)
            modelBuilder.Entity<StudyTopic>()
                .HasOne(st => st.StudyPlan)
                .WithMany(sp => sp.StudyTopics)
                .HasForeignKey(st => st.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete topics when the study plan is deleted

            // StudyTopic -> StudySessions (Cascade Delete)
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.StudyTopic)
                .WithMany()
                .HasForeignKey(ss => ss.TopicId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete sessions when the topic is deleted

            // StudyPlan -> StudySessions (Restrict)
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.StudyPlan)
                .WithMany(sp => sp.StudySessions)
                .HasForeignKey(ss => ss.StudyPlanId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent direct cascade delete to avoid multiple paths

            // StudySession -> User (Restrict)
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.StudySessions)
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of users when sessions are deleted

            // StudyPlan -> User (Cascade Delete)
            modelBuilder.Entity<StudyPlan>()
                .HasOne(sp => sp.User)
                .WithMany()
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete study plans when the user is deleted

            // StudyPlanMembers -> StudyPlan (Cascade Delete)
            modelBuilder.Entity<StudyPlanMembers>()
                .HasOne(m => m.StudyPlan)
                .WithMany()
                .HasForeignKey(m => m.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete members when the study plan is deleted

            // StudyPlanMembers -> User (NoAction)
            modelBuilder.Entity<StudyPlanMembers>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.NoAction); // NoAction to avoid cascading issues when deleting users

            // StudyMaterial -> StudyTopic (Cascade Delete)
            modelBuilder.Entity<StudyMaterial>()
                .HasOne(sm => sm.StudyTopic)
                .WithMany(st => st.StudyMaterials)
                .HasForeignKey(sm => sm.StudyTopicId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete study materials when the topic is deleted
        }

    }
}
