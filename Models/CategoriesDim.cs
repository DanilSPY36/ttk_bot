using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Категории товаров
/// </summary>
public partial class CategoriesDim
{
    public int Id { get; set; }

    /// <summary>
    /// Категория товара
    /// </summary>
    public string Category { get; set; } = null!;
}
