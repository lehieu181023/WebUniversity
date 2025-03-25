using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Faculty
{
    public int Id { get; set; }

    public string FacultyName { get; set; } = null!;

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Lecturer> Lecturers { get; set; } = new List<Lecturer>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
