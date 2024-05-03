using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Основное ТТК по бару
/// </summary>
public partial class DrinksTtk
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int CategoryId { get; set; }

    public int VolumeId { get; set; }

    public int SpotId { get; set; }

    public string? Description { get; set; }

    public string? Ingridients { get; set; }

    public string? HowToCook { get; set; }

    public string? Weight { get; set; }

    public int? ContainerId { get; set; }

    public string? Additives { get; set; }

    /// <summary>
    /// Заготовки (вместо blank)
    /// </summary>
    public string? Prep { get; set; }

    /// <summary>
    /// Содержит относительный путь к изображению в папке с проектом
    /// </summary>
    public string? PhotoPath { get; set; }
}
