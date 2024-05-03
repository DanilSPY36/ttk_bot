using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class TtkCategoriesRepository
    {
        private readonly TgBotContext _context;
        public TtkCategoriesRepository(TgBotContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriesDim>> Get()
        {
            return _context.CategoriesDims.ToList();
        }
    }
}
