using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<StudyPlan> StudyPlans { get; set; }
        public virtual DbSet<StudyTopic> StudyTopics { get; set; }
        public virtual DbSet<StudySession> StudySessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.StudyPlan)
                .WithMany(sp => sp.StudySessions)  
                .HasForeignKey(ss => ss.StudyPlanId)
                .OnDelete(DeleteBehavior.Cascade);  

            modelBuilder.Entity<StudySession>()
                .HasOne(ss => ss.User)
                .WithMany(u => u.StudySessions)  
                .HasForeignKey(ss => ss.UserId)
                .OnDelete(DeleteBehavior.Restrict); 
        }

    }
}
