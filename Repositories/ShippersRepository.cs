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
        private readonly TgBotContext _context;
        public ShippersRepository(TgBotContext context) 
        {
            _context = context;
        }
        public async Task<List<Shipper>> Get()
        {
            return _context.Shippers.AsNoTracking().ToList();
        }
    }
}
