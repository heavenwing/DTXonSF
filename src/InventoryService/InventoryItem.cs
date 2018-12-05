using System;

namespace InventoryService
{
    [Serializable]
    public class InventoryItem
    {
        public InventoryItem(int id,string name,int quantity)
        {
            Id = id;
            Name = name;
            Quantity = quantity;
        }

        public int Id { get; }
        public string Name { get; }
        public int Quantity { get; }
    }
}