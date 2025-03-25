using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class Role
{
    public int Id { get; set; }

    public string? RoleCode { get; set; }

    public int? ParentId { get; set; }

    public DateTime CreateDay { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<RoleInRoleGroup> RoleInRoleGroups { get; set; } = new List<RoleInRoleGroup>();
}
