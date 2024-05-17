using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Должности
/// </summary>
public partial class RolesDim
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;
}
