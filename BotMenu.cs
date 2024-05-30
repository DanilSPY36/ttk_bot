using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;
using ttk_bot.Models;
using ttk_bot.Repositories;

namespace ttk_bot
{
    public class BotMenu
    {
        //списки напитков и еды для распределения по объемам
        private List<DrinksTtk>? drinksByOneVolume;
        private List<DrinksTtk>? drinksByMultipleVolumes;
        private List<DrinksTtk>? drinks;
        // меню клавиатура 
        private ReplyKeyboardMarkup? startMenu;


        private TgBotContext _context;

        public ItemsRepository? itemsRep;
        public ShippersRepository? shippersRep;

        public ttkRepository? TtkRep;
        public TtkCategoriesRepository? drinkCategoriesRep;
        public VolumesRepository? volumesRep;
        public SingleOriginRepository? singleOriginRep;

        public KBJUttkRepository KBJUttkRep;

        public List<KBJUttkRepository> sortedkBJUttkRepositories;

        public List<KbjuTtk> kbjuTtks;

        public BotMenu()
        {
            _context = new TgBotContext();

            itemsRep = new ItemsRepository(_context);
            shippersRep = new ShippersRepository(_context);

            TtkRep = new ttkRepository(_context);
            drinkCategoriesRep = new TtkCategoriesRepository(_context);
            volumesRep = new VolumesRepository(_context);
            KBJUttkRep = new KBJUttkRepository(_context);

            sortedkBJUttkRepositories = new List<KBJUttkRepository>();


            singleOriginRep = new SingleOriginRepository(_context);

            DrinkVolumeChecker();
            KBJUController();
        }

        public ReplyKeyboardMarkup StartMenu()
        {
            var buttonRows = new List<KeyboardButton[]>()
    {
        new KeyboardButton[]
        {
            new KeyboardButton("Зерно"),
            new KeyboardButton("Поставщики"),
            new KeyboardButton("ТТК"),
        }
    };
            startMenu = new ReplyKeyboardMarkup(buttonRows)
            {
                ResizeKeyboard = true
            };

            return startMenu;
        }
        private async void DrinkVolumeChecker()
        {
            drinks = new List<DrinksTtk>();
            drinks = await TtkRep.Get();
            Dictionary<string, List<int>> drinkVolumes = new Dictionary<string, List<int>>();

            drinksByOneVolume = new List<DrinksTtk>();
            drinksByMultipleVolumes = new List<DrinksTtk>();

            foreach (var drink in drinks)
            {
                if (!drinkVolumes.ContainsKey(drink.Name))
                {
                    drinkVolumes[drink.Name] = new List<int>();
                }
                drinkVolumes[drink.Name].Add(drink.VolumeId);
            }

            foreach (var drink in drinks)
            {
                if (drinkVolumes[drink.Name].Count == 1)
                {
                    drinksByOneVolume.Add(drink);
                }
                else
                {
                    drinksByMultipleVolumes.Add(drink);
                }
            }
        }

        /// <summary>
        /// крч тут чет индексы напутаны логик верна но чет не то все ровно 
        /// 
        /// </summary>
        private async void KBJUController()
        {
            kbjuTtks = await KBJUttkRep.Get();

            // Группировка напитков по названию и по объему
            var groupedByNameDrinks = kbjuTtks.GroupBy(x => new { x.Name, x.VolumeId, x.TtkId });
            
            List<string> volumes = new List<string>() { "0.2", "0,3", "0,4", " " };

            foreach (var item in groupedByNameDrinks)
            {
                Console.WriteLine($"name: {item.Key.Name}              \t\t volume: {item.Key.VolumeId}\t ttkId: {item.Key.TtkId}");
            }


            foreach (var item in groupedByNameDrinks) // работает
            {
                //Console.WriteLine($"Name: {item.Key.VolumeId}  Volums: {item.Key.Name}");
                // добавили все сгруппированные напитки по имени и объему
                var temp = new KBJUttkRepository(item.Key.Name, item.Key.VolumeId, item.Key.TtkId);
                sortedkBJUttkRepositories.Add(temp);
                foreach (var sorted in sortedkBJUttkRepositories)  
                {
                    var drinksFromDb = kbjuTtks.FindAll(x => x.Name == sorted.name && x.VolumeId == sorted.volume_id && x.TtkId == sorted.ttk_id);
                    temp.kbjuTtks = new List<KbjuTtk>(drinksFromDb);
                }
            }

            //var tempOrder = sortedkBJUttkRepositories.OrderBy(x => x.name).ThenBy(y => y.volume_id);
            //sortedkBJUttkRepositories = tempOrder.ToList();
            // надо заполнить молоко находим имя и объем 

            foreach (var item in sortedkBJUttkRepositories)
            {
                var drinksFromDb = kbjuTtks.FindAll(x => x.TtkId == item.ttk_id && x.VolumeId == item.volume_id && x.Name == item.name); // получаем список объектов с одинаковым именем и объемом

                foreach (var vareaty in drinksFromDb)
                {
                    if(item.name == vareaty.Name && item.volume_id == vareaty.VolumeId) // если имя и объем совпадает то добавляем молоко
                    {
                        item.variety.Add(vareaty.Variety);
                    }
                }
            }

            foreach (var item in sortedkBJUttkRepositories)
            {
                Console.WriteLine("||||||||||||||||||||||||||||||");
                Console.WriteLine($"Name: {item.name} || KBJUId: {item.id} ||  ttkId: {item.ttk_id} || ");
                Console.WriteLine($"Volums: {item.volume_id}");
                Console.WriteLine("Variaty: ");
                foreach (var va in item.variety)
                {
                    Console.WriteLine($"{va}");
                }
                foreach (var obj in item.kbjuTtks)
                {
                    Console.WriteLine($"{obj.Name} || {obj.Id} || {obj.TtkId} || {obj.VolumeId} || {obj.Variety}");
                }
                Console.WriteLine("\n\n ");

            }

        } // готовый sortedkBJUttkRepositories сгруппированный по имени и по объемам

        private async Task<KBJUttkRepository> VariatyChecker(string name, int? volume)
        {
            foreach (var item in sortedkBJUttkRepositories)
            {
                if (item.name == name && item.volume_id == volume)
                {
                    
                    return item;
                }
            }
            return null;
        }
        private async Task<KBJUttkRepository> VariatyChecker(int id)
        {
            foreach (var item in sortedkBJUttkRepositories)
            {
                if (item.ttk_id == id )
                {
                    return item;
                }
            }
            return null;
        }

        public async Task<List<List<InlineKeyboardButton>>> ShippersMenuAsync()
        {
            List<List<InlineKeyboardButton>> buttonRows = new List<List<InlineKeyboardButton>>();

            foreach (var shipper in await shippersRep.Get())
            {
                var buttonInfo = new InlineKeyboardButton("shippers menu Info")
                {
                    Text = $"ℹ",
                    CallbackData = $"shipInfo||{shipper.Id}"
                };
                var button = new InlineKeyboardButton("shippers menu")
                {
                    Text = $"{shipper.Name}",
                    CallbackData = $"shipper||{shipper.Id}"
                };
                buttonRows.Add(new List<InlineKeyboardButton> { buttonInfo, button });

            }
            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> ItemsShipperMenuAsync(int shipperId, int indexMenu)
        {
            List<List<InlineKeyboardButton>> buttonRows = new List<List<InlineKeyboardButton>>();

            foreach (var item in await itemsRep.Get())
            {
                //Console.WriteLine(item.ShipperId + "||" + shipperId);
                if (item.ShipperId == shipperId.ToString())
                {
                    var button = new InlineKeyboardButton("List items menu")
                    {
                        Text = $"{item.Name}",
                        CallbackData = $"item||{item.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            buttonRows.Add(new List<InlineKeyboardButton> {
                new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"Back||{indexMenu}"
                    }});


            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> CategoryDrinksMenuAsync()
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var Drinkcategories = await drinkCategoriesRep.Get();
            Drinkcategories.OrderBy(x => x.Id);
            var sortedById = Drinkcategories.OrderBy(x => x.Id);
            foreach (var item in sortedById)
            {
                if(item.Id != 17 && item.Id != 18)
                {
                    var button = new InlineKeyboardButton($"{item.Category}")
                    {
                        Text = item.Category,
                        CallbackData = $"categoryDrinks||{item.Id}"
                    };

                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> DrinksMenuAsync(int drinkCategoryId, int indexMenu)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            List<string> volumes = new List<string>() { "0.2", "0,3", "0,4", " " };
            var drinksRepit = new List<string>();

            foreach (var drink in drinksByOneVolume)
            {
                if (drink.CategoryId == drinkCategoryId && !drinksRepit.Contains(drink.Name))
                {
                    drinksRepit.Add(drink.Name);

                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name} {volumes[drink.VolumeId - 1]}",
                        CallbackData = $"drinkByOneVolume||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }


            foreach (var drink in drinksByMultipleVolumes)
            {
                if (drink.CategoryId == drinkCategoryId && !drinksRepit.Contains(drink.Name))
                {
                    drinksRepit.Add(drink.Name);

                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name}",
                        CallbackData = $"drinkByMultipleVolumes||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            buttonRows.Add(
            new List<InlineKeyboardButton> { new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"Назад",
                        CallbackData = $"Back||{indexMenu}"
                    }
                }
            );

            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> VolumesDrinksMenuAsync(int drinkId, int indexMenu)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var drinkName = drinksByMultipleVolumes.First(x => x.Id == drinkId).Name;
            List<string> volumes = new List<string>() { "0.2", "0.3", "0.4", " " };

            foreach (var drink in drinksByMultipleVolumes)
            {
                if (drinkName == drink.Name)
                {
                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name} {volumes[drink.VolumeId - 1]} ",
                        CallbackData = $"drinkByOneVolume||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            buttonRows.Add(new List<InlineKeyboardButton> {
                new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"Back||{indexMenu}"
                    }});


            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> DrinksKBJUMenuAsync(int drinkCategoryId, int indexMenu)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();
            List<string> volumes = new List<string>() { "0.2", "0,3", "0,4", " " };
            var drinksRepit = new List<string>();

            foreach (var drink in drinksByOneVolume)
            {
                if (drink.CategoryId == drinkCategoryId && !drinksRepit.Contains(drink.Name))
                {
                    drinksRepit.Add(drink.Name);

                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name} {volumes[drink.VolumeId - 1]}",
                        CallbackData = $"KBJUBByOneVolume||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }


            foreach (var drink in drinksByMultipleVolumes)
            {
                if (drink.CategoryId == drinkCategoryId && !drinksRepit.Contains(drink.Name))
                {
                    drinksRepit.Add(drink.Name);

                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name}",
                        CallbackData = $"KBJUByMultipleVolumes||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            buttonRows.Add(
            new List<InlineKeyboardButton> { new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"Назад",
                        CallbackData = $"Back||{indexMenu}"
                    }
                }
            );

            return buttonRows;
        }


        public async Task<Dictionary<KBJUttkRepository, List<string>>> DrinkKBJUVariaty(int id, int indexMenu) // когда уже нажали в ттк на напиток получили карточку напитка мы знаем его id и объем
        {
            
            var buttonRows = new List<List<InlineKeyboardButton>>();
            var drinkFromTTK = drinks.FirstOrDefault(d => d.Id == id);

            var drinkFromKBJU = await VariatyChecker(id);

            List<string> milkList = drinkFromKBJU.GetVariaty(id, drinkFromTTK.VolumeId);

            
                
            

            var ReturnDict = new Dictionary<KBJUttkRepository, List<string>>
            {
                { drinkFromKBJU, milkList }
            };

            return ReturnDict;        // тут мы должны вернуть список кнопок молока у конкретного напитка емли они есть .. если нет то просто вывести инфу о кбжу 
        }

        public async Task<List<List<InlineKeyboardButton>>> SingleOriginType() // 0 - зерно
        {
            List<List<InlineKeyboardButton>> buttonRows = new List<List<InlineKeyboardButton>>();
            buttonRows.Add(new List<InlineKeyboardButton>
            {
                new InlineKeyboardButton("Filter")
                {
                    Text = "Зерно под фильтр",
                    CallbackData = $"singleOrigin||{1}"
                },
                new InlineKeyboardButton("Espresso")
                {
                    Text = "Зерно под эспрессо",
                    CallbackData = $"singleOrigin||{2}"
                }
            });
            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> SingleOriginMenuAsync(int idType, int indexMenu) // 1 - зерно уже или под эспрессо или под фильтр
        {
            List<List<InlineKeyboardButton>> buttonRows = new List<List<InlineKeyboardButton>>();

            var listSingles = _context.SingleOrigins.ToList();

            foreach (var item in listSingles.Where(x => x.TypeId == idType))
            {

                var button = new InlineKeyboardButton("singleOriginCard Menu ")
                {
                    Text = $"{item.Name}",
                    CallbackData = $"singleOriginCard||{item.Id}"
                };
                buttonRows.Add(new List<InlineKeyboardButton> { button });
            }
            buttonRows.Add(new List<InlineKeyboardButton> {
                new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"Back||{indexMenu}"
                    }});


            return buttonRows;
        }
    }
}
