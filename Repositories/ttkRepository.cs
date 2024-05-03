using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class ttkRepository
    {
        private readonly TgBotContext _context;
        public ttkRepository(TgBotContext context) 
        {
            _context = context;
        }

        public async Task<List<DrinksTtk>> Get()
        {
            return _context.DrinksTtks.AsNoTracking().ToList();
        }
        public string ToString(int id)
        {
            var matchedItem = _context.DrinksTtks.FirstOrDefault(i => i.Id == id);

            if (matchedItem != null)
            {
                return $"{matchedItem.Name}\n" +
                       $"\n========================================\n" +
                       $"Описание: {matchedItem.Description}" +
                       $"\n========================================\n" +
                       $"Ингридиенты: {matchedItem.Ingridients}" +
                       $"\n========================================\n" +
                       $"Вес 1 порции: {matchedItem.Weight}" +
                       $"\n========================================\n" +
                       $"Как готовить: {matchedItem.HowToCook}";
            }
            else
            {
                return "drink = null";
            }
        }
    }
}
