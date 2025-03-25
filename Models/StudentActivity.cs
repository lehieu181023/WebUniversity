using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class StudentActivity
{
    public int Id { get; set; }

    public string ActivityName { get; set; } = null!;

    public DateOnly EventDate { get; set; }

    public string Location { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Participation> Participations { get; set; } = new List<Participation>();
}
