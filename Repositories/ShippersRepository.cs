using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class ShippersRepository
    {
        private readonly TgBotDbContext _context;
        public ShippersRepository(TgBotDbContext context) 
        {
            _context = context;
        }
        public async Task<List<Shipper>> Get()
        {
            return _context.Shippers.AsNoTracking().ToList();
        }

        public string ToString(int id)
        {
            var matchedItem = _context.Shippers.FirstOrDefault(i => i.Id == id);
            return $"{matchedItem.Name}\n\n" +
                   $"Телефон: {matchedItem.PhoneNumber}\n\n" +
                   $"Email: {matchedItem.Email}\n\n" +
                   $"ИНН: {matchedItem.Inn}";
        }
    }
}
