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
        private readonly TgBotFirstContext _context;

        public OperationRepository(TgBotFirstContext context)
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
                operation.Timestamp = (long)(time - new DateTime(1970, 1, 1)).TotalSeconds;

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