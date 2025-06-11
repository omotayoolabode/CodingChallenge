using CodingChallenge.Models.DTOs.Auth;
using CodingChallenge.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CodingChallenge.Controllers;

/// <summary>
/// Controller handling user authentication and authorization operations
/// </summary>
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user in the system
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/register
    ///     {
    ///         "email": "user@example.com",
    ///         "password": "P@ssw0rd!",
    ///         "confirmPassword": "P@ssw0rd!"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">User registration details</param>
    /// <returns>Registration response with user details and authentication token</returns>
    /// <response code="200">User registered successfully</response>
    /// <response code="400">If the request is invalid or user already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblemDetails))]
    public async Task<IActionResult> Register([FromBody, Required] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registering new user: {Email}", request.Email);
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and returns an access token
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/login
    ///     {
    ///         "email": "user@example.com",
    ///         "password": "P@ssw0rd!"
    ///     }
    ///
    /// </remarks>
    /// <param name="request">User login credentials</param>
    /// <returns>Authentication response with JWT token and user details</returns>
    /// <response code="200">Authentication successful</response>
    /// <response code="401">Invalid email or password</response>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(object))]
    public async Task<IActionResult> Login([FromBody, Required] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Email}", request.Email);
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Login failed for user {Email}: {Message}", request.Email, ex.Message);
            return Unauthorized(new { message = "Invalid email or password" });
        }
    }

    /// <summary>
    /// Logs out the currently authenticated user
    /// </summary>
    /// <remarks>
    /// Requires authentication via JWT token in the Authorization header
    /// </remarks>
    /// <returns>Success message</returns>
    /// <response code="200">User logged out successfully</response>
    /// <response code="401">If user is not authenticated</response>
    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        _logger.LogInformation("User logged out");
        return Ok(new { message = "Logged out successfully" });
    }
}
