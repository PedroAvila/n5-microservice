namespace N5.Api.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IPermissionRepository PermissionRepository { get; }

        void SaveChanges(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
