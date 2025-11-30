using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services.Dtos;
using FinanceTrackerAPI.Services.Interfaces;

namespace backend.Services
{
    public class UserService : IUserService
    {
        private readonly FinanceTrackerDbContext _context;

        public UserService(FinanceTrackerDbContext context) 
        {
            _context = context;
        }

        public async Task<UserDto>CreateUserAsync(CreateUserDto dto)
        {
            //validate input 
            if(dto == null) 
            {
                throw new ArgumentNullException("User info not valid");
            }
            ///Map DTO to entity 
            var user = new User
            {
                Id = 0,
                FirstName = dto.FirstName,
                LastName = dto.LastName, 
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
            }; 
            //Save to DB 
            _context.Add(user);
            await _context.SaveChangesAsync();
            //Map entity to DTO and return 
            return new UserDto
            {
                Id = dto.Id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Role = dto.Role,
            }; 
        }

        public async Task<UserDto>GetUserByIdAsync(int id)
        {
            //Fetch from DB 
            var user = await _context.Users.FirstOrDefault(x => x.Id == id);
            //Handle not found 
            if(user == null)
            {
                throw new ArgumentException($"User with id {id} not found."); 

            }
            //Map to DTO and return 
            return new UserDto 
            {
                Id = id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto)
        {
            //Find user 
            var user = await _context.Users.FirstOrDefault(x => x.id == id);
            //Validate input
            if(user == null)
            {
                throw new ArgumentException($"User with id {id} is not found.");
            }

            if(!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;

            if(!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;
            
                if(!string.IsNullOrEmpty(dto.Email))
                    user.Email = dto.Email;
            
            //SAVE changes 
            await _context.SaveChangesAsync();

            //Update Properties
            return new UserDto
            {
                Id = id,
                FirsName = user.FirsName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _context.Users.ToListAsync();
            return users.Select(a => new UserDto{
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email,
                Role = a.Role,
            });
        }

        public async Task<bool>DeleteUserAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                return false;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
