using WebLab3.Data;
using WebLab3.Data.Interfaces;
using WebLab3.Data.Interfaces.Repositories;
using WebLab3.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace WebLab3.Data.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
        where T : BaseModel
    {
        protected WebDbContext _webDbContext;
        protected DbSet<T> _dbSet;

        public BaseRepository(WebDbContext webDbContext)
        {
            _webDbContext = webDbContext;
            _dbSet = webDbContext.Set<T>();
        }

        public virtual void Add(T data)
        {
            _webDbContext.Add(data);
            _webDbContext.SaveChanges();
        }

        public virtual bool Any()
        {
            return _dbSet.Any();
        }

        public virtual void Delete(T data)
        {
            _dbSet.Remove(data);
            _webDbContext.SaveChanges();
        }

        public virtual void Delete(int id)
        {
            var data = Get(id);
            Delete(data);
        }

        public virtual T? Get(int id)
        {
            return _dbSet.FirstOrDefault(x => x.Id == id);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
    }
}
