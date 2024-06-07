using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Линейка зерна (create, innovate, basse)
/// </summary>
public partial class BeanCategoriesDim
{
    public int Id { get; set; }

    public string BeanCategory { get; set; } = null!;
}
