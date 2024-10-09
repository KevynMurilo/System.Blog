using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Blog.Application.DTOs;

public class ResetPasswordDto
{
    public string Email { get; set; }
    public string VerificationCode { get; set; }
    public string NewPassword { get; set; }
}
