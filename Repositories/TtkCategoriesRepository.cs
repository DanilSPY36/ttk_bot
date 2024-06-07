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
        private readonly TgBotDbContext _context;
        public TtkCategoriesRepository(TgBotDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoriesDim>> Get()
        {
            return _context.CategoriesDims.ToList();
        }
        public async Task<string> Get(int id)
        {
            var categoryDimsList = await _context.CategoriesDims.ToListAsync();
            return categoryDimsList.First(x=> x.Id == id).Category;
        }
    }
}
