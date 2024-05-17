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
    private static TgBotContext? _context;
    private static BotMenu _botMenu = null!;

    // тест логики меню
    private static Dictionary<long, List<InlineKeyboardMarkup>> userStates;
    private static List<UserStateController> userStateControllers;
    private static UserStateController userStateController;
    static async Task Main()
    {
        userStateControllers = new List<UserStateController> { };



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
                    Console.WriteLine($"{user.Username} Пишет хуйню следующего характера: {message.Text} || messageId = {message.MessageId}\n");
                    switch (message.Text)
                    {
                        case "/start":
                            await botClient.SendTextMessageAsync(chatInfo.Id, "Приветствую. Я 'цикорий и доски бот' читай микрогайд: \n\n" +
                                "Зерно - ты сможешь найти всю информацию о моносортах и блендах\n\n" +
                                "Поставщики - вся выпечка, кбжу, составы, сроки,  условия хранения и аллергены\n\n" +
                                "ТТК - все технические карты основного меню\n\n" +
                                "КБЖУ напитки - кбжу любого напитка.\n\n" +
                                "Если есть вопросы, предложения или нашли что-то неладное, то напишите ему @DanilSPY", replyMarkup: _botMenu.StartMenu());
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
        UserStateController newUserMenu;
        switch (menuName)
        {
            case "Поставщики":
                {

                    userStateController = new UserStateController(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.ShippersMenuAsync()), "Список поставщиков:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns);
                    break;
                }
            case "ТТК":
                {
                    userStateController = new UserStateController(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.CategoryDrinksMenuAsync()), "ТТК на напитки:");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns);
                    break;
                }
            case "Зерно":
                {
                    userStateController = new UserStateController(chatId, 0, message.MessageId + 1, new InlineKeyboardMarkup(await _botMenu.SingleOriginType()), "Выбери тип зерНННЫЫЫААА: ");
                    await _botClient.SendTextMessageAsync(userStateController.userChatId, userStateController.TextMessage, replyMarkup: userStateController.menuInlineBtns);
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
                    var stateTmp = userStateControllers.LastOrDefault(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId && x.userMenuIndex == choseIdMenuButton);

                    userStateControllers.Remove(stateTmp);

                    userStateController = userStateControllers.Last(x => x.userChatId == chat.Id && x.messageIndex == callbackQuery.Message.MessageId && x.userMenuIndex == choseIdMenuButton - 1);

                    await _botClient.EditMessageTextAsync(
                                chatId: chat.Id,
                                messageId: callbackQuery.Message.MessageId,
                                text: $"{userStateController.TextMessage}",
                                replyMarkup: userStateController.menuInlineBtns
                            );
                    //await _botClient.DeleteMessageAsync(chat.Id, messageId: message.MessageId);
                    
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    break;
                }
            case "BackPhoto":
                {
                    await _botClient.DeleteMessageAsync(chat.Id, messageId: callbackQuery.Message.MessageId);
                    break;
                }
            case "shipper":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    
                    userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.ItemsShipperMenuAsync(choseIdMenuButton, 1)), "Продукты поставщика");
                    await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns) ;
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "shipInfo":
                {
                    userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId,
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
                         $"{_botMenu.itemsRep.ToString(choseIdMenuButton)}");
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    await _botClient.EditMessageTextAsync(
                                        chatId: chat.Id,
                                        messageId: callbackQuery.Message.MessageId,
                                        text: userStateController.TextMessage,
                                        replyMarkup: userStateController.menuInlineBtns);
                    Console.WriteLine($"ChatId = {userStateController.userChatId} || messageId = {userStateController.messageIndex} || Вложенность меню = {userStateController.userMenuIndex}");
                    userStateControllers.Add(userStateController);
                    break;
                }
            case "categoryDrinks":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);

                    var categoryDrinksName = await _botMenu.drinkCategoriesRep.Get(choseIdMenuButton);
                    userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.DrinksMenuAsync(choseIdMenuButton, 1)), $"Категория - {categoryDrinksName}");
                    
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
                        userStateController = new UserStateController(chat.Id, 2, callbackQuery.Message.MessageId,
                        new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Назад",
                                                    CallbackData = $"BackPhoto||{2}"
                                                }
                                            }),
                         $"{_botMenu.TtkRep.ToString(choseIdMenuButton)}");
                        /*await _botClient.EditMessageTextAsync(
                                    chatId: chat.Id,
                                    messageId: callbackQuery.Message.MessageId,
                                    text: $"{userStateController.TextMessage}",
                                    replyMarkup: userStateController.menuInlineBtns);
                        */

                        // фото 
                        var photoPath = _context.DrinksTtks.FirstOrDefault(p => p.Id == choseIdMenuButton);
                        Console.WriteLine($"Incorrect photo path {photoPath}");

                        using (FileStream stream = new FileStream(photoPath.PhotoPath, FileMode.Open, FileAccess.Read))
                        {
                            InputFileStream inputOnlineFile = new InputFileStream(stream);
                            await botClient.SendPhotoAsync(chatId: chat.Id, photo: inputOnlineFile, caption: $"{userStateController.TextMessage}", replyMarkup: userStateController.menuInlineBtns);
                        }

                        //userStateControllers.Add(userStateController);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                        throw;
                    }
                    break;
                }
            case "drinkByMultipleVolumes":
                {
                    await botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
                    userStateController = new UserStateController(chat.Id, 2, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.VolumesDrinksMenuAsync(choseIdMenuButton, 2)), $"Выбери объем {choseMenuButton[0]} ");

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
                        userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.SingleOriginMenuAsync(choseIdMenuButton, 1)), $"Зерно под фильтр");
                    }
                    else
                    {
                        userStateController = new UserStateController(chat.Id, 1, callbackQuery.Message.MessageId, new InlineKeyboardMarkup(await _botMenu.SingleOriginMenuAsync(choseIdMenuButton, 1)), $"Зерно под эспрессо");
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
                    userStateController = new UserStateController(chat.Id, 2, callbackQuery.Message.MessageId,
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