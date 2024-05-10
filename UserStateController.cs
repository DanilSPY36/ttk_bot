using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace ttk_bot
{
    public class UserStateController
    {
        public UserStateController() { }
        public UserStateController(long userChatId, int userMenuIndex, int messageIndex, InlineKeyboardMarkup menuInlineBtns, string TextMessage)
        {
            this.userChatId = userChatId;
            this.userMenuIndex = userMenuIndex;
            this.messageIndex = messageIndex;
            this.menuInlineBtns = menuInlineBtns;
            this.TextMessage = TextMessage;
        }

        public int userMenuIndex { get; set; } = 0;
        public int messageIndex { get; set; } = 0;
        public long userChatId { get; set; } = 0;
        public InlineKeyboardMarkup menuInlineBtns { get; set; }
        public string? TextMessage { get; set; }

        public void RemoveMenu(InlineKeyboardMarkup menuInlineBtns)
        {
            TextMessage = null;
            userMenuIndex = 0;
            this.menuInlineBtns = menuInlineBtns;
        }
    }
}
