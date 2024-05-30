using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

    public long? ChatId { get; set; }

    public long? TgUserId { get; set; }

    public bool? IsAdmin { get; set; }

    public bool IsAccess { get; set; }

    public int? SpotId { get; set; }

    public int? RoleId { get; set; }

    public User(int Id, string name, string? firstName, string? lastName, long? chatId, long? tgUserId, int? spotId, int? roleId)
    {
        this.Id = Id;
        Name = name;
        FirstName = firstName;
        LastName = lastName;
        ChatId = chatId;
        TgUserId = tgUserId;
        IsAdmin = false;
        IsAccess = false;
        SpotId = spotId;
        RoleId = roleId;
    }
}