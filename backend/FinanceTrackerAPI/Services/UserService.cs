using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BCrypt.Net;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using FinanceTrackerAPI.FinanceTracker.Domain.Exceptions;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly FinanceTrackerDbContext _context;

        public UserService(FinanceTrackerDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            // Validate input 
            if (dto == null)
            {
                throw new ValidationException("User cannot be null.");
            }

            // Check if email is already in use (case-insensitive)
            var emailInUse = await _context.Users.AnyAsync(u => u.Email != null && u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (emailInUse)
            {
                throw new ValidationException($"Email '{dto.Email}' is already in use.");
            }

            // Map DTO to entity 
            var user = new User
            {
                Id = 0,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role ?? "User",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Token = string.Empty,  // Set during authentication
                RefreshToken = string.Empty,  // Set during authentication
            };

            // Save to DB 
            _context.Add(user);
            await _context.SaveChangesAsync();

            // Map entity to DTO and return 
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            // Fetch from DB 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            // Handle not found 
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            // Map to DTO and return 
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto dto)
        {
            // Find user 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            // Validate input
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrEmpty(dto.Email) && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                // Check if email is already in use (case-insensitive)
                var emailInUse = await _context.Users.AnyAsync(u => u.Email != null && u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase) && u.Id != id);
                if (emailInUse)
                {
                    throw new ValidationException($"Email '{dto.Email}' is already in use.");
                }
                user.Email = dto.Email;
            }

            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            if (dto.Role != null)
                user.Role = dto.Role;

            user.UpdatedAt = DateTime.UtcNow;

            // Save changes 
            await _context.SaveChangesAsync();

            // Return updated DTO
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(a => new UserDto
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Role = a.Role,
            });
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User", id);
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
