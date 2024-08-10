using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using ttk_bot;
using ttk_bot.Models;
using ttk_bot.Repositories;
using ttk_bot.SearchLogic;
using StackExchange.Redis;
using Newtonsoft.Json;

class Program
{
    private static List<Update>? updateList;
    private static DateTime _lastCheckTime = DateTime.MinValue;
    private static readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(2);
    //redis
    private static IConnectionMultiplexer? _redis;
    private static IDatabase? _dbRedis;
    private static string? userStateControllersJson;

    private static ITelegramBotClient _botClient = null!;
    private static ReceiverOptions _receiverOptions = null!;
    private static TgBotDbContext? _context;
    private static BotMenu _botMenu = null!;

    // тест логики меню
    private static List<UserState>? userStateControllers;
    private static UserState? userStateController;
    private static UsersRepository? usersRep;
    private static Dictionary<KBJUttkRepository, List<string>>? choseKBJUDrink;
    public static event EventHandler? ProcessExit;
    //статистика
    private static OperationRepository operationRep;

    //ожидание сообщения разработчика об обновлении
    private static bool _waitingForUpdateMessage = false;

    // ожидание сообщения от юзера с конкретным id
    private static List<SearcherUser> SearcherUserList;
    static async Task Main()
    {
        updateList = new List<Update>();
        ProcessExit +=  OnProcessExit;
        _redis = ConnectionMultiplexer.Connect("45.141.79.78:6379");
        _dbRedis = _redis.GetDatabase();

        // test key 7451248242:AAEL-I9cbNrF6u2k5ELQ47SP-jFH3as5-jg
        // release key 7472801395:AAFtc8Um1ZdOmm6iCjuKQhqOby_GdwIwZ9M
        _botClient = new TelegramBotClient("7472801395:AAFtc8Um1ZdOmm6iCjuKQhqOby_GdwIwZ9M");
        _botMenu = new BotMenu();
        usersRep = new UsersRepository(_context = new TgBotDbContext());
        choseKBJUDrink = new Dictionary<KBJUttkRepository, List<string>>();
        var itemsRep = new ItemsRepository(_context = new TgBotDbContext());


        
        // Проверка, что ключ "userStateControllers" существует в Redis
        if (_dbRedis.KeyExists("userStateControllersTest"))
        {
            // Получение JSON-строки из Redis
            string userStateControllersJson = _dbRedis.StringGet("userStateControllersTest");

            // Десериализация JSON-строки в список
            userStateControllers = JsonConvert.DeserializeObject<List<UserState>>(userStateControllersJson);
        }
        else
        {
            // Инициализация пустого списка, если ключ не найден
            userStateControllers = new List<UserState>();
        }

        operationRep = new OperationRepository(_context = new TgBotDbContext());
        SearcherUserList = new List<SearcherUser>();

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

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };
        
        await Task.Delay(Timeout.Infinite, cts.Token);
        
    }
    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        updateList.Add(update);
        //if (DateTime.Now - _lastCheckTime >= _checkInterval)
        //{
            // Обновляем время последней проверки
            _lastCheckTime = DateTime.Now;

            // Проверяем и удаляем устаревшие обновления
            //await DeleteOldMessagesAsync(botClient, cancellationToken);
        //}


        switch (update.Type)
            {
                case UpdateType.Message:
                    {
                    var message = update.Message;
                        var user = message.From;
                        var chatInfo = message.Chat;
                    

                    if (await usersRep.accessCheck(user.Username, user.FirstName, user.LastName, chatInfo.Id, user.Id))
                        {
                        // отправка текста сообщения отсальным пользователям если это сообщение написал я или давид  
                        await UpdateMessageFromDeveloper(message);
                        // отправка запроса боту на поиск items, ttk
                        await UpdateSearchMessageFromUsers(update);

                        Console.WriteLine($"{user.Username} написал сообщение: {message.Text} || messageId = {message.MessageId}\n");
                            switch (message.Text)
                            {
                            case "/start":
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Алоха 🌴, я твой личный ттк-бот, читай микрогайд: \n\n" +
                                        "Зерно - ты сможешь найти всю информацию о моносортах и блендах\n\n" +
                                        "Поставщики - вся выпечка, кбжу, составы, сроки,  условия хранения и аллергены\n\n" +
                                        "ТТК - все технические карты основного меню, так же у каждой карточки напитка есть КБЖУ\n\n" +
                                        "Если есть вопросы, предложения или нашли что-то неладное, обращайтесь к нему @DanilSPY\n\n" +
                                        "Vibe use and high waves🏄‍♂️🏄‍♂️🏄‍♂️", replyMarkup: _botMenu.StartMenu(),protectContent: true);
                                    Console.WriteLine($"{user.Username} Send: {message.Text}");
                                    break;
                            case "/UpdateBot465890927":
                                {
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Напиши текст с обновлением:\n" +
                                        "Алоха 🌴. С вами на волнах обновленный бот (1.3.3)\n\n" +
                                        "Список изменений:\n" +
                                        "1)\n" +
                                        "2)\n" +
                                        "3)\n" +
                                        "4)\n" +
                                        "Если обнаружили ошибки пишите ему @DanilSPY\n" +
                                        "Vibe use and high waves🏄‍♂️🏄‍♂️🏄‍♂️", replyMarkup: _botMenu.StartMenu(), protectContent: false);
                                    _waitingForUpdateMessage = true;
                                    break;
                                }
                            case "/GetChangedData465890927":
                                {
                                    // Принудительное обнаружение изменений
                                    _botMenu.UpdateDataInDbContext();
                                    break;
                                }
                            case "/searchttk": // поиск  ttk
                                {
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Напиши название напитка", protectContent: true);
                                    SearcherUser searcherUser = new SearcherUser();
                                    searcherUser.idSearchBranch = 1;
                                    searcherUser.id = chatInfo.Id;
                                    searcherUser.tgName = user.Username;
                                    searcherUser.isSearch = true;
                                    SearcherUserList.Add(searcherUser);
                                    break;
                                }
                            case "/searchproduct": // поиск items
                                {
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Напиши название продукта", protectContent: true);
                                    SearcherUser searcherUser = new SearcherUser();
                                    searcherUser.idSearchBranch = 2;
                                    searcherUser.id = chatInfo.Id;
                                    searcherUser.tgName = user.Username;
                                    searcherUser.isSearch = true;
                                    SearcherUserList.Add(searcherUser);
                                    break;
                                }
                            case "/searchbean": // поиск singleOrigin
                                {
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Напиши название зерна (пока что желательно на английском)", protectContent: true);
                                    SearcherUser searcherUser = new SearcherUser();
                                    searcherUser.idSearchBranch = 3;
                                    searcherUser.id = chatInfo.Id;
                                    searcherUser.tgName = user.Username;
                                    searcherUser.isSearch = true;
                                    SearcherUserList.Add(searcherUser);
                                    break;
                                }
                            case "/faq": // частозадоваемые вопросы
                                {
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Faq в разработке.", protectContent: true);
                                    break;
                                }
                            case "/redisTest465890927": // сохранение данных в redis 
                                {
                                    string userStateControllersJson = JsonConvert.SerializeObject(userStateControllers);

                                    _dbRedis.StringSet("userStateControllersTest", userStateControllersJson);

                                    Console.WriteLine("Cache data saved to Redis and local cache cleared.");
                                    break;
                                }
                            case "/redisClearCash465890927":
                                {
                                    if (_dbRedis.KeyExists("userStateControllersTest"))
                                    {
                                        // Удаление ключа "userStateControllers" и связанных данных из Redis
                                        _dbRedis.KeyDelete("userStateControllersTest");
                                    }
                                    Console.WriteLine("Cache data deleted from Redis.");
                                    break;
                                }
                            case "/exceptioin":
                                {
                                    var exc = new List<int>();

                                    int x = exc[10];

                                    break;
                                }

                                default:
                                    break;
                            }
                            await BaseMenu(chatInfo.Id, message.Text, message);
                            return;
                        }
                        else
                        {
                            
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет доступа\n\nНапишите @DanilSPY с какого вы спота с просьбой выдать пропуск", protectContent: true);

                            // выдача доступа в нашем приватном чате разрабов. в телеге.
                            var accessNewUser = _context.Users.FirstOrDefault(x => x.ChatId == message.Chat.Id);
                            await _botClient.SendTextMessageAsync(chatId: -4224330568, text:$"Выдать доступ пользователю: @{accessNewUser.Name} ?", replyMarkup: new InlineKeyboardMarkup(await _botMenu.ReturnAccess(accessNewUser)));
                            return;
                        }
                    }
                case UpdateType.CallbackQuery:
                    {
                    var user = update.CallbackQuery.From;
                    var chatInfo = update.CallbackQuery;
                        if (await usersRep.accessCheck(user.Username, user.FirstName, user.LastName, chatInfo.From.Id, user.Id))
                        {
                        try 
                        { 
                            await CallBackQueryMenu(botClient, update);
                        }
                        catch
                        {

                        }
                        }
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

                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.ShippersMenuAsync()), "Список поставщиков:", message.Date.AddHours(3));
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            case "ТТК":
                {
                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.CategoryDrinksMenuAsync()), "ТТК на напитки:", message.Date.AddHours(3));
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            case "Зерно":
                {
                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.SingleOriginType()), "Выбери тип зерa: ", message.Date.AddHours(3));
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            default:
                return;   
        }
        userStateControllers.Add(userStateController);

        //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
    }
    private static async Task CallBackQueryMenu(ITelegramBotClient botClient, Update update)
    {
        var message = update.Message;
        var callbackQuery = update.CallbackQuery;
        var chatInfo = callbackQuery.Message.Chat;
        var user = callbackQuery.From;
        Console.WriteLine($"Date: {callbackQuery.Message.Date} || {user.Username} ({user.Id}) нажал на кнопку: {callbackQuery.Data}");
        
        var chat = callbackQuery.Message.Chat;
        var choseMenuButton = callbackQuery.Data.Split("||");
        int choseIdMenuButton = int.Parse(choseMenuButton[1]);
        int isSearchMenuButton = int.Parse(choseMenuButton[2]);
        int userIdFromDb;
        if (choseMenuButton.Count() > 3 )
        {
            userIdFromDb = int.Parse(choseMenuButton[3]);
        }
        else
        {
            userIdFromDb = 0;
        }

        switch (choseMenuButton[0])
        {
            case "Back":
                {
                    var stateTmp = userStateControllers.LastOrDefault(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId && x.userMenuIndex == choseIdMenuButton);
                    if(stateTmp!= null)
                    {
                        userStateControllers.Remove(stateTmp);
                        stateTmp = userStateControllers.LastOrDefault(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId);

                        await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{stateTmp.TextMessage}",
                                    replyMarkup: stateTmp.menuInlineBtns
                                );
                    }
                    else
                    {
                        try
                        {
                            await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                        }
                        catch (Exception ex)
                        {
                            await _botClient.SendTextMessageAsync(chatId: chat.Id, text: $"К сожалению сообщению больше двух дней. Я не могу его удалить");
                        }
                    }
                    break;
                }
            case "BackPhoto":
                {
                    try
                    {
                        await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                        await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId -1);

                    }
                    catch (Exception ex)
                    {
                        await _botClient.SendTextMessageAsync(chatId: chat.Id, text: $"К сожалению сообщению больше двух дней. Я не могу его удалить");
                    }
                    var stateTmp = userStateControllers.LastOrDefault(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId - 2 && x.userMenuIndex == choseIdMenuButton);
                    if (stateTmp != null)
                    {
                        userStateControllers.Remove(stateTmp);
                    }
                        break;
                }

            case "shipper":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                    userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.ItemsShipperMenuAsync(choseIdMenuButton, 1)), "Продукты поставщика");
                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);
                    //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "shipInfo":
                {
                    userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{1}||{-1}"
                                                }
                                            }),
                         $"{_botMenu.shippersRep.ToString(choseIdMenuButton)}");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: userStateController.TextMessage,
                                        replyMarkup: userStateController.menuInlineBtns);
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "item":
                {
                    try
                    {
                        userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}||{-1}"
                                                }
                                            }),
                         $"{_botMenu.itemsRep.ToString(choseIdMenuButton)}");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        

                        // photo
                        var photoPath = _context.Items.FirstOrDefault(p => p.Id == choseIdMenuButton);
                        Console.WriteLine($"Photo Path: {photoPath.PhotoPath}");
                        using (FileStream stream = new FileStream(photoPath.PhotoPath, FileMode.Open, FileAccess.Read))
                        {
                            InputFileStream inputOnlineFile = new InputFileStream(stream);
                            await botClient.SendPhotoAsync(chatId: chat.Id, photo: inputOnlineFile, protectContent: true);
                            await botClient.SendTextMessageAsync(chatId: chat.Id, text: $"{userStateController.TextMessage}", replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                        }
                    }
                    catch (Exception ex)
                    {
                        userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}||{-1}"
                                                }
                                            }),
                         $"{_botMenu.itemsRep.ToString(choseIdMenuButton)}");
                        await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                        await _botClient.EditMessageTextAsync(
                                            chatId: chat.Id,
                                            messageId: callbackQuery.Message.MessageId,
                                            text: userStateController.TextMessage,
                                            replyMarkup: userStateController.menuInlineBtns);
                    }
                    finally
                    {
                        userStateControllers.Add(userStateController);
                        await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 2, choseIdMenuButton, isSearchMenuButton);
                    }
                    
                    
                    

                    break;
                }

            case "categoryDrinks":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    var categoryDrinksName = await _botMenu.drinkCategoriesRep.Get(choseIdMenuButton);
                    //var tempUser = _context.Users.FirstOrDefault(x => x.ChatId == callbackQuery.Message.Chat.Id);
                    var tempUsers = await usersRep.Get();
                    var tempUser = tempUsers.FirstOrDefault(x => x.ChatId == callbackQuery.Message.Chat.Id);
                    userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.DrinksMenuAsync(choseIdMenuButton, 1, tempUser.SpotId)), $"Категория - {categoryDrinksName}");

                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);
                    //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "drinkByOneVolume":
                {


                    userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                    new InlineKeyboardMarkup(new[]
                                        {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}||{-1}"
                                                },
                                                new InlineKeyboardButton("KBJU")
                                                {
                                                    Text = "КБЖУ",
                                                    CallbackData = $"KBJUPhoto||{choseIdMenuButton}||{-1}"
                                                }
                                        }),
                     $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");

                    // фото 
                    var photoPath = _context.DrinksTtks.FirstOrDefault(p => p.Id == choseIdMenuButton);
                    Console.WriteLine($"Photo Path: {photoPath.PhotoPath}");
                    if (photoPath.PhotoPath != null)
                    {
                        using (FileStream stream = new FileStream(photoPath.PhotoPath, FileMode.Open, FileAccess.Read))
                        {
                            InputFileStream inputOnlineFile = new InputFileStream(stream);
                            await botClient.SendPhotoAsync(chatId: chat.Id, photo: inputOnlineFile, protectContent: true);
                            await botClient.SendTextMessageAsync(chatId: chat.Id, text: $"{userStateController.TextMessage}", replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                        }
                    }
                    else
                    {
                        if (choseIdMenuButton == 10025) // swell-set
                        {
                            userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                                            new InlineKeyboardMarkup(new[]
                                           {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}||{-1}"
                                                }}), $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");
                        }
                        else
                        {
                            userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                                            new InlineKeyboardMarkup(new[]
                                           {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}||{-1}"
                                                },
                                                new InlineKeyboardButton("KBJU")
                                                {
                                                    Text = "КБЖУ",
                                                    CallbackData = $"KBJU||{choseIdMenuButton}||{-1}"
                                                }
                                           }), $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");
                        }

                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage}",
                                   replyMarkup: userStateController.menuInlineBtns);
                    }
                    userStateControllers.Add(userStateController);
                    await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 1, choseIdMenuButton, isSearchMenuButton);



                    break;
                }
            case "drinkByMultipleVolumes":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.VolumesDrinksMenuAsync(choseIdMenuButton, 2)), $"Выбери объем: ");

                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);

                    //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }

            case "singleOrigin":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    if (choseIdMenuButton == 1)
                    {
                        userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.SingleOriginMenuAsync(choseIdMenuButton, 1)), $"Зерно под фильтр");
                    }
                    else
                    {
                        userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.SingleOriginMenuAsync(choseIdMenuButton, 1)), $"Зерно под эспрессо");
                    }
                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);
                    //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);

                    break;
                }
            case "singleOriginCard":
                {
                    userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}||{-1}"
                                                }
                                            }),
                         $"{_botMenu.singleOriginRep.ToString(choseIdMenuButton)}");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: userStateController.TextMessage,
                                        replyMarkup: userStateController.menuInlineBtns);
                    //Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 3, choseIdMenuButton, isSearchMenuButton);
                    break;
                }

            
            case "KBJUPhoto": // выбор молока после нажатия на KBJU на карточке с фотографией
                {
                    var buttonRows = new List<List<InlineKeyboardButton>>();
                    
                    choseKBJUDrink = await _botMenu.DrinkKBJUVariaty(choseIdMenuButton, 3);

                    foreach (var x in choseKBJUDrink.First().Value)
                    {
                        int index = choseKBJUDrink.First().Value.IndexOf(x);
                        var button = new InlineKeyboardButton($"{x}")
                        {
                            Text = x,
                            CallbackData = $"DrinkVariationPhoto||{index + 1}||{-1}" // индекс молока крч в одном напитке 
                        };
                        buttonRows.Add(new List<InlineKeyboardButton> { button });
                    }
                    buttonRows.Add(new List<InlineKeyboardButton> {new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"BackPhoto||{2}||{-1}"
                    }});


                    var obj = choseKBJUDrink.Keys.FirstOrDefault(x => x.ttk_id == choseIdMenuButton);

                    //Console.WriteLine($"amount variation:{choseKBJUDrink.First().Value.Count()}");
                    if (choseKBJUDrink.First().Value.Count() == 0 || choseKBJUDrink.First().Value.Count() == 1) // если у напитка нет вариаций
                    {
                        
                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage}КБЖУ {obj.GetKBJU()}",
                                   replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}||{-1}"
                                                }
                                            }));
                    }
                    else
                    {

                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage}",
                                   replyMarkup: new InlineKeyboardMarkup(buttonRows));
                    }
                    //await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 4, choseIdMenuButton);
                    break;
                }
            case "KBJU": // выбор молока после нажатия на KBJU на карточке с фотографией
                {
                    var buttonRows = new List<List<InlineKeyboardButton>>();

                    choseKBJUDrink = await _botMenu.DrinkKBJUVariaty(choseIdMenuButton, 3);
                    if(choseKBJUDrink.Count == 0)
                    {
                        await _botClient.EditMessageTextAsync(
                                       chatId: chat.Id,
                                       messageId: callbackQuery.Message.MessageId,
                                       text: $"{userStateController.TextMessage} Кбжу разработчиком пока что не известен.",
                                       replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                                {
                                                    new InlineKeyboardButton("Back")
                                                    {
                                                        Text = "Назад",
                                                        CallbackData = $"Back||{2}||{-1}"
                                                    }
                                                }));
                    }
                    else 
                    {

                        foreach (var x in choseKBJUDrink.First().Value)
                        {
                            int index = choseKBJUDrink.First().Value.IndexOf(x);
                            var button = new InlineKeyboardButton($"{x}")
                            {
                                Text = x,
                                CallbackData = $"DrinkVariation||{index + 1}||{-1}" // индекс молока крч в одном напитке 
                            };
                            buttonRows.Add(new List<InlineKeyboardButton> { button });
                        }
                        buttonRows.Add(new List<InlineKeyboardButton> {new InlineKeyboardButton("Back")
                        {
                            Text = "Назад",
                            CallbackData = $"Back||{2}||{-1}"
                        }});


                        var obj = choseKBJUDrink.Keys.FirstOrDefault(x => x.ttk_id == choseIdMenuButton);

                        //Console.WriteLine($"amount variation:{choseKBJUDrink.First().Value.Count()}");
                        if (choseKBJUDrink.First().Value.Count() == 0 || choseKBJUDrink.First().Value.Count() == 1) // если у напитка нет вариаций
                        {

                            await _botClient.EditMessageTextAsync(
                                       chatId: chat.Id,
                                       messageId: callbackQuery.Message.MessageId,
                                       text: $"{userStateController.TextMessage}КБЖУ {obj.GetKBJU()}",
                                       replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                                {
                                                    new InlineKeyboardButton("Back")
                                                    {
                                                        Text = "Назад",
                                                        CallbackData = $"Back||{2}||{-1}"
                                                    }
                                                }));
                        }
                        else
                        {

                            await _botClient.EditMessageTextAsync(
                                       chatId: chat.Id,
                                       messageId: callbackQuery.Message.MessageId,
                                       text: $"{userStateController.TextMessage}",
                                       replyMarkup: new InlineKeyboardMarkup(buttonRows));
                        }


                    }
                    //await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 4, choseIdMenuButton);
                    break;
                }
            case "DrinkVariationPhoto":
                {
                    // надо достать список объектов кбжу и из него достать напиток с определенным молоком

                    //Console.WriteLine(choseKBJUDrink.First().Key.GetKBJU(choseIdMenuButton));
                    await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage} КБЖУ\n{choseKBJUDrink.First().Key.GetKBJU(choseIdMenuButton)}", // надо написать метод который будет по выбранному молоку присылать карточку конкретного напитка 
                                   replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}||{-1}"
                                                }
                                            }));
                    break;
                }
            case "DrinkVariation":
                {
                    // надо достать список объектов кбжу и из него достать напиток с определенным молоком

                    //Console.WriteLine(choseKBJUDrink.First().Key.GetKBJU(choseIdMenuButton));
                    await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage} КБЖУ\n{choseKBJUDrink.First().Key.GetKBJU(choseIdMenuButton)}", // надо написать метод который будет по выбранному молоку присылать карточку конкретного напитка 
                                   replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}||{-1}"
                                                }
                                            }));
                    break;
                }

            case "Delete":
                {
                    try
                    {
                        await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                        await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId - 1);

                    }
                    catch (Exception)
                    {
                        await _botClient.SendTextMessageAsync(chatId: chat.Id, text: $"К сожалению сообщению больше двух дней. Я не могу его удалить");
                    }
                    var tempMessage = userStateControllers.Find(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId);
                    if(tempMessage != null)
                    {
                        userStateControllers.Remove(tempMessage);
                    }

                    break;
                }
            case "SpotName":
                {
                    var userTemp = _context.Users.FirstOrDefault(x => x.Id == userIdFromDb);
                    if (userTemp != null)
                    {
                        userTemp.SpotId = choseIdMenuButton;
                        try 
                        {
                            _context.Users.Update(userTemp);
                            _context.SaveChanges();
                        }
                        catch (Exception) { }
                        await _botClient.EditMessageTextAsync(
                                      chatId: -4224330568,
                                      messageId: callbackQuery.Message.MessageId,
                                      text: $"@{userTemp.Name} Доступ = {userTemp.IsAccess} Spot = {userTemp.SpotId}");
                    }
                    break;
                }
            case "Access_true":
                {
                    
                    var userTemp = _context.Users.FirstOrDefault(x => x.Id == choseIdMenuButton);
                    
                    if(userTemp!= null)
                    {
                        userTemp.IsAccess = true;
                        try
                        {
                            _context.Users.Update(userTemp);
                            _context.SaveChanges();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ex}");
                            throw;
                        }
                        await _botClient.EditMessageTextAsync(
                                       chatId: -4224330568,
                                       messageId: callbackQuery.Message.MessageId,
                                       text: $"@{userTemp.Name} Доступ = {userTemp.IsAccess}\n с какого спота:", 
                                       replyMarkup: new InlineKeyboardMarkup( await _botMenu.ReturnUserSpot(userTemp.Id)));

                        await _botClient.SendTextMessageAsync(userTemp.ChatId, "Hello again surfer 🤙🏼\n" +
                            "press /start for pay respect ✌🏼", protectContent: true);
                        
                        

                        //               text: $"@{userTemp.Name} Доступ = {userTemp.IsAccess}");
                        
                    }
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
    private static async Task UpdateMessageFromDeveloper(Message message)
    {
        var user = message.From;
        if (_waitingForUpdateMessage && (user.Id == 465890927 || user.Id == 841506985))
        {
            await usersRep.BotUpdateInfoMessage(_botClient, message.Text);
            _waitingForUpdateMessage = false;
        }
    }
    private static async Task UpdateSearchMessageFromUsers(Update update) 
    {
        var message = update.Message;
        //var callbackQuery = update.CallbackQuery;
        var chatInfo = message.Chat;
        var user = message.From;

        var tempUserSearcher = SearcherUserList.FirstOrDefault(x => x.id == user.Id);
        
        if (tempUserSearcher!= null && tempUserSearcher.isSearch)
        {
            tempUserSearcher.searchMessage = message.Text;
            userStateController = new UserState(chatInfo.Id, 1, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.SearcheResult(tempUserSearcher)), $"Search result:");
            if(userStateController.menuInlineBtns.InlineKeyboard.Count() == 1)
            {
                await _botClient.SendTextMessageAsync(
                                    chatId: chatInfo.Id,
                                    text: $"Sorry, I didn't find anything for this request...",
                                    replyMarkup: userStateController.menuInlineBtns);
            }
            else
            {
                userStateControllers.Add(userStateController);
                await _botClient.SendTextMessageAsync(
                                        chatId: chatInfo.Id,
                                        text: $"{userStateController.TextMessage}",
                                        replyMarkup: userStateController.menuInlineBtns);

            }
            SearcherUserList.Remove(tempUserSearcher);
        }
    }
    private static async Task DeleteOldMessagesAsync(ITelegramBotClient botClient, CancellationToken cancellationToken)
    {

        try
        {
            // Получаем список обновлений
            //var updates = await botClient.GetUpdatesAsync(offset: 0, limit: 100, timeout: 10, cancellationToken: cancellationToken);

            // Фильтруем устаревшие обновления (например, старше 2 минут)
            var oldUpdates = updateList?.Where(u => u.Message.Date != null && u.Message.Date < DateTime.Now.AddMinutes(-1));

            // Удаляем устаревшие обновления
            foreach (var oldUpdate in oldUpdates)
            {
                await botClient.DeleteMessageAsync(oldUpdate.CallbackQuery.From.Id, oldUpdate.CallbackQuery.Message.MessageId, cancellationToken);
            }

            Console.WriteLine($"Deleted {oldUpdates.Count()} old updates.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking and deleting old updates: {ex.Message}");
        }


    }
    private static void OnProcessExit(object sender, EventArgs e)
    {
        // Сохранение данных кэша в Redis
        string userStateControllersJson = JsonConvert.SerializeObject(userStateControllers);

        _dbRedis.StringSet("userStateControllersTest", userStateControllersJson);
        

        Console.WriteLine("Cache data saved to Redis and local cache cleared.");
    }
}