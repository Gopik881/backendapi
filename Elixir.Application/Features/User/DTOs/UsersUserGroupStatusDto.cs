using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.User.DTOs;

public class UsersUserGroupStatusDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Status { get; set; }
    public bool IsEligible { get; set; }
    public bool? IsSameGroup { get; set; }
    public bool? IsDefaultGroup { get; set; }
    public string NotEligibleReason { get; set; } = String.Empty;
}
