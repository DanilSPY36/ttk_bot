using System;
using System.Collections.Generic;

namespace ttk_bot.Models;

public partial class Operation
{
    public long UserId { get; set; }

    public int BranchId { get; set; }

    public int ProductId { get; set; }

    public long Timestamp { get; set; }

    public int Id { get; set; }

    public bool? IsFake { get; set; }
}
