using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentationAndReports.Models;

public partial class Soldier
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Gender { get; set; }

    public string? State { get; set; }

    public string? RelativesEmail { get; set; }

    public List<DateOnly>? Holidays { get; set; }

    public List<DateOnly>? SickLeaves { get; set; }

    [NotMapped]
    public int UnitDuties { get; set; }
    [NotMapped]
    public int OutsideDuties { get; set; }
    [NotMapped]
    public List<int> WeekendWeeks { get; set; } = new();
    [NotMapped]
    public Duty? LastDuty { get; set; }
}
