
namespace N5.Api.Permissions.GetById
{
    public record GetPermissionByIdQuery(int Id) : IQuery<GetPermissionByIdResult>;
    public record GetPermissionByIdResult(Permission Permission);

    internal class GetPermissionByIdQueryHandler(IUnitOfWork unitOfWork)
        : IQueryHandler<GetPermissionByIdQuery, GetPermissionByIdResult>
    {
        public async Task<GetPermissionByIdResult> Handle(GetPermissionByIdQuery query, CancellationToken cancellationToken)
        {
            var exist = await unitOfWork.PermissionRepository.ExistAsync(x => x.Id == query.Id);
            if (!exist)
                throw new BusinessException($"The requested record does not exist {query.Id}", HttpStatusCode.NotFound);

            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(query.Id);
            return new GetPermissionByIdResult(permission);
        }
    }
}
