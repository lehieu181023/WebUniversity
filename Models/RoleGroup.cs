using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class RoleGroup
{
    public int Id { get; set; }

    public string? NameRoleGroup { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual ICollection<RoleInRoleGroup> RoleInRoleGroups { get; set; } = new List<RoleInRoleGroup>();
}
