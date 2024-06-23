using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Запросы от пользователей
/// </summary>
public partial class UserQuestion
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Question { get; set; } = null!;
}
