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
        private readonly TgBotContext _context;
        public VolumesRepository(TgBotContext context)
        {
            _context = context;
        }
        public async Task<List<VolumesDim>> Get()
        {
            return _context.VolumesDims.ToList();
        }
    }
}
