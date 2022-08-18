using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.APIs.Specifications;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Specifications.ProductsSpecification
{
    public class ProducWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProducWithFiltersForCountSpecification(ProductSpecParams productParams) :
            base(P =>
                 (string.IsNullOrEmpty(productParams.Search) || P.Name.ToLower().Contains(productParams.Search)) &&
                 (!productParams.BrandId.HasValue || productParams.BrandId == P.ProductBrandId) &&
                 (!productParams.TypesId.HasValue || productParams.TypesId == P.ProductTypeId)
                )
        {
        }
    }
}
