
namespace N5.Api.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AccessManagementContext _context;
        private readonly IPermissionRepository _permissionRepository;

        public UnitOfWork(AccessManagementContext context)
        {
            _context = context;
            _permissionRepository = new PermissionRepository(_context);
        }

        public IPermissionRepository PermissionRepository => _permissionRepository;

        public void Dispose() => _context?.Dispose();

        public void SaveChanges(CancellationToken cancellationToken = default) => _context.SaveChanges();

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync();
    }
}
