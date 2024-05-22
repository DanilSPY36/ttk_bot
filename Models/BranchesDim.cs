using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Ветки меню
/// </summary>
public partial class BranchesDim
{
    public int Id { get; set; }

    public string Branch { get; set; } = null!;
}
