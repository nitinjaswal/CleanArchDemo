using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler
    : IRequestHandler<CreateUserCommand, CreateUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Notice: depends on IUserRepository (Domain interface)
    // NOT on UserRepository (Infrastructure) — this is the magic!
    public CreateUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateUserResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        // Use Domain factory method
        var user = User.Create(request.Name, request.Email);

        // Save via interface — no SQL knowledge here
        await _userRepository.AddAsync(user, cancellationToken);
        //
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return new CreateUserResponse(user.Id, user.Name, user.Email);
    }
}