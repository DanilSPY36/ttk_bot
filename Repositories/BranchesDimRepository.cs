using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class BranchesDimRepository
    {
        private readonly TgBotDbContext _context;

        public BranchesDimRepository(TgBotDbContext context)
        {
            _context = context;
        }
    }
}