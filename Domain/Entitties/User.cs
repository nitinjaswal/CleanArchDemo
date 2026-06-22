namespace Domain.Entities;

public class User
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } =string.Empty;
    public DateTime CreatedAt { get; private set; }

    // Private constructor - no one can do "new User()" directly
    protected User() { }

    // Factory method - only way to create a User
    public static User Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");

        return new User
        {
            Name = name,
            Email = email,
            CreatedAt = DateTime.UtcNow
        };
    }
}