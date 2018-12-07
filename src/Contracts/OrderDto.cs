using System;
using System.Runtime.Serialization;

namespace Contracts
{
    //NOTE dto with DataContract and(must have) DataMember or Serializable or nothing

    //[DataContract]
    //[Serializable]
    public class OrderDto
    {
        //[DataMember]
        public Guid Id { get; set; } = Guid.NewGuid();

        //[DataMember]
        public int ProductId { get; set; }

        //[DataMember]
        public int Quantity { get; set; }
    }
}