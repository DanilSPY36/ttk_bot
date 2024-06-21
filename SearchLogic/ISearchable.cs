using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttk_bot.SearchLogic
{
    public interface ISearchable
    {
        string Name { get; }
        string Description { get; }
        string Composition { get; }
    }
}