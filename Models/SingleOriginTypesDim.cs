using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Типы зерна
/// </summary>
public partial class SingleOriginTypesDim
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;
}
