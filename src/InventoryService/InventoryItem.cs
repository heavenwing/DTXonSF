using System;

namespace InventoryService
{
    [Serializable]
    public class InventoryItem
    {
        public InventoryItem(int id,string name,int availableStock)
        {
            Id = id;
            Name = name;
            AvailableStock = availableStock;
        }

        public int Id { get; }
        public string Name { get; }
        public int AvailableStock { get; private set; }

        public void AddStock(int quantity)
        {
            AvailableStock += quantity;
        }

        public bool RemoveStock(int quantity)
        {
            if (quantity > AvailableStock) return false;
            AvailableStock -= quantity;
            return true;
        }
    }
}