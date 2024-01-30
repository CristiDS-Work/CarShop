using CarShop.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarShop.Data
{
    public class ShopContext : DbContext
    {
        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Car> Cars { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=SSD_1;Initial Catalog=CarShop;Integrated Security=True;TrustServerCertificate=True");
        }
    }
}
