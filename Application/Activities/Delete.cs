using Application.Core;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;

            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<Unit>> Handle(
                Command request,
                CancellationToken cancellationToken
            )
            {
                Activity activity = await _context.Tbl_Activity.FindAsync(request.Id);

                if (activity == null)
                    return Result<Unit>.Failure($"failed to delete activity,  ID: {request.Id}", 2);

                _context.Remove(activity);

                bool result = await _context.SaveChangesAsync() > 0;

                if (!result)
                    return Result<Unit>.Failure($"failed to delete activity,  ID: {request.Id}", 3);

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}
