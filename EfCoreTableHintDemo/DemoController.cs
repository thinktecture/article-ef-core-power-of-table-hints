using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thinktecture;
using Thinktecture.EntityFrameworkCore;

namespace EfCoreTableHintDemo
{
   [Route("[controller]")]
   public class DemoController : Controller
   {
      private const IsolationLevel _ISOLATION_LEVEL =
            // IsolationLevel.ReadCommitted // Violates our business requirements, but just for testing purposes
            IsolationLevel.RepeatableRead
         // IsolationLevel.Serializable
         ;

      private readonly DemoDbContext _ctx;

      public DemoController(DemoDbContext ctx)
      {
         _ctx = ctx;
      }

      [HttpPost("{id}")]
      public async Task DoAsync(Guid id)
      {
         await using var tx = await _ctx.Database.BeginTransactionAsync(_ISOLATION_LEVEL);

         var product = await _ctx.Products
                                 .WithTableHints(SqlServerTableHint.RowLock, SqlServerTableHint.UpdLock)
                                 .FirstAsync(p => p.Id == id);

         // do more or less complex stuff

         product.LastProductAccess = DateTime.UtcNow;

         await _ctx.SaveChangesAsync();

         await tx.CommitAsync();
      }
   }
}
