using Auth.API.Constants;
using Auth.API.Dtos;
using Auth.API.Entities;
using Auth.API.Helpers;
using Auth.API.Messaging.KafkaMessage;
using Auth.API.Messaging.Producers;
using Auth.API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Serilog.Context;

namespace Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly EmailHelper _emailHelper;
        private readonly IAuthenService _authenService;
        private readonly ILogger<AuthController> _logger;
        private readonly RedisHelper _redisHelper;
        private readonly RegisterProducerService _registerProducerService;
        private readonly IMapper _mapper;

        public AuthController(IUserService userService, ITokenService tokenService, EmailHelper emailHelper, IAuthenService authenService, ILogger<AuthController> logger, RedisHelper redisHelper, RegisterProducerService registerProducerService, IMapper mapper)
        {
            _userService = userService;
            _tokenService = tokenService;
            _emailHelper = emailHelper;
            _authenService = authenService;
            _logger = logger;
            _redisHelper = redisHelper;
            _registerProducerService = registerProducerService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "Login"))
            {
                _logger.LogInformation("Đăng nhập cho email: {Email}", loginDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đăng nhập không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _authenService.Authenticate(loginDto);
                if (user == null)
                {
                    _logger.LogWarning("Đăng nhập thất bại: Email hoặc mật khẩu không đúng {Email} từ {ClientIp}", loginDto.Email, ip);
                    return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
                }
                if (!user.IsVerified)
                {
                    _logger.LogWarning("Đăng nhập thất bại: Tài khoản chưa được xác minh {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Tài khoản chưa được xác minh. Vui lòng kiểm tra email của bạn." });
                }
                if (user.Status == "Inactive")
                {
                    _logger.LogWarning("Đăng nhập thất bại: Tài khoản bị khóa {Email}", loginDto.Email);
                    return Unauthorized(new { message = "Tài khoản đã bị khóa" });
                }

                _logger.LogInformation("Đăng nhập thành công cho {Email}, UserId: {UserId}", loginDto.Email, user.Id);
                return Ok(await CreateAuthResponse(user));
            }
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "ResetPassword"))
            {
                _logger.LogInformation("Yêu cầu đặt lại mật khẩu cho {Email}", resetPasswordDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đặt lại mật khẩu không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _userService.GetUserByEmail(resetPasswordDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Đặt lại mật khẩu thất bại: Không tìm thấy người dùng với email {Email}", resetPasswordDto.Email);
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                PassWordHelper.CreatePasswordHash(resetPasswordDto.NewPassword, out var passwordHash, out var passwordSalt);
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;

                var updated = await _userService.UpdateUser(user);
                if (updated)
                {
                    _logger.LogInformation("Đặt lại mật khẩu thành công cho {Email}, UserId: {UserId}", resetPasswordDto.Email, user.Id);
                    return Ok("Đặt lại mật khẩu thành công");
                }
                else
                {
                    _logger.LogError("Đặt lại mật khẩu thất bại khi lưu vào cơ sở dữ liệu cho {Email}", resetPasswordDto.Email);
                    return StatusCode(500, "Đặt lại mật khẩu thất bại khi lưu vào cơ sở dữ liệu");
                }
            }
        }

        [HttpPost("verify-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyEmailDto verifyOtpDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "VerifyOtp"))
            {
                _logger.LogInformation("Xác minh OTP cho {Email}", verifyOtpDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu OTP không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _userService.GetUserByEmail(verifyOtpDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Xác minh OTP thất bại: Không tìm thấy người dùng với email {Email}", verifyOtpDto.Email);
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                var storedOtp = await _redisHelper.GetRedisAsync<string>(user.Id.ToString());
                if (storedOtp == null || storedOtp != verifyOtpDto.Otp)
                {
                    _logger.LogWarning("Xác minh OTP thất bại: OTP không hợp lệ cho {Email}", verifyOtpDto.Email);
                    return BadRequest(new { message = "OTP không hợp lệ" });
                }

                _logger.LogInformation("Xác minh OTP thành công cho {Email}, UserId: {UserId}", verifyOtpDto.Email, user.Id);
                return Ok("Xác minh OTP thành công");
            }
        }

        [HttpPost("login-google")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithGoogle([FromBody] GoogleLoginDto googleLogin)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "LoginGoogle"))
            {
                _logger.LogInformation("Đăng nhập Google cho {Email}", googleLogin.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đăng nhập Google không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _userService.GetUserByEmail(googleLogin.Email)
                           ?? await RegisterGoogleUser(googleLogin);

                if (user == null)
                {
                    _logger.LogError("Đăng nhập Google thất bại: Lỗi đăng ký cho {Email}", googleLogin.Email);
                    return BadRequest(new { message = "Đăng ký thất bại" });
                }

                _logger.LogInformation("Đăng nhập Google thành công cho {Email}, UserId: {UserId}", googleLogin.Email, user.Id);
                return Ok(await CreateAuthResponse(user));
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "Register"))
            {
                _logger.LogInformation("Đăng ký tài khoản cho {Email}", registerDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu đăng ký không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                if (await _userService.GetUserByEmail(registerDto.Email) != null)
                {
                    _logger.LogWarning("Đăng ký thất bại: Email {Email} đã được sử dụng", registerDto.Email);
                    return Conflict(new { message = "Email đã được sử dụng" });
                }

                string userId = Guid.NewGuid().ToString();
                if (!await _authenService.RegisterUser(userId, registerDto))
                {
                    _logger.LogError("Đăng ký thất bại cho {Email}", registerDto.Email);
                    return BadRequest(new { message = "Đăng ký thất bại" });
                }

                var otp = new Random().Next(100000, 999999).ToString();
                await _redisHelper.SetRedisAsync(userId.ToString(), otp, TimeSpan.FromMinutes(30));

                var emailSent = await SendVerificationEmail(registerDto.Email, otp, registerDto.FullName);
                if (emailSent)
                {
                    _logger.LogInformation("Đăng ký thành công cho {Email}, UserId: {UserId}, OTP đã được gửi", registerDto.Email, userId);
                    return Ok("Đăng ký thành công. Vui lòng xác minh email của bạn.");
                }
                else
                {
                    _logger.LogError("Không thể gửi email xác minh cho {Email}", registerDto.Email);
                    return StatusCode(500, "Có lỗi xảy ra");
                }
            }
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "VerifyEmail"))
            {
                _logger.LogInformation("Xác minh email cho {Email}", verifyEmailDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu xác minh email không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _userService.GetUserByEmail(verifyEmailDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Xác minh email thất bại: Không tìm thấy người dùng với email {Email}", verifyEmailDto.Email);
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                if (user.IsVerified)
                {
                    _logger.LogWarning("Xác minh email thất bại: Email {Email} đã được xác minh", verifyEmailDto.Email);
                    return BadRequest(new { message = "Email đã được xác minh" });
                }

                var storedOtp = await _redisHelper.GetRedisAsync<string>(user.Id.ToString());
                if (storedOtp == null || storedOtp != verifyEmailDto.Otp)
                {
                    _logger.LogWarning("Xác minh email thất bại: OTP không hợp lệ cho {Email}", verifyEmailDto.Email);
                    return BadRequest(new { message = "OTP không hợp lệ" });
                }

                user.IsVerified = true;
                await _userService.UpdateUser(user);
                var messageJson = JsonConvert.SerializeObject(_mapper.Map<User, UserRegisterMessage>(user));
                await _registerProducerService.ProducerMessage(messageJson);

                _logger.LogInformation("Xác minh email thành công cho {Email}, UserId: {UserId}", verifyEmailDto.Email, user.Id);
                return Ok("Xác minh email thành công");
            }
        }

        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto resendOtpDto)
        {
            var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
            using (LogContext.PushProperty("ClientIp", ip))
            using (LogContext.PushProperty("Action", "ResendOtp"))
            {
                _logger.LogInformation("Gửi lại OTP cho {Email}", resendOtpDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Dữ liệu gửi lại OTP không hợp lệ: {@Errors}", ModelState.Values.SelectMany(v => v.Errors));
                    return ValidationErrorResponse();
                }

                var user = await _userService.GetUserByEmail(resendOtpDto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Gửi lại OTP thất bại: Không tìm thấy người dùng với email {Email}", resendOtpDto.Email);
                    return NotFound(new { message = "Không tìm thấy người dùng" });
                }

                var otp = new Random().Next(100000, 999999).ToString();
                await _redisHelper.SetRedisAsync(user.Id.ToString(), otp, TimeSpan.FromMinutes(30));

                var emailSent = await SendVerificationEmail(resendOtpDto.Email, otp, user.FullName ?? resendOtpDto.Email);
                if (emailSent)
                {
                    _logger.LogInformation("Gửi lại OTP thành công cho {Email}, UserId: {UserId}", resendOtpDto.Email, user.Id);
                    return Ok("OTP đã được gửi lại thành công");
                }
                else
                {
                    _logger.LogError("Không thể gửi lại email OTP cho {Email}", resendOtpDto.Email);
                    return StatusCode(500, "Có lỗi xảy ra");
                }
            }
        }

        private async Task<bool> SendVerificationEmail(string email, string otp, string name)
        {
            using (LogContext.PushProperty("Action", "SendVerificationEmail"))
            {
                _logger.LogInformation("Đang gửi email xác minh đến {Email} với mã OTP: {Otp}", email, otp);
                var result = await _emailHelper.SendEmail(email, "Xác minh email", otp, name);
                if (result)
                {
                    _logger.LogInformation("Gửi email xác minh thành công đến {Email}", email);
                }
                else
                {
                    _logger.LogError("Gửi email xác minh thất bại đến {Email}", email);
                }
                return result;
            }
        }

        private IActionResult ValidationErrorResponse() => BadRequest(new
        {
            message = "Dữ liệu không hợp lệ",
            errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()
        });

        private async Task<AuthResponseDto> CreateAuthResponse(User user)
        {
            using (LogContext.PushProperty("Action", "TạoPhảnHồiXácThực"))
            {
                _logger.LogDebug("Đang tạo phản hồi xác thực cho UserId: {UserId}", user.Id);

                var role = await _userService.GetRoleById(user.RoleId);
                var staffInfo = await _userService.GetStaffByUserId(user.Id);
                string branchId = user.RoleId == AppConstants.USER_ROLE_ID
                    ? Guid.Empty.ToString()
                    : staffInfo?.BranchId ?? Guid.Empty.ToString();

                var response = new AuthResponseDto
                {
                    Id = user.Id,
                    Name = user.FullName,
                    Email = user.Email,
                    Avatar = user.Avatar ?? "",
                    AccessToken = await _tokenService.GenerateToken(user),
                    RefreshToken = "",
                    RoleName = role?.RoleName ?? "Không xác định",
                    BranchId = branchId
                };

                _logger.LogDebug("Phản hồi xác thực đã được tạo cho UserId: {UserId}", user.Id);
                return response;
            }
        }

        private async Task<User?> RegisterGoogleUser(GoogleLoginDto googleLogin)
        {
            using (LogContext.PushProperty("Action", "ĐăngKýNgườiDùngGoogle"))
            {
                _logger.LogInformation("Đang đăng ký người dùng Google cho {Email}", googleLogin.Email);

                var registerDto = new RegisterDto
                {
                    Email = googleLogin.Email,
                    FullName = googleLogin.Name,
                    PassWord = Guid.NewGuid().ToString(),
                    IsLoginWithGoogle = true
                };

                var isRegistered = await _authenService.RegisterUser(Guid.NewGuid().ToString(), registerDto);
                if (isRegistered)
                {
                    var user = await _userService.GetUserByEmail(googleLogin.Email);
                    _logger.LogInformation("Người dùng Google đã được đăng ký thành công cho {Email}, UserId: {UserId}", googleLogin.Email, user?.Id);
                    return user;
                }
                else
                {
                    _logger.LogError("Đăng ký người dùng Google thất bại cho {Email}", googleLogin.Email);
                    return null;
                }
            }
        }
    }
}