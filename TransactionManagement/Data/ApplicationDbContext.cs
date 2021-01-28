using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TransactionManagement.Models;

namespace TransactionManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
