using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// КБЖУ ТТК
/// </summary>
public partial class KbjuTtk
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Variety { get; set; }

    public float? Proteins { get; set; }

    public float? Fats { get; set; }

    public float? Carbohydrates { get; set; }

    public float? Calories { get; set; }

    public float? Energy { get; set; }

    public float? Caffeine { get; set; }
}
