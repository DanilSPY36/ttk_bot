using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

public partial class User
{
    public string Name { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public long? ChatId { get; set; }

    public long? TgUserId { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? IsAccess { get; set; }

    public int? SpotId { get; set; }

    public int? RoleId { get; set; }

    public int Id { get; set; }
}
