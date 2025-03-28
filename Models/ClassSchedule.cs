using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class ClassSchedule
{
    public int Id { get; set; }

    public int ClassShiftId { get; set; }

    public int CourseId { get; set; }

    public int RoomId { get; set; }

    public int ClassId { get; set; }

    public int DayOfWeek { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public DateOnly? StartDay { get; set; }

    public DateOnly? EndDay { get; set; }

    public virtual Class? Class { get; set; } = null!;

    public virtual ClassShift? ClassShift { get; set; } = null!;

    public virtual Course? Course { get; set; } = null!;

    public virtual Room? Room { get; set; } = null!;
}
