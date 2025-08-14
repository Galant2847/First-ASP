using FirstASP.Data;
using FirstASP.JWT;
using FirstASP.Models;
using Microsoft.AspNetCore.Mvc;

namespace FirstASP.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AuthService authService, FirstApiContext context) : ControllerBase
{
    // регистрация пользователя
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto request)
    {
        try
        {
            await authService.RegisterUser(request);
            return Ok("Registration successful. Check your email for verification code.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // логин пользователя
    [HttpPost("login")]
    public IActionResult Login(LoginDto request)
    {
        try
        {
            var user = context.Users.FirstOrDefault(x => x.Username == request.Username);
            var token = authService.LoginUser(request);
            
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            return Ok(new { Token = token, Message = $"Login successfully. " +
                                                     $"Role: {user.Role}, " +
                                                     $"Username: {user.Username}"});
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message); // 401
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
        catch (Exception)
        {
            return StatusCode(500, "An internal server error occurred"); // 500
        }
    }

    [HttpDelete("deleteAllUsers")]

    public IActionResult Delete()
    {
        var allUsers = context.Users.ToList();
        
        context.Users.RemoveRange(allUsers);
        context.SaveChanges();
        
        return Ok();
    }
    
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto request)
    {
        var isVerified = await authService.VerifyEmail(request.Email, request.Code);
        return isVerified 
            ? Ok("Email успешно подтверждён") 
            : BadRequest("Неверный код или срок действия истёк");
    }

    /*[HttpPost("createAdmin")]
public IActionResult CreateAdmin()
{
    _authService.CreateAdmin();
    return Ok();
}*/
}