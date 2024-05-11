using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class SingleOriginRepository
    {
        private readonly TgBotContext _context;

        public SingleOriginRepository(TgBotContext _context)
        {
            this._context = _context;
        }

        public async Task<List<SingleOrigin>> Get()
        {
            return _context.SingleOrigins.AsNoTracking().ToList();
        }

        public string ToString(int id)
        {
            var matchedItem = _context.SingleOrigins.FirstOrDefault(i => i.Id == id);

            if (matchedItem != null)
            {
                return $"{matchedItem.Name}\n\n" +
                       $"Регион: {matchedItem.Region}\n\n" +
                       $"Обработка: {matchedItem.Process}\n\n" +
                       $"Букет: {matchedItem.Flavor} \n\n" +
                       $"Кислотность: {matchedItem.Acidity}\n\n" +
                       $"Дескрипторы: {matchedItem.Taste}\n\n" +
                       $"Послевкусие: {matchedItem.Aftertaste}\n\n" +
                       $"Тело: {matchedItem.Body}\n\n" +
                       $"Q grade: {matchedItem.Q}\n\n" +
                       $"Разновидности: {matchedItem.Variety}\n\n" +
                       $"Описание: \n{matchedItem.Description}";
            }
            else
            {
                return "drink = null";
            }
        }
    }
}
