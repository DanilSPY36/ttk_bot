using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Продукция поставщиков
/// </summary>
public partial class Item
{
    public int Id { get; set; }

    public string ShipperId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>
    /// Состав
    /// </summary>
    public string? Composition { get; set; }

    public int? Weight { get; set; }

    public float? Proteins { get; set; }

    public float? Fats { get; set; }

    public float? Carbohydrates { get; set; }

    public float? Calories { get; set; }

    public float? Energy { get; set; }

    public bool? Vegan { get; set; }

    public bool? SugarFree { get; set; }

    public bool? GlutenFree { get; set; }

    public bool? DairyFree { get; set; }

    public bool? SoyaFree { get; set; }

    /// <summary>
    /// 100% Натуральный!!! Эксперты в шоке!! Для мужского здоро...
    /// </summary>
    public bool? Natural100 { get; set; }

    /// <summary>
    /// Условия хранения
    /// </summary>
    public string? StorageCond { get; set; }

    /// <summary>
    /// Срок годности
    /// </summary>
    public string? ExpirationDate { get; set; }

    public string? Allergens { get; set; }

    public int CategoryId { get; set; }

    public string? PhotoPath { get; set; }
}
