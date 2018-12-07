using Contracts;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;

namespace InventoryService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class InventoryService : StatefulService, IRemotingInventoryService
    {
        public InventoryService(StatefulServiceContext context)
            : base(context)
        { }

        private async Task<bool> AddStockAsync(int productId, int quantity)
        {
            await InitialDataAsync();

            var items = await StateManager.GetOrAddAsync<IReliableDictionary2<int, InventoryItem>>("Inventories");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var item = await items.TryGetValueAsync(tx, productId);
                if (item.HasValue)
                {
                    item.Value.AddStock(quantity);
                    await tx.CommitAsync();
                    ServiceEventSource.Current.Message("Inventory Service Adding Committed");
                    return true;
                }

                ServiceEventSource.Current.ServiceMessage(Context, "Product is not exsited. ProductId: {0}.", productId);
                return false;
            }
        }

        public async Task<bool> CompensateStockAsync(OrderDto dto)
        {
            await InitialDataAsync();

            //NOTE check whether removed stock for this order, now always true
            if (!await CheckRemovedStockForThisOrder(dto.Id)) return false ;

            return await AddStockAsync(dto.ProductId, dto.Quantity);
        }

        private Task<bool> CheckRemovedStockForThisOrder(Guid orderId)
        {
            return Task.FromResult(orderId != Guid.Empty);
        }

        public async Task<bool> RemoveStockAsync(OrderDto dto)
        {
            await InitialDataAsync();

            //TODO save OrderId for CheckRemovedStockForThisOrder

            var items = await StateManager.GetOrAddAsync<IReliableDictionary2<int, InventoryItem>>("Inventories");

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var item = await items.TryGetValueAsync(tx, dto.ProductId);
                if (item.HasValue)
                {
                    if (item.Value.RemoveStock(dto.Quantity))
                    {
                        await items.SetAsync(tx, dto.ProductId, item.Value);

                        //NOTE Local transaction is failed, will cause CreateOrder don't commit
                        //throw new Exception("Local transaction is failed");

                        await tx.CommitAsync();
                        ServiceEventSource.Current.Message("Inventory Service Removing Committed");
                        return true;
                    }
                    ServiceEventSource.Current.Message("Inventory Service Removing UnCommitted");
                    return false;
                }

                ServiceEventSource.Current.ServiceMessage(Context, "Product is not exsited. ProductId: {0}.", dto.ProductId);
                return false;
            }
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        private async Task InitialDataAsync()
        {
            try
            {
                var items = await StateManager.GetOrAddAsync<IReliableDictionary2<int, InventoryItem>>("Inventories");

                using (ITransaction tx = StateManager.CreateTransaction())
                {

                    if (await items.GetCountAsync(tx) > 0) return;

                    await items.AddAsync(tx, 1, new InventoryItem(1, "Product 1", 10));
                    await items.AddAsync(tx, 2, new InventoryItem(2, "Product 2", 20));
                    await items.AddAsync(tx, 3, new InventoryItem(3, "Product 3", 30));

                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Message("ERROR: " + ex.Message);
            }
        }
    }
}
