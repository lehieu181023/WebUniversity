using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Enrollment
{
    public int Id { get; set; }

    public int? StudentId { get; set; }

    public int? CourseId { get; set; }

    public double? Score { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual Course? Course { get; set; }

    public virtual Student? Student { get; set; }
}
