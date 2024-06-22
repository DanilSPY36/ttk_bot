using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttk_bot.SearchLogic
{
    public class SearcherUser
    {
        public long id { get; set; }
        public string? tgName { get; set; }
        public bool isSearch {  get; set; }
        public int idSearchBranch { get; set; } // ветка поиска 1=TTK 2=Items 3=SingleOrigin
        public string? searchMessage { get; set; }
    }
}
