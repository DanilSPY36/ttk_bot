using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Telegram.Bot.Types;
using ttk_bot.Models;
using User = ttk_bot.Models.User;

namespace ttk_bot.Repositories
{
    public class UsersRepository
    {
        private readonly TgBotContext _context;
        private List<User> usersList = new List<User>();

        public UsersRepository()
        {
            usersList = new List<User>();
        }
        public UsersRepository(TgBotContext context)
        {
            _context = context;
        }

        public async Task<List<User>> Get()
        {
            return await _context.Users.AsNoTracking().ToListAsync();
        }
        private async Task newUserDbAdd(int id, string name, string? firstName, string? lastName, long? chatId, long? tgUserId, int? spotId, int? roleId)
        {
            var newUser = new User(id, name, firstName, lastName, chatId, tgUserId, spotId,  roleId);
            usersList.Add(newUser);
            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
        public async Task<bool> accessCheck( string name, string? firstName, string? lastName, long? chatId, long? tgUserId, int? spotId, int? roleId)
        {
            usersList = await _context.Users.AsNoTracking().ToListAsync();
            var lastUserId = await _context.Users.MaxAsync(id => id.Id);

            // проходим по списку пользователей.
            foreach (var user in usersList)
            {
                // tckb нашли возвращаем его доступ 
                if (user.TgUserId == tgUserId)
                {
                    return user.IsAccess;
                }
            }
            // если не нашли то добавляем нового в дб и в лист с доступом 0
            await newUserDbAdd(lastUserId  + 1, name, firstName, lastName, chatId, tgUserId, spotId, roleId);
            return false;
        }


    }
}
