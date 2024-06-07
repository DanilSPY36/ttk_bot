using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttk_bot.Models;

namespace ttk_bot.Repositories
{
    public class VolumesRepository
    {
        private readonly TgBotDbContext _context;
        public VolumesRepository(TgBotDbContext context)
        {
            _context = context;
        }
        public async Task<List<VolumesDim>> Get()
        {
            return _context.VolumesDims.ToList();
        }
    }
}
