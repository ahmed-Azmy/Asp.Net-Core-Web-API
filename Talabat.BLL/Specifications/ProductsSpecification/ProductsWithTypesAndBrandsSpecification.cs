using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.APIs.Specifications;
using Talabat.BLL.Specifications.ProductsSpecification;
using Talabat.DAL.Entities;

namespace Talabat.BLL.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams) :
            base(P =>
                 (string.IsNullOrEmpty(productParams.Search) || P.Name.ToLower().Contains(productParams.Search)) &&
                 (!productParams.BrandId.HasValue || productParams.BrandId == P.ProductBrandId) &&
                 (!productParams.TypesId.HasValue || productParams.TypesId == P.ProductTypeId)
                )
        {
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);

            ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1), productParams.PageSize);

            AddOrderBy(P => P.Name);

            if(!string.IsNullOrEmpty(productParams.Sort))
            {
                switch (productParams.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDecending(P => P.Price);
                            break;
                    default:
                        AddOrderBy(P => P.Name);
                        break;
                }
            }
        }
        public ProductsWithTypesAndBrandsSpecification(int id):base(P => P.Id == id)
        {
            AddInclude(P => P.ProductType);
            AddInclude(P => P.ProductBrand);
        }
    }
}
