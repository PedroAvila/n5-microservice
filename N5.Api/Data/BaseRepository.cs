using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace N5.Api.Data
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly AccessManagementContext _context;

        public BaseRepository(AccessManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Set<T>().FindAsync(id, cancellationToken);
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            bool exist = await _context.Set<T>().AnyAsync(predicate, cancellationToken);
            return exist;
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var entry = await _context.Set<T>().AddAsync(entity, cancellationToken);
            return entry.Entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            await Task.Run(() =>
            {
                _context.Set<T>().Update(entity);
            }, cancellationToken);
        }
    }
}
