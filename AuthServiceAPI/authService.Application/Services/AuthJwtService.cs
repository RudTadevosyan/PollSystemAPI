using authService.Domain.CustomExceptions;
using authService.Domain.Entities;
using authService.Domain.Interfaces;
using AuthService.Shared.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace authService.Application.Services
{
    public class AuthJwtService : IAuthJwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthJwtService(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration config, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _mapper = mapper;
        }

        public async Task<JwtDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                throw new NotFoundException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return GenerateJwt(user);
        }

        public async Task<JwtDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email)
                ?? throw new NotFoundException($"User with email {loginDto.Email} not found");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                throw new DomainException($"Invalid password.");

            return GenerateJwt(user);
        }

        public async Task DeleteAsync(DeleteDto deleteUserDto)
        {
            var user = await _userManager.FindByEmailAsync(deleteUserDto.Email)
                ?? throw new NotFoundException($"User with email {deleteUserDto.Email} not found");

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, deleteUserDto.Password, false);
            if (!passwordCheck.Succeeded)
                throw new DomainException($"Invalid password.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Failed to delete user.");
        }

        public async Task ChangePasswordAsync(UpdateDto updateUserDto)
        {
            var user = await _userManager.FindByEmailAsync(updateUserDto.Email)
                ?? throw new NotFoundException($"User with email {updateUserDto.Email} not found");
            
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, updateUserDto.Password, false);
            if(!passwordCheck.Succeeded)
                throw new DomainException($"Invalid password.");
            
            var result = await _userManager.ChangePasswordAsync(user, updateUserDto.Password, updateUserDto.NewPassword);
            if(!result.Succeeded)
                throw new DomainException(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        private JwtDto GenerateJwt(User user)
        {
            var claims = new List<Claim> //claims list
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: cred
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return new JwtDto(tokenString);
        }

    }
}