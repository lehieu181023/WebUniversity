using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Subject
{
    public int Id { get; set; }

    public string SubjectName { get; set; } = null!;

    public int Credit { get; set; }

    public int? FacultyId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Faculty? Faculty { get; set; }
}
