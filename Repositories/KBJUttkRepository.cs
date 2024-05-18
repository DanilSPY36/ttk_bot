using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class KBJUttkRepository
    {
        private readonly TgBotContext _context;

        public KBJUttkRepository(TgBotContext context)
        {
            _context = context;
        }

        public async Task<List<KbjuTtk>> Get()
        {
            return _context.KbjuTtks.AsNoTracking().ToList();
        }
    }
}
