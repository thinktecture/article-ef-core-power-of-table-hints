using System;
using Microsoft.EntityFrameworkCore;

namespace EfCoreTableHintDemo
{
   public class DemoDbContext : DbContext
   {
      public DbSet<Product> Products { get; set; }

      public DemoDbContext(DbContextOptions<DemoDbContext> options)
         : base(options)
      {
      }
   }
}
