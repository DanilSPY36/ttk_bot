using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class UsersRepository
    {
        private readonly TgBotContext _context;

        public UsersRepository(TgBotContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Get()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
    }
}
