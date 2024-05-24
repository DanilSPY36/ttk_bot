using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class KBJUttkRepository
    {
        private readonly TgBotContext _context;

        public string? name { get; set; }
        public int id { get; set; }
        public int? ttk_id { get; set; }
        public int? volume_id { get; set; }
        public List<string>? variety { get; set; }
        public List<KbjuTtk> kbjuTtks { get; set; }


        public KBJUttkRepository(string name, int volume_id, int? ttk_id) 
        {
            this.name = name;
            this.volume_id = volume_id;
            this.ttk_id = ttk_id;
            variety = new List<string>();
        }
        public KBJUttkRepository(TgBotContext context)
        {
            _context = context;
            kbjuTtks = _context.KbjuTtks.AsNoTracking().ToList();
        }

        public async Task<List<KbjuTtk>> Get()
        {
            return _context.KbjuTtks.AsNoTracking().ToList();
        }
        public string ToString(int id)
        {
            var matchedItem = _context.KbjuTtks.FirstOrDefault(i => i.Id == id);

            if (matchedItem != null)
            {
                return $"{matchedItem.Name}\n\n" +
                       $"Кофеин: \n" +
                       $"{matchedItem.Caffeine}\n\n" +
                       $"Каллории: \n" +
                       $"{matchedItem.Calories}\n\n" +
                       $"Белки: " +
                       $"{matchedItem.Proteins}\n\n" +
                       $"Жиры: " +
                       $"{matchedItem.Fats} \n\n" +
                       $"Угливоды: " +
                       $"{matchedItem.Carbohydrates}";
                //+$"Описание: \n{matchedItem.Description}";
            }
            else
            {
                return "drink = null";
            }
        }


        

        public List<string> GetVariaty(int id, int volume)
        {
            var temp = kbjuTtks.FindAll(i => i.Id == id && i.VolumeId == volume);
            var list = new List<string>();
            foreach (var item in temp)
            {
                list.Add(item.Variety);
            }

            foreach (var item in list)
            {
                Console.WriteLine(item + "\n");
            }
            return list;

        }
        
    }
}
