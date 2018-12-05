using System;
using System.Runtime.Serialization;

namespace Contracts
{
    [DataContract]
    public class OrderDto
    {
        public Guid Id { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}