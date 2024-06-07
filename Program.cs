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


class Program
{
    private static ITelegramBotClient _botClient = null!;
    private static ReceiverOptions _receiverOptions = null!;
    private static TgBotDbContext? _context;
    private static BotMenu _botMenu = null!;

    // тест логики меню
    private static List<UserState> userStateControllers;
    private static UserState userStateController;
    private static UsersRepository usersRep;
    private static Dictionary<KBJUttkRepository, List<string>> choseKBJUDrink;

    //статистика
    private static OperationRepository operationRep;

    static async Task Main()
    {
        _botClient = new TelegramBotClient("7451248242:AAEtWuvnh-dQgiTOiZV6prGs8EiBxrt2i8A");
        _botMenu = new BotMenu();
        usersRep = new UsersRepository(_context = new TgBotDbContext());
        choseKBJUDrink = new Dictionary<KBJUttkRepository, List<string>>();
        var itemsRep = new ItemsRepository(_context = new TgBotDbContext());
        userStateControllers = new List<UserState> { };

        operationRep = new OperationRepository(_context = new TgBotDbContext());

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

            // оповещение пользователей о новой обнове. 
        //usersRep.BotUpdateInfoMessage(_botClient);


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
                        
                        
                    if (await usersRep.accessCheck(user.Username, user.FirstName, user.LastName, chatInfo.Id, user.Id, 123, 5))
                        {
                            Console.WriteLine($"{user.Username} написал сообщение: {message.Text} || messageId = {message.MessageId}\n");
                            switch (message.Text)
                            {
                                case "/start":
                                    await botClient.SendTextMessageAsync(chatInfo.Id, "Категорически Приветствую. Я твой личный ттк-бот, читай микрогайд: \n\n" +
                                        "Зерно - ты сможешь найти всю информацию о моносортах и блендах\n\n" +
                                        "Поставщики - вся выпечка, кбжу, составы, сроки,  условия хранения и аллергены\n\n" +
                                        "ТТК - все технические карты основного меню\n\n" +
                                        "КБЖУ напитки - кбжу любого напитка.\n\n" +
                                        "Если есть вопросы, предложения или нашли что-то неладное, то напишите ему @DanilSPY", replyMarkup: _botMenu.StartMenu(),protectContent: true);
                                    Console.WriteLine($"{user.Username} Send: {message.Text}");
                                    break;
                                default:
                                    break;
                            }
                            await BaseMenu(chatInfo.Id, message.Text, message);
                            return;
                        }
                        else
                        {
                            
                            _botClient.SendTextMessageAsync(message.Chat.Id, "У вас нет доступа\n\nНапишите @DanilSPY с какого вы спота с просьбой выдать пропуск", protectContent: true);
                            return;
                        }
                    }
                case UpdateType.CallbackQuery:
                    {
                    var user = update.CallbackQuery.From;
                    var chatInfo = update.CallbackQuery;
                        if (await usersRep.accessCheck(user.Username, user.FirstName, user.LastName, chatInfo.From.Id, user.Id, 123, 5))
                        {
                            CallBackQueryMenu(botClient, update);
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

                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.ShippersMenuAsync()), "Список поставщиков:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            case "ТТК":
                {
                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.CategoryDrinksMenuAsync()), "ТТК на напитки:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            case "Зерно":
                {
                    userStateController = new UserState(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.SingleOriginType()), "Выбери тип зерa: ");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns, protectContent: true);
                    break;
                }
            default:
                return;   
        }
        userStateControllers.Add(userStateController);

        Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
    }
    private static async Task CallBackQueryMenu(ITelegramBotClient botClient, Update update)
    {
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
                    var stateTmp = userStateControllers.Last(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId && x.userMenuIndex == choseIdMenuButton);

                    userStateControllers.Remove(stateTmp);
                    

                    // надо доставать не последний в списке, а последний в списке с проверкой на юзера 

                    stateTmp = userStateControllers.Last(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId);

                    await _botClient.EditMessageTextAsync(
                                chatId: chat.Id,
                                messageId: callbackQuery.Message.MessageId,
                                text: $"{stateTmp.TextMessage}",
                                replyMarkup: stateTmp.menuInlineBtns
                            );
                    //await _botClient.DeleteMessageAsync(chat.Id, messageId: message.MessageId);

                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    break;
                }
            case "BackPhoto":
                {
                    await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                    await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId -1);
                    userStateControllers.Remove(userStateControllers.Last());
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
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
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
                                                    CallbackData = $"Back||{1}"
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
                    userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}"
                                                }
                                            }),
                         $"{_botMenu.itemsRep.ToString(choseIdMenuButton)}");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: userStateController.TextMessage,
                                        replyMarkup: userStateController.menuInlineBtns);
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    Console.WriteLine($"DAte: {callbackQuery.Message.Date}");
                    await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 2, choseIdMenuButton);

                    break;
                }
            case "categoryDrinks":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                    var categoryDrinksName = await _botMenu.drinkCategoriesRep.Get(choseIdMenuButton);
                    userStateController = new UserState(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.DrinksMenuAsync(choseIdMenuButton, 1)), $"Категория - {categoryDrinksName}");

                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "drinkByOneVolume":
                {
                    try
                    {
                        ///
                        userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}"
                                                },
                                                new InlineKeyboardButton("KBJU")
                                                {
                                                    Text = "КБЖУ",
                                                    CallbackData = $"KBJUPhoto||{choseIdMenuButton}"
                                                }
                                            }),
                         $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");

                        // фото 
                        var photoPath = _context.DrinksTtks.FirstOrDefault(p => p.Id == choseIdMenuButton);
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
                        Console.WriteLine(ex.ToString());
                        if (choseIdMenuButton == 10025) // swell-set
                        {
                            userStateController = new UserState(chat.Id, 2, callbackQuery.Message.MessageId,
                                            new InlineKeyboardMarkup(new[]
                                           {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}"
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
                                                    CallbackData = $"Back||{2}"
                                                },
                                                new InlineKeyboardButton("KBJU")
                                                {
                                                    Text = "КБЖУ",
                                                    CallbackData = $"KBJU||{choseIdMenuButton}"
                                                }
                                           }), $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");
                        }
                        
                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage}",
                                   replyMarkup: userStateController.menuInlineBtns);
                        

                        throw;
                    }
                    finally
                    {
                        userStateControllers.Add(userStateController);
                        await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 1, choseIdMenuButton);
                    }
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

                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
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
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
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
                                                    CallbackData = $"Back||{2}"
                                                }
                                            }),
                         $"{_botMenu.singleOriginRep.ToString(choseIdMenuButton)}");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: userStateController.TextMessage,
                                        replyMarkup: userStateController.menuInlineBtns);
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    await operationRep.addOperation(callbackQuery.Message.Date, user.Id, 3, choseIdMenuButton);
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
                            CallbackData = $"DrinkVariationPhoto||{index + 1}" // индекс молока крч в одном напитке 
                        };
                        buttonRows.Add(new List<InlineKeyboardButton> { button });
                    }
                    buttonRows.Add(new List<InlineKeyboardButton> {new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"BackPhoto||{2}"
                    }});


                    var obj = choseKBJUDrink.Keys.FirstOrDefault(x => x.ttk_id == choseIdMenuButton);

                    Console.WriteLine($"amount variation:{choseKBJUDrink.First().Value.Count()}");
                    if (choseKBJUDrink.First().Value.Count() == 0 || choseKBJUDrink.First().Value.Count() == 1) // если у напитка нет вариаций
                    {
                        
                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage} КБЖУ {obj.GetKBJU()}",
                                   replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}"
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

                    foreach (var x in choseKBJUDrink.First().Value)
                    {
                        int index = choseKBJUDrink.First().Value.IndexOf(x);
                        var button = new InlineKeyboardButton($"{x}")
                        {
                            Text = x,
                            CallbackData = $"DrinkVariation||{index + 1}" // индекс молока крч в одном напитке 
                        };
                        buttonRows.Add(new List<InlineKeyboardButton> { button });
                    }
                    buttonRows.Add(new List<InlineKeyboardButton> {new InlineKeyboardButton("Back")
                    {
                        Text = "Назад",
                        CallbackData = $"Back||{2}"
                    }});


                    var obj = choseKBJUDrink.Keys.FirstOrDefault(x => x.ttk_id == choseIdMenuButton);

                    Console.WriteLine($"amount variation:{choseKBJUDrink.First().Value.Count()}");
                    if (choseKBJUDrink.First().Value.Count() == 0 || choseKBJUDrink.First().Value.Count() == 1) // если у напитка нет вариаций
                    {

                        await _botClient.EditMessageTextAsync(
                                   chatId: chat.Id,
                                   messageId: callbackQuery.Message.MessageId,
                                   text: $"{userStateController.TextMessage} КБЖУ {obj.GetKBJU()}",
                                   replyMarkup: userStateController.menuInlineBtns = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"Back||{2}"
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
                                                    CallbackData = $"BackPhoto||{2}"
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
                                                    CallbackData = $"Back||{2}"
                                                }
                                            }));
                    break;
                }

            case "Delete":
                {
                    await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                    var tempMessage = userStateControllers.Find(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId);
                    if(tempMessage != null)
                    {
                        userStateControllers.Remove(tempMessage);
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

    
}