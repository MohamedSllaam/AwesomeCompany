﻿using AwesomeCompany.Entities;
using Microsoft.EntityFrameworkCore;

namespace AwesomeCompany
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Company> Companies {  get; set; }
        public DbSet<Employee> Employees {  get; set; }
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Company>(builder =>
            {
                builder.ToTable("Companies");

                builder
                      .HasMany(company => company.Employees)
                      .WithOne()
                      .HasForeignKey(e => e.CompanyId)
                      .IsRequired();

                builder.HasData(new Company
                {
                    Id= 1,
                    Name= "Awesome"
                });
            });
            modelBuilder.Entity<Employee>(builder => {
                builder.ToTable("Employees");
                var employees = Enumerable
                .Range(1, 1000)
                .Select(id => new Employee
                {
                    Id = id,
                    Name = $"Employee {id}",
                    Salary = 100.0m,
                    CompanyId = 1
                }).ToList();
                builder.HasData(employees);
            });
        }
    }
}
