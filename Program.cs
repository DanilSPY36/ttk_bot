using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ttk_bot;
using ttk_bot.Models;
using ttk_bot.Repositories;



class Program
{
    private static ITelegramBotClient _botClient = null!;
    private static ReceiverOptions _receiverOptions = null!;
    private static TgBotContext? _context;
    private static BotMenu _botMenu = null!;

    // тест логики меню
    private static Dictionary<long, List<InlineKeyboardMarkup>> userStates;
    private static List<UserStateController> userStateControllers;
    private static UserStateController userStateController;
    static async Task Main()
    {
        userStateControllers = new List<UserStateController> { new UserStateController() };
        userStateController = new UserStateController();
        userStates = new Dictionary<long, List<InlineKeyboardMarkup>>();


        _botClient = new TelegramBotClient("7190916687:AAG4L9eYwyj8bLJtXajo6uTP-k-MuIkRdIs");
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
                    userStates[chatInfo.Id] = new List<InlineKeyboardMarkup>();
                    switch (message.Text)
                    {
                        case "/start":
                            await botClient.SendTextMessageAsync(chatInfo.Id, "Твоя главная клавиатура.", replyMarkup: _botMenu.StartMenu());
                            Console.WriteLine($"{user.Username} Send: {message.Text}");
                            break;
                        default:
                            break;
                    }
                    await BaseMenu(chatInfo.Id, message.Text, message);
                    return;
                }
            case UpdateType.CallbackQuery:
                {
                    CallBackQueryMenu(botClient, update);
                }
                return;
        }
    }

    private static async Task BaseMenu(long chatId, string menuName, Message message)
    {
        switch (menuName)
        {
            case "Поставщики":
                {
                    userStateController = new UserStateController(chatId, 0, message.MessageId, new InlineKeyboardMarkup(await _botMenu.ShippersMenuAsync()), "Список поставщиков:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns);
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "ТТК":
                {
                    userStateController = new UserStateController(chatId, 0, message.MessageId, new InlineKeyboardMarkup(await _botMenu.CategoryDrinksMenuAsync()), "ТТК на напитки:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns);
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "Зерно":
                {
                    if (userStateControllers.Count > 0)
                    {
                        userStateControllers.Clear();
                    }
                    //userStateControllers.Add(new UserStateController(chatId, new InlineKeyboardMarkup(await _botMenu.CategoryDrinksMenuAsync()), "Список зерна:"));
                    //newMenu = new InlineKeyboardMarkup(await _botMenu.DrinksMenuAsync(1));
                    //userStates[chatId].Add(newMenu);
                    break;
                }
            default:
                return;   
        }
        Console.WriteLine($"{userStateController.userChatId} || Вложенность меню = {userStateController.userMenuIndex}");
    }
    private static async Task CallBackQueryMenu(ITelegramBotClient botClient, Update update)
    {
        InlineKeyboardMarkup newMenu;

        var message = update.Message;
        var callbackQuery = update.CallbackQuery;
        var chatInfo = callbackQuery.Message.Chat;
        var user = callbackQuery.From;
        Console.WriteLine($"{user.FirstName} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");
        var chat = callbackQuery.Message.Chat;


        var choseMenuButton = callbackQuery.Data.Split("||");
        int choseIdMenuButton = int.Parse(choseMenuButton[1]);

        switch (choseMenuButton[0])
        {
            case "Back":
                {
                    userStateController = userStateControllers.FirstOrDefault(x => x.userChatId == chat.Id &&  x.messageIndex == callbackQuery.Message.MessageId);
                    Console.WriteLine($"{userStateController.userChatId} || Вложенность меню = {userStateController.userMenuIndex}");

                    await _botClient.EditMessageTextAsync(
                                chatId: chat.Id,
                                messageId: callbackQuery.Message.MessageId,
                                text: $"{userStateControllers[choseIdMenuButton].TextMessage}",
                                replyMarkup: userStateControllers[choseIdMenuButton].menuInlineBtns
                            );
                    userStateControllers.Remove(userStateController);
                    break;
                }
            case "shipper":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    //newMenu = new InlineKeyboardMarkup(await _botMenu.ItemsShipperMenuAsync(choseIdMenuButton, userStates[chat.Id].Count-1));
                    //userStates[chat.Id].Add(newMenu);
                    userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.ItemsShipperMenuAsync(choseIdMenuButton, 1)), "Продукты поставщика");
                    userStateControllers.Add(userStateController);
                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns) ;
                    Console.WriteLine($"{userStateController.userChatId} || Вложенность меню = {userStateController.userMenuIndex}");
                    break;
                }
            case "item":
                {
                    userStateController = new UserStateController(chat.Id, 2, callbackQuery.Message.MessageId, 
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}"
                                                }
                                            }),
                        "");
                    userStateControllers.Add(userStateController);

                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: $"{_botMenu.itemsRep.ToString(choseIdMenuButton)}",
                                        replyMarkup: userStateController.menuInlineBtns);   
                    Console.WriteLine($"{userStateController.userChatId} || Вложенность меню = {userStateController.userMenuIndex}");

                    break;
                }
            default:
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