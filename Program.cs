using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ttk_bot;
using ttk_bot.Models;
using ttk_bot.Repositories;



class Program
{
    private static ITelegramBotClient _botClient = null!;
    private static ReceiverOptions _receiverOptions = null!;
    private static TgBotContext? _context;
    private static BotMenu _botMenu = null!;

    static async Task Main()
    {
        _botClient = new TelegramBotClient("7190916687:AAG4L9eYwyj8bLJtXajo6uTP-k-KLksjoA");
        _botMenu = new BotMenu();
        var usersRep = new UsersRepository(_context = new TgBotContext());
        var itemsRep = new ItemsRepository(_context = new TgBotContext());


        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[]
            {
                UpdateType.Message,
                UpdateType.CallbackQuery,
            },

            ThrowPendingUpdates = true,
        };

        using var cts = new CancellationTokenSource();


        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cts.Token);
        var me = await _botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");
        await Task.Delay(-1);
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                {
                    var message = update.Message;
                    var user = message.From;
                    var chatInfo = message.Chat;
                    Console.WriteLine($"{user.Username} Пишет хуйню следующего характера: {message.Text}\n");

                    switch (message.Text)
                    {
                        
                        case "/start":
                            await botClient.SendTextMessageAsync(chatInfo.Id, "Твоя главная клавиатура.", replyMarkup: _botMenu.StartMenu());
                            Console.WriteLine($"{user.Username} Send: {message.Text}");
                            break;
                        case "base products":
                            //await botClient.SendTextMessageAsync(chatInfo.Id, "Тут все продукты батончики, бутылочные напитки и выпечка", replyMarkup: _botMenu.BaseListMenu());
                            Console.WriteLine($"{user.Username} Send: {message.Text}");
                            break;
                        case "TTK":
                            var drinkscategoryMenu = await _botMenu.CategoryDrinksMenuAsync();
                            await botClient.SendTextMessageAsync(chatInfo.Id, "Тут все ттк, все то что ты должен знать, как Отче наш", replyMarkup: drinkscategoryMenu);
                            Console.WriteLine($"{user.Username} Send: {message.Text}");
                            break;
                        case "shippers":
                            var shippersMenu = await _botMenu.ShippersMenuAsync();
                            await botClient.SendTextMessageAsync(chatInfo.Id, "Тут все твои поставщики", replyMarkup: shippersMenu);
                            Console.WriteLine($"{user.Username} Send: {message.Text}");
                            break;
                        default:
                            break;
                    }
                    return;
                }
            case UpdateType.CallbackQuery:
                {
                    var message = update.Message;
                    var callbackQuery = update.CallbackQuery;
                    var chatInfo = callbackQuery.Message.Chat;
                    var user = callbackQuery.From;
                    Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");
                    var chat = callbackQuery.Message.Chat;
                    if (callbackQuery.Data.Contains("category"))
                    {
                        var choseCategoryId = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseCategoryId[0]} || {choseCategoryId[1]}");
                        var choseCategory = await _botMenu.DrinksMenuAsync(int.Parse(choseCategoryId[1]));
                        await botClient.SendTextMessageAsync(chat.Id, "список :", replyMarkup: choseCategory);
                    }
                    if (callbackQuery.Data.Contains("shipper"))
                    {
                        var choseShipperId = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseShipperId[0]} || {choseShipperId[1]}");
                        var itemsMenu = await _botMenu.ItemsShipperMenuAsync(int.Parse(choseShipperId[1]));
                        await botClient.SendTextMessageAsync(chat.Id, "список :", replyMarkup: itemsMenu);
                    }
                    if (callbackQuery.Data.Contains("shipInfo"))
                    {
                        var choseShipperId = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseShipperId[0]} || {choseShipperId[1]}");
                        var itemsMenu = await _botMenu.ItemsShipperMenuAsync(int.Parse(choseShipperId[1]));
                        await botClient.SendTextMessageAsync(chat.Id, _botMenu.shippersRep.ToString(int.Parse(choseShipperId[1])));
                    }
                    if (callbackQuery.Data.Contains("item"))
                    {
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        var choseShipperId = callbackQuery.Data.Split("||");
                        Console.WriteLine($"{choseShipperId[0]} || {choseShipperId[1]}");
                        //var matchedItem = _botMenu.[int.Parse(choseShipperId[1]) - 1];
                        await botClient.SendTextMessageAsync(chat.Id, _botMenu.itemsRep.ToString(int.Parse(choseShipperId[1])));
                    }
                    if (callbackQuery.Data.Contains("drinkList"))
                    {
                        var choseCategoryId = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseCategoryId[0]} || {choseCategoryId[1]}");
                        await botClient.SendTextMessageAsync(chat.Id, _botMenu.TtkRep.ToString(int.Parse(choseCategoryId[1])));
                    }
                    if (callbackQuery.Data.Contains("drinkByOneVolume"))
                    {
                        var choseCategoryId = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseCategoryId[0]} || {choseCategoryId[1]}");
                        var photoPath = await _botMenu.TtkRep.GetPhoto(int.Parse(choseCategoryId[1]));
                        //await botClient.SendTextMessageAsync(chat.Id, _botMenu.TtkRep.ToString(int.Parse(choseCategoryId[1])));
                        using (var stram = System.IO.File.OpenRead(photoPath))
                        {
                            await botClient.SendPhotoAsync(chat.Id, new InputFileStream(stram),caption: _botMenu.TtkRep.ToString(int.Parse(choseCategoryId[1])));
                        }

                    }
                    if (callbackQuery.Data.Contains("drinkByMultipleVolumes"))
                    {
                        var choseDrinkName = callbackQuery.Data.Split("||");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        Console.WriteLine($"{choseDrinkName[0]} || {choseDrinkName[1]}");
                        var DrinkName = await _botMenu.VolumesDrinksMenuAsync(choseDrinkName[1]);
                        await botClient.SendTextMessageAsync(chat.Id, $"Выбери объем {choseDrinkName[1]}: ", replyMarkup: DrinkName) ;
                    }

                }
                return;
        }
    }
    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

    /*public IConfiguration Configuration { get; }
    public void ConfigureServices(IServiceCollection services)
    {
        //var config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();

        services.AddSingleton<IConfiguration>(Configuration);

        services.AddDbContext<TgBotContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
    }*/
}
