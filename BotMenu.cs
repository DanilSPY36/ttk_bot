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
            new KeyboardButton("base products"),
            new KeyboardButton("shippers"),
            new KeyboardButton("TTK"),
        }
    };

            startMenu = new ReplyKeyboardMarkup(buttonRows)
            {
                ResizeKeyboard = true
            };

            return startMenu;
        }

        public async Task<InlineKeyboardMarkup> ShippersMenuAsync()
        {
            List<List<InlineKeyboardButton>> buttonRows = new List<List<InlineKeyboardButton>>();


            foreach (var shipper in await shippersRep.Get())
            {
                var button = new InlineKeyboardButton("shippers menu")
                {
                    Text = $"{shipper.Id} || {shipper.Name}",
                    CallbackData = $"shipper||{shipper.Id}"
                };
                buttonRows.Add(new List<InlineKeyboardButton> { button });
            }

            shippersMenu = new InlineKeyboardMarkup(buttonRows);

            return shippersMenu;
        }

        public async Task<InlineKeyboardMarkup> ItemsShipperMenuAsync(int shipperId)
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

            itemsShipperMenu = new InlineKeyboardMarkup(buttonRows);

            return itemsShipperMenu;
        }

        public async Task<InlineKeyboardMarkup> CategoryDrinksMenuAsync()
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

            return drinkCategoryMenu;
        }

        public async Task<InlineKeyboardMarkup> DrinksMenuAsync(int drinkCategoryId)
        {
            volumesDims = await volumesRep.Get();
            var buttonRows = new List<List<InlineKeyboardButton>>();

            var drinksRepit = new List<string>();

            foreach (var drink in await TtkRep.Get())
            {
                if (drink.CategoryId == drinkCategoryId && !drinksRepit.Contains(drink.Name))
                {
                    drinksRepit.Add(drink.Name);
                    
                    var button = new InlineKeyboardButton("list ttk menu")
                    {
                        Text = $"{drink.Name}",
                        CallbackData = $"drinkList||{drink.Id}"
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
            ttkDrinkMenu = new InlineKeyboardMarkup(buttonRows);
            return ttkDrinkMenu;
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


        public async Task<InlineKeyboardMarkup> VolumesDrinksMenuAsync(int drinkName)
        {
            var buttonRows = new List<List<InlineKeyboardButton>>();

            var res = from drink in drinksByMultipleVolumes
                      join volume in volumesDims on drink.VolumeId equals volume.Id
                      select new
                      {
                          DrinkName = drink.Name,
                          VolumeName = volume.Volume,
                      };


            foreach (var drink in await TtkRep.Get())
            {
                var button = new InlineKeyboardButton("list ttk menu")
                {
                    Text = $"{drink.Name}  ",
                    CallbackData = $"drinkName||{drink.Name}"
                };
            }
            

            return volumeDrinkMenu;
        }
    }
}
