using GymSystem.DAL.Entities.Identity;
using GymSystem.DAL.Identity;

public class ActiveUserManager
{
	private readonly AppIdentityDbContext _context;

	public ActiveUserManager(AppIdentityDbContext context)
	{
		_context = context;
	}

	public bool IsUserLoggedIn(string userId)
	{
		if (string.IsNullOrWhiteSpace(userId))
			throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");

		return _context.ActiveUsers.Any(a => a.UserId == userId);
	}

	public async Task AddUser(string userId)
	{
		if (string.IsNullOrWhiteSpace(userId))
			throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");

		if (IsUserLoggedIn(userId))
			throw new InvalidOperationException($"User with ID {userId} is already active.");

		var activeUser = new ActiveUser
		{
			UserId = userId,
			LoginTime = DateTime.UtcNow
		};

		_context.ActiveUsers.Add(activeUser);
		await _context.SaveChangesAsync();
	}

	public async Task RemoveUser(string userId)
	{
		if (string.IsNullOrWhiteSpace(userId))
			throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");

		var activeUser = await _context.ActiveUsers.FindAsync(userId);
		if (activeUser == null)
			throw new InvalidOperationException($"User with ID {userId} is not active.");

		_context.ActiveUsers.Remove(activeUser);
		await _context.SaveChangesAsync();
	}
}