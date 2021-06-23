using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EfCoreTableHintDemo
{
   class Program
   {
      static async Task Main(string[] args)
      {
         using var host = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(builder =>
                                                        {
                                                           builder.ConfigureServices((hostCtx, services) =>
                                                                                     {
                                                                                        services.AddMvc();
                                                                                        services.AddDbContext<DemoDbContext>(optionsBuilder => optionsBuilder.UseSqlServer(hostCtx.Configuration.GetConnectionString("Demo")));
                                                                                     });

                                                           builder.Configure(app =>
                                                                             {
                                                                                app.UseRouting();
                                                                                app.UseEndpoints(routeBuilder => routeBuilder.MapControllers());
                                                                             });
                                                        })
                              .Build();

         await InitDatabaseAsync(host);

         await host.RunAsync();
      }

      private static async Task InitDatabaseAsync(IHost host)
      {
         using var scope = host.Services.CreateScope();

         var dbContext = scope.ServiceProvider.GetRequiredService<DemoDbContext>();
         await dbContext.Database.EnsureCreatedAsync();

         var id = new Guid("06D6E029-320B-4347-9E47-03CCFF153652");

         if (!await dbContext.Products.AnyAsync(p => p.Id == id))
         {
            dbContext.Products.Add(new() { Id = id, LastProductAccess = DateTime.UtcNow });
            await dbContext.SaveChangesAsync();
         }
      }
   }
}
