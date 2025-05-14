using System.Linq.Expressions;

namespace AddPulseShowManagement.Repo
{
    public interface IEntity<T> where T : class
    {
        T Add(T entity);
        List<T> MultipleAdd(List<T> entity);
        T Edit(T entityToUpdate);
        List<T> Get();
        IQueryable<T> GetIQueryable();
        List<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", bool isTrackingOff = false);
        List<T> Get(out int total, out int totalDisplay, Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int pageIndex = 1, int pageSize = 10, bool isTrackingOff = false);
        T GetById(int id);
        bool IsExist(Expression<Func<T, bool>> predicate);
        int GetCount(Expression<Func<T, bool>> filter = null);
        void Remove(Expression<Func<T, bool>> filter);
        void Remove(int id);
        void Remove(T entityToDelete);
        void MutipleRemove(List<T> ListEntityToDelete);
    }
}