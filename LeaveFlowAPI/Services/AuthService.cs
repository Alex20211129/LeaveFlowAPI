using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Helpers;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwtHelper;

        public AuthService(AppDbContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return null;

            // 檢查 ManagerId 是否存在
            if (dto.ManagerId.HasValue)
            {
                var managerExists = await _context.Users.AnyAsync(u => u.Id == dto.ManagerId.Value);
                if (!managerExists)
                    return null;
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                ManagerId = dto.ManagerId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto
            {
                Token = _jwtHelper.GenerateToken(user),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role
            };
        }
    }
}