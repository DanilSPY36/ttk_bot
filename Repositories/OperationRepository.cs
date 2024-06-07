using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;
using Npgsql;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace ttk_bot.Repositories
{
    public class OperationRepository
    {
        private readonly TgBotDbContext _context;

        public OperationRepository(TgBotDbContext context)
        {
            _context = context;
        }

        public async Task addOperation(DateTime time, long userId, int branchId, int productId)
        {
            var userContext = _context.Users.First(x => x.TgUserId == userId);
            try
            {
                //var postgresTimestamp = date.GetDateTimeFormats(,);
                var operation = new Operation();
                operation.UserId = userContext.Id;
                operation.BranchId = branchId;
                operation.ProductId = productId;
                //time.AddHours(3);
                operation.Timestamp = (long)(time - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds + 10800;
                
                _context.Operations.Add(operation);
                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex}");
                throw;
            }
        }
    }
}