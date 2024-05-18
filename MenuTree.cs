using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace ttk_bot
{
    public class MenuTree
    {
        public int menuId {  get; set; }
        public string menuName {  get; set; }
        public List<MenuTree> ttkSubState { get; set; }
        public InlineKeyboardMarkup replyMarkup { get; set; }
        public string? TextMessage { get; set; }
        public FileStream? stream { get; set; }

        public MenuTree() 
        { 
            ttkSubState = new List<MenuTree>(); 
        }
        private MenuTree(int menuId, string menuName, string TextMessage, InlineKeyboardMarkup replyMarkup)
        {
            this.TextMessage = TextMessage;
            this.replyMarkup = replyMarkup;
        }

        public void addNewSubMenu(int menuId, string menuName, string TextMessage, InlineKeyboardMarkup replyMarkup) // добавление новой ветки меню
        {
            ttkSubState.Add(new MenuTree(menuId, menuName, TextMessage, replyMarkup));
        }

    }
}
