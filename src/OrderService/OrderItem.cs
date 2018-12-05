using Contracts;
using System;

namespace OrderService
{
    [Serializable]
    public sealed class OrderItem
    {
        public OrderItem(OrderDto dto)
        {
            Id = dto.Id;
            ProductId = dto.ProductId;
            Quantity = dto.Quantity;
        }

        public Guid Id { get; }

        public int ProductId { get; }

        public int Quantity { get; }
    }
}
