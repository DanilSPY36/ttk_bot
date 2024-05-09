using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Категории товаров
/// </summary>
public partial class CategoriesItemsDim
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;
}
