using AutoMapper;
using E_Commerce.Application.DTOs.Order;
using E_Commerce.Domain.Entities.Orders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Profiles
{
    public class OrderItemPictureUrlResolver(IOptions<UrlSettings> options)
     : IValueResolver<OrderItem, OrderItemDto, string>
    {
        private readonly UrlSettings _urlSettings = options.Value;

        public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
        {
            var baseUrl = _urlSettings.BaseUrl.TrimEnd('/');
            var path = source.Product.PictureUrl.TrimStart('/');
            return $"{baseUrl}/Files/{path}";
        }
    } 
}
