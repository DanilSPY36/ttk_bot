using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

public partial class SpotsDim
{
    public int Id { get; set; }

    public string SpotName { get; set; } = null!;

    public string? Region { get; set; }

    public string? City { get; set; }
}
