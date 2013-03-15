#region Using directives

using System.Linq;

#endregion


namespace Contracts.Data
{
    public interface IProvider<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
    }
}