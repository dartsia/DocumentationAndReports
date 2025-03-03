using System;
using System.Collections.Generic;

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
}
