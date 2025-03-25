using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Lecturer
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? FacultyId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public string? Image { get; set; }

    public bool Gender { get; set; }

    public string? Cccd { get; set; }

    public string? Address { get; set; }

    public DateOnly? BirthDate { get; set; }

    public string? LecturerCode { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual Faculty? Faculty { get; set; }
}
