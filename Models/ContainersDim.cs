using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Все возможный тары для товаров
/// </summary>
public partial class ContainersDim
{
    public int Id { get; set; }

    public string Container { get; set; } = null!;
}
