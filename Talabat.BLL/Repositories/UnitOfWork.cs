using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;
using Talabat.DAL;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext storeContext;
        private Hashtable _repository;

        public UnitOfWork(StoreContext storeContext)
        {
            this.storeContext = storeContext;
        }
        public async Task<int> Complete()
        {
            return await storeContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            storeContext.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if(_repository == null)
                _repository = new Hashtable();

            var type = typeof(TEntity).Name;

            if(!_repository.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(storeContext);
                _repository.Add(type, repository);
            }

            return (IGenericRepository<TEntity>) _repository[type];
        }
    }
}
