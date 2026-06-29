using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace E_Commerce.API.Controllers
{
    public class ProductsController : ApiBaseController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }
        // Get all products
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetAllProducts(CancellationToken ct = default)
        {
            var res = await productService.GetAllProductsAsync(ct);
            return ToActionResult(res);
        }
        // Get Product By Id 
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProblemDetails) , StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id , CancellationToken ct = default)
        {
            var res = await productService.GetProductByIdAsync(id, ct);
            return ToActionResult(res);
        }
        // Get all types
        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<TypeDto>>> GetAllTypes(CancellationToken ct = default)
        {
            var res = await productService.GetAllTypesAsync(ct);
            return ToActionResult(res);
        }
        // Get all brands
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<BrandDto>>> GetAllBrands(CancellationToken ct = default)
        {
            var res = await productService.GetAllBrandsAsync(ct);
            return ToActionResult(res);
        }
    }
}
