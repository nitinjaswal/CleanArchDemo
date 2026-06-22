using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

// This class IMPLEMENTS IUserRepository defined in Domain
// Domain has no idea this class exists — that's Clean Architecture!
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        //It tracks the user entity in memory, but doesn't save to the database until SaveChangesAsync is called.
        await _context.Users.AddAsync(user, cancellationToken);
        //await _context.SaveChangesAsync(cancellationToken);
    }
}