using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public bool Gender { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int? ClassId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public string? Image { get; set; }

    public string Cccd { get; set; } = null!;

    public string? StudentCode { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Class? Class { get; set; }
}
