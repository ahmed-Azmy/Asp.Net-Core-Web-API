using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Talabat.DAL.Entities;

namespace Talabat.APIs.Specifications
{
    public class BaseSpecification<T> : ISpecification<T> 
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }
        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagingEnabled { get; set; }

        public BaseSpecification()
        {

        }
        public BaseSpecification(Expression<Func<T, bool>> Criteria)
        {
            this.Criteria = Criteria;
        }

        public void AddInclude(Expression<Func<T, object>> include)
        {
            Includes.Add(include);
        }
        public void AddOrderBy(Expression<Func<T, object>> orderBy)
        {
            OrderBy = orderBy;
        }
        public void AddOrderByDecending(Expression<Func<T, object>> orderByDecending)
        {
            OrderByDescending = orderByDecending;
        }

        public void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }
    }
}
