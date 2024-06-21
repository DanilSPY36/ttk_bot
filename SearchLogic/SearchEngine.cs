using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttk_bot.SearchLogic
{
    public class SearchEngine<T> where T : ISearchable
    {
        public static IEnumerable<T> Search(IEnumerable<T> items, string searchTerm)
        {
            return items.Where(item => item.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }
    }
}
