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
        private readonly TgBotDbContext _context;

        public SingleOriginRepository(TgBotDbContext _context)
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
                string output = "";
                output += $"{matchedItem.Name}\n\n";

                if (!string.IsNullOrEmpty(matchedItem.Region))
                {
                    output += $"Регион: {matchedItem.Region}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Process))
                {
                    output += $"Обработка: {matchedItem.Process}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Flavor))
                {
                    output += $"Букет: {matchedItem.Flavor}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Acidity))
                {
                    output += $"Кислотность: {matchedItem.Acidity}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Taste))
                {
                    output += $"Дескрипторы: {matchedItem.Taste}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Aftertaste))
                {
                    output += $"Послевкусие: {matchedItem.Aftertaste}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Body))
                {
                    output += $"Тело: {matchedItem.Body}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Q.ToString()))
                {
                    output += $"Q grade: {matchedItem.Q}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Variety))
                {
                    output += $"Разновидности: {matchedItem.Variety}\n\n";
                }

                if (!string.IsNullOrEmpty(matchedItem.Description))
                {
                    output += $"Описание: \n{matchedItem.Description}";
                }

                return output;
            }
            else
            {
                return "drink = null";
            }

        }
    }
}
