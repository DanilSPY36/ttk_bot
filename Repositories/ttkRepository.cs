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
        private readonly TgBotFirstContext _context;
        public ttkRepository(TgBotFirstContext context) 
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
                return $"{matchedItem.Name}\n\n" +
                       $"Ингридиенты: \n" +
                       $"{matchedItem.Ingridients}\n\n" +
                       $"Как готовить: \n" +
                       $"{matchedItem.HowToCook}\n\n" +
                       $"Описание: {matchedItem.Description}\n\n" +
                       $"Вес 1 порции: " +
                       $"{matchedItem.Weight}\n\n" +
                       $"Добавки: \n" +
                       $"{matchedItem.Additives} \n\n";
                       //+$"Описание: \n{matchedItem.Description}";
            }
            else
            {
                return "drink = null";
            }
        }
        public async Task<string>  GetPhoto(int id)
        {
            var matchedItem = _context.DrinksTtks.FirstOrDefault(i => i.Id == id);

            return $"{matchedItem.PhotoPath}";
        }
    }
}
