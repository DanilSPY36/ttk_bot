using Microsoft.EntityFrameworkCore;
using System;
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
        private List<VolumesDim>? volumesDims;

        private ReplyKeyboardMarkup? startMenu;
        private InlineKeyboardMarkup? baseListMenu;
        private InlineKeyboardMarkup? shippersMenu;
        private InlineKeyboardMarkup? drinkCategoryMenu;
        private InlineKeyboardMarkup? itemsShipperMenu;
        private InlineKeyboardMarkup? ttkDrinkMenu;
        private InlineKeyboardMarkup? volumeDrinkMenu;

        private InlineKeyboardMarkup? registrationMenu;

        private TgBotContext _context;

        public ItemsRepository? itemsRep;
        public ShippersRepository? shippersRep;

        public ttkRepository? TtkRep;
        public TtkCategoriesRepository? drinkCategoriesRep;
        public VolumesRepository? volumesRep;
        public SingleOriginRepository? singleOriginRep;





        public BotMenu()
        {
            _context = new TgBotContext();

            itemsRep = new ItemsRepository(_context);
            shippersRep = new ShippersRepository(_context);

            TtkRep = new ttkRepository(_context);
            drinkCategoriesRep = new TtkCategoriesRepository(_context);
            volumesRep = new VolumesRepository(_context);

            singleOriginRep = new SingleOriginRepository(_context);

            DrinkVolumeChecker();
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

            foreach (var item in await drinkCategoriesRep.Get())
            {
                if(item.Category != "Добавки")
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
            volumesDims = await volumesRep.Get();
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

        private async void DrinkVolumeChecker()
        {
            var drinks = await TtkRep.Get();
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

            foreach (var item in drinksByOneVolume)
            {
                Console.WriteLine($"{item.Name} || {item.VolumeId}");
            }
            Console.WriteLine("\n|||||||||||||||||||||\n");

            foreach (var item in drinksByMultipleVolumes)
            {
                Console.WriteLine($"{item.Name} || {item.VolumeId}");
            }
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

            volumeDrinkMenu = new InlineKeyboardMarkup(buttonRows);

            return buttonRows;
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
