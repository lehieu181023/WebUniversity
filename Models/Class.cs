using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Class
{
    public int Id { get; set; }

    public string ClassName { get; set; } = null!;

    public int? FacultyId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual Faculty? Faculty { get; set; }

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}
