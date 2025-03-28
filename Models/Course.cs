using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Course
{
    public int Id { get; set; }

    public int? SubjectId { get; set; }

    public int? LecturerId { get; set; }

    public int Semester { get; set; }

    public int SchoolYear { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public string? CourseName { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual Lecturer? Lecturer { get; set; }

    public virtual Subject? Subject { get; set; }
}
