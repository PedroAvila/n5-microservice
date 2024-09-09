using N5.Api.Models;

namespace N5.Api.Data
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(AccessManagementContext context) : base(context) { }
        
    

    }
}
