using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Participation
{
    public int StudentId { get; set; }

    public int ActivityId { get; set; }

    public int? ConductScore { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual StudentActivity Activity { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
