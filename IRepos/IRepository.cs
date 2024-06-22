using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttk_bot.IRepos
{
    public interface IRepository<T>
    {
        Task<List<T>> GetByName(string searchTerm);
    }
}
