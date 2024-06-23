using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Добавленные вопросы и ответы на них
/// </summary>
public partial class ProdQuestion
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Question { get; set; } = null!;

    public string Answer { get; set; } = null!;
}
