using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Room
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Building { get; set; }

    public string? Floor { get; set; }

    public bool Vacuity { get; set; }

    public bool Status { get; set; }

    public DateTime CreateDay { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();
}
