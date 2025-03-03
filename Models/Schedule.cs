using System;
using System.Collections.Generic;

namespace DocumentationAndReports.Models;

public partial class Schedule
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public string? Type { get; set; }

    public string? SoldierName { get; set; }
}
