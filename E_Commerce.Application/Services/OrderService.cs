using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Order;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Orders;
using E_Commerce.Domain.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderService(
      IMapper mapper,
      IUnitOfWork unitOfWork,
      IBasketRepository basketRepository) : IOrderService
    {
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken cancellationToken = default)
        {
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId, cancellationToken);

            if (basket == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket Not Found", $"Basket With Id {orderDto.BasketId} Is Not Found"));

            if (basket.Items.Count == 0)
                return Result<OrderToReturnDto>.Fail(Error.Validation("Basket is Empty", $"Can Not Create Order With Basket With Id {orderDto.BasketId}"));

            var existingOrder = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new PaymentIntentSpec(basket.PaymentIntentId), cancellationToken);

            if (existingOrder is not null)
                unitOfWork.GetRepository<Order, Guid>().Remove(existingOrder);

            var orderRepo = unitOfWork.GetRepository<Order, Guid>();
            var productRepo = unitOfWork.GetRepository<Product, int>();

            var productIds = basket.Items.Select(i => i.Id).ToHashSet();
            var products = (await productRepo.GetAllAsync(new ProductsWithIdsSpecifications(productIds), cancellationToken)).ToDictionary(x => x.Id);

            var orderItems = new List<OrderItem>(basket.Items.Count);
            foreach (var item in basket.Items)
            {
                if (!products.TryGetValue(item.Id, out var product))
                    return Result<OrderToReturnDto>.Fail(Error.NotFound("Product Not Found", $"Product With Id {item.Id} Is Not Found "));

                orderItems.Add(new OrderItem
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    Product = new ProductItemOrdered
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl = product.PictureUrl
                    }
                });
            }

            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShipToAddress);
            var deliveryRepo = unitOfWork.GetRepository<DeliveryMethod, int>();
            var deliveryMethod = await deliveryRepo.GetByIdAsync(orderDto.DeliveryMethodId, cancellationToken);
            if (deliveryMethod == null)
                return Result<OrderToReturnDto>.Fail(Error.NotFound("Delivery Method Not Found", $"DeliveryMethod With Id {orderDto.DeliveryMethodId} Is Not Found "));

            var subTotal = orderItems.Sum(x => x.Quantity * x.Price);
            var order = new Order(email, orderItems, orderAddress, deliveryMethod, subTotal , basket.PaymentIntentId);

            orderRepo.Add(order);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);

            if (result <= 0)
            {
                return Result<OrderToReturnDto>.Fail(Error.Failure("Order Save Failed", "Cannot create order."));
            }

            await basketRepository.DeleteBasketAsync(orderDto.BasketId, cancellationToken);

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken cancellationToken = default)
        {
            var deliveryMethods = await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync(cancellationToken);
            return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersForUserAsync(string email, CancellationToken ct = default)
        {
            var orders = await unitOfWork.GetRepository<Order, Guid>().GetAllAsync(new OrderSpecifications(email) , ct);
            if (orders.Any()) return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
            return Result<IReadOnlyList<OrderToReturnDto>>.Fail(Error.NotFound("Orders Not Found", $"No Orders Found For User With Email {email}"));
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAndEmailForUserAsync(Guid id, string email, CancellationToken ct = default)
        {
            var order = await unitOfWork.GetRepository<Order, Guid>().GetByIdAsync(new OrderSpecifications(id, email), ct);
            if (order == null) return Result<OrderToReturnDto>.Fail(Error.NotFound("Order Is Not Found", $"Order With Id {id} Is Not Found"));
            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }
    }
}
