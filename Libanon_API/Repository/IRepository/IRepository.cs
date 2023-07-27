using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using Expression = Microsoft.Ajax.Utilities.Expression;

namespace Libanon_API.Repository.IRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);
        void Add(TEntity entity);
        void Remove(TEntity entity);
    }
}
