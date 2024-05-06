using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Категории товаров
/// </summary>
public partial class CategoriesDim1
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;
}
