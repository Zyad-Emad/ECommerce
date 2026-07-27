using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    public class BasketsController : ApiBaseController
    {
        private readonly IBasketService basketService;

        public BasketsController(IBasketService basketService)
        {
            this.basketService = basketService;
        }
        // Get BaseUrl/api/Baskets/Id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<BasketDto>> GetBasket(string id , CancellationToken ct = default)
        {
            var res = await basketService.GetBasketAsync(id, ct);
            return ToActionResult(res);
        }
        // Post BaseUrl/api/Baskets -> Body [ BaskedDto ]
        [HttpPost]
        public async Task<ActionResult<BasketDto>> CreateOrUpdateBasket(BasketDto basket , CancellationToken ct = default)
        {
            var res = await basketService.CreateOrUpdateBasketAsync(basket, ct: ct);
            return ToActionResult(res);
        }
        // Delete BaseUrl/api/Baskets/Id
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteBasket(string id , CancellationToken ct = default)
        {
            var res = await basketService.DeleteBasketAsync(id, ct);
            return ToActionResult(res);
        }
    }
}
