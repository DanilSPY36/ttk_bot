using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

public partial class Operation
{
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public int UserId { get; set; }

    /// <summary>
    /// Ветка меню
    /// </summary>
    public int BranchId { get; set; }

    public int ProductId { get; set; }
}
