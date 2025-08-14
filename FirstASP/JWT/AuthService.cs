using FirstASP.Data;
using FirstASP.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FirstASP.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FirstASP.JWT;

public class AuthService(FirstApiContext context, IEmailService emailService, IConfiguration configuration)
{
    public async Task RegisterUser(RegisterDto request)
    {
        if (context.Users.Any(x => x.Username == request.Username))
            throw new Exception("User already exists");

        if (request.Username == string.Empty)
            throw new NullReferenceException("Enter username");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            HashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password),
            IsEmailVerified = false,
            EmailVerificationCode = GenerateVerificationCode(),
            EmailVerificationCodeExpiry = DateTime.UtcNow.AddMinutes(5)
        };

        context.Users.Add(user);
        await emailService.SendVerificationEmailAsync(user.Email, user.EmailVerificationCode);
        
        await context.SaveChangesAsync();
    }
    
    public async Task<bool> VerifyEmail(string email, string code)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => 
                u.Email == email && 
                u.EmailVerificationCode == code &&
                u.EmailVerificationCodeExpiry > DateTime.UtcNow);

        if (user == null) return false;

        user.IsEmailVerified = true;
        user.EmailVerificationCode = null;
        user.EmailVerificationCodeExpiry = null;
        
        await context.SaveChangesAsync();

        return true;
    }

    public string LoginUser(LoginDto request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request), "Request cannot be null");

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            throw new ArgumentException("Username and password are required!");

        var user = context.Users.FirstOrDefault(x => x.Username == request.Username);
    
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.HashPassword))
            throw new UnauthorizedAccessException("Invalid password");

        return GenerateToken(user);
    }

    private string GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        if (string.IsNullOrEmpty(configuration["Jwt:Key"]))
            throw new ArgumentNullException("Jwt:Key not configured");

        if (user.ID <= 0)
            throw new ArgumentException("Invalid User ID");
    
        if (string.IsNullOrEmpty(user.Username))
            throw new ArgumentException("User already exists");
    
        var role = string.IsNullOrEmpty(user.Role) ? "user" : user.Role;
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.ID.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, role.ToLower())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);
        
        Console.WriteLine($"Generated token for role: {role}");
    
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    private static string GenerateVerificationCode()
    {
        return new Random().Next(100000, 999999).ToString(); // 6-значный код
    }

    /*public void CreateAdmin()
    {
        var admin = new User
        {
            Username = "karim",
            HashPassword = BCrypt.Net.BCrypt.HashPassword("d67nkHUUK4"),
            Role = "admin",
            Email = "abdullinkarim420@gmail.com"
        };
        
        context.Users.Add(admin);
        context.SaveChanges();
    }*/
}