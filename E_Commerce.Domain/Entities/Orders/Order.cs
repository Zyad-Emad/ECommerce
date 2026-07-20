using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Orders
{
    public class Order : BaseEntity<Guid>
    {
        private Order() { }

        public Order(
            string buyerEmail,
            ICollection<OrderItem> items,
            OrderAddress shipToAddress,
            DeliveryMethod deliveryMethod,
            decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            Items = items;
            ShipToAddress = shipToAddress;
            DeliveryMethod = deliveryMethod;
            DeliveryMethodId = deliveryMethod.Id;
            SubTotal = subTotal;
        }

        public string BuyerEmail { get; private set; } = default!;
        public DateTimeOffset OrderDate { get; private set; } = DateTimeOffset.UtcNow;
        public ICollection<OrderItem> Items { get; private set; } = [];
        public OrderAddress ShipToAddress { get; private set; } = default!;
        public DeliveryMethod DeliveryMethod { get; private set; } = default!;
        public int DeliveryMethodId { get; private set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal SubTotal { get; private set; }

        public decimal GetTotal() => SubTotal + (DeliveryMethod?.Cost ?? 0m);

    }
}
