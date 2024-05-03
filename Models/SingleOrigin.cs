using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Моносорта
/// </summary>
public partial class SingleOrigin
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TypeId { get; set; }

    public string? Region { get; set; }

    public string? Height { get; set; }

    public string? Station { get; set; }

    public string? Process { get; set; }

    public string? Variety { get; set; }

    public string? Flavor { get; set; }

    public string? Acidity { get; set; }

    public string? Taste { get; set; }

    public string? Aftertaste { get; set; }

    public string? Body { get; set; }

    public float Q { get; set; }

    public string? Description { get; set; }
}
