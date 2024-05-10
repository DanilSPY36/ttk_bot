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





        public BotMenu()
        {
            _context = new TgBotContext();
            itemsRep = new ItemsRepository(_context);
            shippersRep = new ShippersRepository(_context);
            TtkRep = new ttkRepository(_context);
            drinkCategoriesRep = new TtkCategoriesRepository(_context);
            volumesRep = new VolumesRepository(_context);
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

        public async Task<List<List<InlineKeyboardButton>>> ItemsShipperMenuAsync(int shipperId, long indexMenu)
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
                var button = new InlineKeyboardButton($"{item.Category}")
                {
                    Text = item.Category,
                    CallbackData = $"category||{item.Id}"
                };
                buttonRows.Add(new List<InlineKeyboardButton> { button });
            }

            drinkCategoryMenu = new InlineKeyboardMarkup(buttonRows);

            return buttonRows;
        }

        public async Task<List<List<InlineKeyboardButton>>> DrinksMenuAsync(int drinkCategoryId)
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
                        CallbackData = $"drinkByMultipleVolumes||{drink.Name}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            buttonRows.Add(
            new List<InlineKeyboardButton> { new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"Назад",
                        CallbackData = $"drinkList||back"
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

        public async Task<InlineKeyboardMarkup> VolumesDrinksMenuAsync(string drinkName)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();

            List<string> volumes = new List<string>() {"0.2","0.3","0.4", " " };

            foreach (var drink in  drinksByMultipleVolumes)
            {
                if(drinkName == drink.Name)
                {
                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name} {volumes[drink.VolumeId - 1]} ",
                        CallbackData = $"drinkByOneVolume||{drink.Id}"
                    };
                    buttonRows.Add(new List<InlineKeyboardButton> { button });
                }
            }
            volumeDrinkMenu = new InlineKeyboardMarkup(buttonRows);

            return volumeDrinkMenu;
        }
    }
}
