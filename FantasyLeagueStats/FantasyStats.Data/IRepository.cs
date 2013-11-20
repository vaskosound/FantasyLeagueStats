using System;
using System.Linq;
using System.Linq.Expressions;

namespace FantasyStats.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();

        T GetById(int id);

        T GetById(string id);

        void Add(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(int id);

        void DeleteRange(Expression<Func<T, bool>> predicate);

        void Detach(T entity);
    }
}
