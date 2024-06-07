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
                return $"{matchedItem.Name}\n\n" +
                                $"Состав: " +
                                $"{matchedItem.Composition}\n\n" +
                                $"Вес 1 порции: {matchedItem.Weight} \n" +
                                $"Белки, гр: {matchedItem.Proteins} \n" +
                                $"Жиры, гр: {matchedItem.Fats} \n" +
                                $"Углеводы, гр: {matchedItem.Carbohydrates}  \n" +
                                $"Калорийность, ккал: {matchedItem.Calories}  \n" +
                                $"КлДж: {matchedItem.Energy}\n\n" +
                                $"Сроки хранения: {matchedItem.ExpirationDate}  \n" +
                                $"Условия хранения: {matchedItem.StorageCond} \n\n" +
                                $"Описание: {matchedItem.Description}\n";
            }
            else
            {
                return "null item";
            }
        }
    }
}
