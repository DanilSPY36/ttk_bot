using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Поставщики
/// </summary>
public partial class Shipper
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    /// ФИО
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Цифры какие-то
    /// </summary>
    public string? Inn { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Region { get; set; }
}
