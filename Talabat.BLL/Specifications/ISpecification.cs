using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Talabat.DAL.Entities;

namespace Talabat.APIs.Specifications
{
    public interface ISpecification<T> 
    {
        public Expression<Func<T , bool>> Criteria { get; set; }

        public List<Expression<Func<T, object>>> Includes { get; set; }

        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPagingEnabled { get; set; }

    }
}
