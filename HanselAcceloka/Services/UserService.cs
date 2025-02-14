using HanselAcceloka.Entities;
using HanselAcceloka.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HanselAcceloka.Services;

public class UserService
{
    private readonly AccelokaContext _db;
    private readonly ILogger<UserService> _logger;

    public UserService(AccelokaContext db, ILogger<UserService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<UserResponse?> AddUserAsync(UserRequest userRequest)
    {
        try
        {
            var emailExists = await _db.Users.AnyAsync(u => u.email == userRequest.Email);
            if (emailExists)
            {
                throw new InvalidOperationException("Email sudah terdaftar.");
            }

            string passwordHash = HashPassword(userRequest.Password);

            var user = new user
            {
                username = userRequest.Username,
                email = userRequest.Email,
                password_hash = passwordHash,
                created_at = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            _logger.LogInformation("User {UserId} berhasil ditambahkan.", user.user_id);

            return new UserResponse
            {
                UserId = user.user_id,
                Username = user.username,
                Email = user.email,
                CreatedAt = user.created_at
            };
        }
        catch (DbUpdateException dbEx)
        {
            _logger.LogError(dbEx, "Database error: {Message}", dbEx.InnerException?.Message ?? dbEx.Message);
            throw new Exception("Gagal menyimpan user ke database. Detail: " + dbEx.InnerException?.Message ?? dbEx.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error: {Message}", ex.Message);
            throw;
        }
    }


    private string HashPassword(string password)
    {
        return Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(password)));
    }
}