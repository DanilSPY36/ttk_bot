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

        public KBJUttkRepository() { }
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
        public string GetKBJU()
        {
            var matchedItem = kbjuTtks.FirstOrDefault(x => x.Name == this.name && x.VolumeId == volume_id);
            List<string> volumes = new List<string>() { "0.2", "0.3", "0.4", " " };
            if (matchedItem != null)
            {
                return $"{matchedItem.Name} {volumes[matchedItem.VolumeId - 1]}\n" +
                       $"Кофеин: " +
                       $"{matchedItem.Caffeine}\n" +
                       $"Калории: " +
                       $"{matchedItem.Calories}\n" +
                       $"Белки: " +
                       $"{matchedItem.Proteins}\n" +
                       $"Жиры: " +
                       $"{matchedItem.Fats} \n" +
                       $"Углеводы: " +
                       $"{matchedItem.Carbohydrates}\n" +
                       $"кДж: {matchedItem.Energy} ";
                //+$"Описание: \n{matchedItem.Description}";
            }
            else
            {
                return "drink = null";
            }
        }


        

        public List<string> GetVariaty(int id, int volume)
        {
            var temp = kbjuTtks.FindAll(i => i.TtkId == id && i.VolumeId == volume);
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
