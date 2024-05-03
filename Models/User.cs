using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Все пользователи, пользовавшиеся ботом
/// </summary>
public partial class User
{
    public int Id { get; set; }

    /// <summary>
    /// tg name
    /// </summary>
    public string Name { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? ChatId { get; set; }

    public int? TgUserId { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsAccess { get; set; }

    public int? SpotId { get; set; }
}
