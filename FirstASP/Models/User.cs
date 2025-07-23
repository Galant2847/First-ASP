using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FirstASP.Models;

public class User
{
    public int ID { get; init; }
    
    [Required]
    [StringLength(50)]
    public required string Username { get; set; }
    
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
    
    [Required]
    public required string HashPassword { get; set; }
    
    public bool IsEmailVerified { get; set; } = false;
    public string? EmailVerificationCode { get; set; }
    public DateTime? EmailVerificationCodeExpiry { get; set; }

    public string Role { get; init; } = "user";
}

public class LoginDto
{
    [DefaultValue("username")]
    [Required] public required string Username { get; set; }

    [DefaultValue("password")]
    [Required] public required string Password { get; set; }
}

public class RegisterDto
{
    [DefaultValue("username")]
    [Required] 
    public required string Username { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [StringLength(int.MaxValue, MinimumLength = 8,
    ErrorMessage = "Длина пароля должна быть от 8 символов")]   
    [DefaultValue("password")]
    [Required] public required string Password { get; set; }
}