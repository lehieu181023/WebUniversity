using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int RoleGroupId { get; set; }

    public int? StudentId { get; set; }

    public int? LecturerId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual Lecturer? Lecturer { get; set; }

    public virtual RoleGroup? RoleGroup { get; set; } = null!;

    public virtual Student? Student { get; set; }
}
