using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

public partial class Item
{
    public int Id { get; set; }

    public string ShipperId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

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

    public bool? Natural100 { get; set; }

    public string? StorageCond { get; set; }

    public string? ExpirationDate { get; set; }

    public string? Allergens { get; set; }

    public int CategoryId { get; set; }

    public string? PhotoPath { get; set; }

    public bool? IsArchive { get; set; }
}
