using FuzzySharp;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.IRepos;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class ttkRepository : IRepository<DrinksTtk>
    {
        private readonly TgBotDbContext _context;
        public ttkRepository(TgBotDbContext context) 
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
                string output = "";
                output += $"{matchedItem.Name}\n\n";

                if (!string.IsNullOrEmpty(matchedItem.Ingridients))
                {
                    output += $"Ингридиенты: \n{matchedItem.Ingridients}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.HowToCook))
                {
                    output += $"Как готовить: \n{matchedItem.HowToCook}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Description))
                {
                    output += $"Описание: {matchedItem.Description}\n\n";
                }

                if (matchedItem.Weight != null)
                {
                    output += $"Вес 1 порции: {matchedItem.Weight} \n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Additives))
                {
                    output += $"Добавки: \n{matchedItem.Additives}\n\n";
                }

                return output;
            }
            else
            {
                return "drink = null";
            }
        }
        public async Task<string> GetPhoto(int id)
        {
            var matchedItem = _context.DrinksTtks.FirstOrDefault(i => i.Id == id);

            return $"{matchedItem.PhotoPath}";
        }
        public async Task<List<DrinksTtk>> GetByName(string searchTerm)
        {
            //var regex = new Regex($"\\b{Regex.Escape(searchTerm)}\\b", RegexOptions.IgnoreCase);
            var tempList = await _context.DrinksTtks.ToListAsync();
            var tempFuzzList = new List<DrinksTtk>();
            foreach (var item in tempList)
            {
                if (Fuzz.PartialRatio(item.Name, searchTerm) > 50)
                {
                    tempFuzzList.Add(item);
                }
            }

            return tempFuzzList;
        }
    }
}