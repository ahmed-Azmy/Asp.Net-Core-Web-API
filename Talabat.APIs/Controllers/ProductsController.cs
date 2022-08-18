using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.BLL.Interfaces;
using Talabat.BLL.Specifications;
using Talabat.BLL.Specifications.ProductsSpecification;
using Talabat.DAL.Entities;

namespace Talabat.APIs.Controllers
{
    //[Authorize]
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> productRepo;
        private readonly IGenericRepository<ProductBrand> brandsRepo;
        private readonly IGenericRepository<ProductType> typesRepo;
        private readonly IMapper mapper;

        public ProductsController(IGenericRepository<Product> ProductRepo, 
                                  IGenericRepository<ProductBrand> BrandsRepo,
                                  IGenericRepository<ProductType> TypesRepo, 
                                  IMapper mapper)
        {
            productRepo = ProductRepo;
            brandsRepo = BrandsRepo;
            typesRepo = TypesRepo;
            this.mapper = mapper;
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Pagination<ProductToReturnDto>>>> GetProducts([FromQuery]ProductSpecParams productParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(productParams);
            var CountSpec = new ProducWithFiltersForCountSpecification(productParams);
            var totalItems =await productRepo.GetCountAsync(CountSpec);
            var Products = await productRepo.GetAllWithSpecAsync(spec);

            var result = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
            if (result == null) return NotFound(new ApiResponse(404));
            return Ok(new Pagination<ProductToReturnDto>(productParams.PageIndex , productParams.PageSize, totalItems, result));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse) ,StatusCodes.Status404NotFound)]
        [HttpGet("{Id}")]
        public async Task<ActionResult<Product>> GetProduct(int Id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(Id);
            var Product = await productRepo.GetEntityWithSpecAsync(spec);

            var result = mapper.Map<Product , ProductToReturnDto>(Product);
            if (result == null) return NotFound(new ApiResponse(404));
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
        {
            var Brands = await brandsRepo.GetAllAsync();
            if (Brands == null) return NotFound(new ApiResponse(404));
            return Ok(Brands);
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetTypes()
        {
            var Types = await typesRepo.GetAllAsync();
            if (Types == null) return NotFound(new ApiResponse(404));
            return Ok(Types);
        }
    }
}
