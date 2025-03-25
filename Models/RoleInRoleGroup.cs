using System;
using System.Collections.Generic;

namespace WebUniversity.Models;

public partial class RoleInRoleGroup
{
    public int Id { get; set; }

    public int RoleId { get; set; }

    public int RoleGroupId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual RoleGroup RoleGroup { get; set; } = null!;
}
