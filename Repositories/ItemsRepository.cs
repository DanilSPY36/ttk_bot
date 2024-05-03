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
        private readonly TgBotContext _context;

        public ItemsRepository(TgBotContext context)
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
                return $"{matchedItem.Name}\n" +
                                $"\n========================================\n" +
                                $"Описание: {matchedItem.Description}" +
                                $"\n========================================\n" +
                                $"Состав: {matchedItem.Composition}" +
                                $"\n========================================\n" +
                                $"Вес 1 порции: {matchedItem.Weight}" +
                                $"\n========================================\n" +
                                $"Белки, гр: {matchedItem.Proteins}" +
                                $"\n========================================\n" +
                                $"Жиры, гр: {matchedItem.Fats}" +
                                $"\n========================================\n" +
                                $"Углеводы, гр: {matchedItem.Carbohydrates}" +
                                $"\n========================================\n" +
                                $"Калорийность, ккал: {matchedItem.Calories}" +
                                $"\n========================================\n" +
                                $"КлДж: {matchedItem.Energy}" +
                                $"\n========================================\n" +
                                $"Сроки хранения: {matchedItem.StorageCond}" +
                                $"\n========================================\n" +
                                $"Условия хранения: {matchedItem.ExpirationDate}" +
                                $"\n========================================\n";
            }
            else
            {
                return "null item";
            }
        }
    }
}
