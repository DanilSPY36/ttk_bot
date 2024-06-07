using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class ItemsRepository
    {
        private readonly TgBotDbContext _context;
       
        public ItemsRepository(TgBotDbContext context)
        {
            _context = context;
        }

        public async Task<List<Item>> Get()
        {
            return _context.Items.AsNoTracking().ToList();
        }

        public string ToString(int id)
        {
            var matchedItem = _context.Items.FirstOrDefault(i => i.Id == id);
            if (matchedItem != null)
            {
                string output = "";
                output += $"{matchedItem.Name}\n\n";

                if (!string.IsNullOrEmpty(matchedItem.Composition))
                {
                    output += $"Состав: {matchedItem.Composition}\n\n";
                }

                if (matchedItem.Weight != null)
                {
                    output += $"Вес 1 порции: {matchedItem.Weight}\n";
                }

                if (matchedItem.Proteins != null)
                {
                    output += $"Белки, гр: {matchedItem.Proteins}\n";
                }

                if (matchedItem.Fats != null)
                {
                    output += $"Жиры, гр: {matchedItem.Fats}\n";
                }

                if (matchedItem.Carbohydrates != null)
                {
                    output += $"Углеводы, гр: {matchedItem.Carbohydrates}\n";
                }

                if (matchedItem.Calories != null)
                {
                    output += $"Калорийность, ккал: {matchedItem.Calories}\n";
                }

                if (matchedItem.Energy != null)
                {
                    output += $"КлДж: {matchedItem.Energy}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.ExpirationDate))
                {
                    output += $"Сроки хранения: {matchedItem.ExpirationDate}\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.StorageCond))
                {
                    output += $"Условия хранения: {matchedItem.StorageCond}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Description))
                {
                    output += $"Описание: {matchedItem.Description}\n";
                }

                return output;
            }
            else
            {
                return "null item";
            }

        }
    }
}
