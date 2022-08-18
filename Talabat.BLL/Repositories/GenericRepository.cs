using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.APIs.Specifications;
using Talabat.BLL.Interfaces;
using Talabat.DAL;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext context;

        public GenericRepository(StoreContext context)
        {
            this.context = context;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
          => await context.Set<T>().ToListAsync();

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)  
            =>   await ApplySpecifications(spec).ToListAsync();
        

        public async Task<T> GetAsync(int Id)
          => await context.Set<T>().FindAsync(Id);

        public async Task<int> GetCountAsync(ISpecification<T> spec)
        => await ApplySpecifications(spec).CountAsync();

        public async Task<T> GetEntityWithSpecAsync(ISpecification<T> spec)
          => await ApplySpecifications(spec).FirstOrDefaultAsync();

        public async Task Add(T entity)
        => await context.Set<T>().AddAsync(entity);
        public void Update(T entity)
        => context.Set<T>().Update(entity);
        public void Delete(T entity)
        => context.Set<T>().Remove(entity);

        private IQueryable<T> ApplySpecifications(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(context.Set<T>(), spec);
        }
    }
}
