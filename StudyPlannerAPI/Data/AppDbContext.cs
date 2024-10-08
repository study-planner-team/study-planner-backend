﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

    
    }
}
