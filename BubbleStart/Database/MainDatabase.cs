﻿using BubbleStart.Model;
using EntityFramework.DynamicFilters;
using MySql.Data.Entity;
using System;
using System.Data.Entity;

namespace BubbleStart.Database
{
    // Code-Based Configuration and Dependency resolution
    //[DbConfigurationType(typeof(MySqlEFConfiguration))]
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class MainDatabase : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Apointment> Apointments { get; set; }
        public DbSet<Program> Programs { get; set; }
        public DbSet<ShowUp> ShowUps { get; set; }
        public DbSet<Payment> Payments { get; set; }

        public MainDatabase() : base("BubbleDatabase")
        {
            Configuration.ValidateOnSaveEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        public DateTime Limit { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           
            modelBuilder.Properties<string>()
            .Configure(s => s.HasMaxLength(200).HasColumnType("varchar"));
            modelBuilder.Properties().Where(x => x.PropertyType == typeof(bool))
             .Configure(x => x.HasColumnType("bit"));
            //modelBuilder.Entity<ShowUp>()
            //        .Property(p => p.Arrive)
            //        .HasColumnType("bit");
            //modelBuilder.Entity<Illness>()
            //   .HasRequired(s => s.Customer)
            //   .WithRequiredPrincipal(ad => ad.Illness);
            base.OnModelCreating(modelBuilder);
        }
    }
}