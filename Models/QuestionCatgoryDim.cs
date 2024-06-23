using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

/// <summary>
/// Категории вопросов
/// </summary>
public partial class QuestionCatgoryDim
{
    public int Id { get; set; }

    public string Category { get; set; } = null!;
}
