
namespace Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellation = default);
        //why use cancellation token? Because it allows us to cancel the operation if needed, which is important for long-running tasks or when the user navigates away from the page.
        //who will cancel the operation? The caller of the method, such as a controller action or a service method, can decide to cancel the operation based on certain conditions (e.g., user request, timeout, etc.).
    }
}
