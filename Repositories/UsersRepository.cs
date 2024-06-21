using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ttk_bot.Models;
using User = ttk_bot.Models.User;

namespace ttk_bot.Repositories
{
    public class UsersRepository
    {
        private readonly TgBotDbContext _context;
        private List<User> usersList = new List<User>();

        public UsersRepository()
        {
            usersList = new List<User>();
        }
        public UsersRepository(TgBotDbContext context)
        {
            _context = context;
        }


        public async Task<List<User>> Get()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
        private async Task newUserDbAdd(int id, string name, string? firstName, string? lastName, long? chatId, long? tgUserId)
        {
            var newUser = new User();
            newUser.Id = id;
            newUser.Name = name;
            newUser.FirstName = firstName;
            newUser.LastName = lastName;
            newUser.ChatId = chatId;
            newUser.TgUserId = tgUserId;
            usersList.Add(newUser);
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
        public async Task<bool> accessCheck(string name, string? firstName, string? lastName, long? chatId, long? tgUserId)
        {
            usersList = await _context.Users.AsNoTracking().ToListAsync();
            var lastUserId = await _context.Users.MaxAsync(id => id.Id);

            // проходим по списку пользователей.
            foreach (var user in usersList)
            {
                // tckb нашли возвращаем его доступ 
                if (user.TgUserId == tgUserId)
                {
                    return (bool)user.IsAccess;
                }
            }
            // если не нашли то добавляем нового в дб и в лист с доступом 0
            await newUserDbAdd(lastUserId  + 1, name, firstName, lastName, chatId, tgUserId);
            return false;
        }

        public async Task BotUpdateInfoMessage(ITelegramBotClient _botClient, string message)
        {
            
            var tempUsers = await Get();
            var replyMark = new InlineKeyboardMarkup(new[]
                                            {
                                                new InlineKeyboardButton("Back")
                                                {
                                                    Text = "Удалить сообщение",
                                                    CallbackData = $"Delete||0"
                                                } });

            //_botClient.SendTextMessageAsync(chatId: 465890927, message, replyMarkup: replyMark, protectContent: true);

            foreach (var user in tempUsers) 
            {
                _botClient.SendTextMessageAsync(chatId: user.ChatId , message, replyMarkup: replyMark, protectContent: true);
            }
        }


    }
}
