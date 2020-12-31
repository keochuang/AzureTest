using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace AzureProgrammerTest
{

    public class OrderContext : DbContext
    {
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(ConfigurationManager.AppSettings["ConnectionString"]);
        }*/

        public DbSet<Order> Orders { get; set; }
    }
}
