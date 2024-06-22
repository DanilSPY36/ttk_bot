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
        private readonly TgBotDbContext _context;

        public string? name { get; set; }
        public int id { get; set; }
        public int? ttk_id { get; set; }
        public int volume_id { get; set; }
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
        public KBJUttkRepository(TgBotDbContext context)
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
                return $"{matchedItem.Name} {volumes[(int)matchedItem.VolumeId - 1]} {matchedItem.Variety}\n" +
                       $"Кофеин, гр: " +
                       $"{matchedItem.Caffeine:F1}\n" +
                       $"Калории, гр: " +
                       $"{matchedItem.Calories:F1}\n" +
                       $"Белки, гр: " +
                       $"{matchedItem.Proteins:F1}\n" +
                       $"Жиры, гр: " +
                       $"{matchedItem.Fats:F1} \n" +
                       $"Углеводы, гр: " +
                       $"{matchedItem.Carbohydrates:F1}\n" +
                       $"кДж: {matchedItem.Energy:F1} ";
                
            }
            else
            {
                return "drink = null";
            }
        }
        public string GetKBJU(int VariationId)
        {
            var matchedItem = kbjuTtks.FirstOrDefault(x => x.Name == this.name && x.VolumeId == volume_id && x.Variety == variety[VariationId - 1]);
            List<string> volumes = new List<string>() { "0.2", "0.3", "0.4", " " };
            if (matchedItem != null)
            {
                return $"{matchedItem.Name} {volumes[(int)matchedItem.VolumeId - 1]}\n" +
                       $"Молоко {matchedItem.Variety:F1}\n" +
                       $"Кофеин, гр: " +
                       $"{matchedItem.Caffeine:F1}\n" +
                       $"Калории, гр: " +
                       $"{matchedItem.Calories:F1}\n" +
                       $"Белки, гр: " +
                       $"{matchedItem.Proteins:F1}\n" +
                       $"Жиры, гр: " +
                       $"{matchedItem.Fats:F1} \n" +
                       $"Углеводы, гр: " +
                       $"{matchedItem.Carbohydrates:F1}\n" +
                       $"кДж: {matchedItem.Energy:F1} ";
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
