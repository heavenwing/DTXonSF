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

namespace OrderService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class OrderService : StatefulService, IRemotingOrderService
    {
        public OrderService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<bool> CreateOrderAsync(OrderDto dto)
        {
            var items = await StateManager.GetOrAddAsync<IReliableDictionary2<Guid, OrderItem>>("Orders");

            ServiceEventSource.Current.ServiceMessage(Context,
                "Received create order request. OrderId: {0}. ProductId: {1}. Quantity: {2}.", dto.Id, dto.ProductId, dto.Quantity);

            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var order = new OrderItem(dto);
                var item = await items.AddOrUpdateAsync(tx, order.Id, order, (k, v) => order);

                //remove stock
                var inventoryService = RemotingProxyFactory.CreateInventoryService();
                var resultRemovingStock = false;
                try
                {
                    resultRemovingStock = await inventoryService.RemoveStockAsync(dto);
                }
                catch (Exception ex)
                {
                    ServiceEventSource.Current.ServiceMessage(Context, "Removing stock is error. OrderId: {0}. ProductId: {1}. Quantity: {2}. ErrorMessage: {3}.", dto.Id, dto.ProductId, dto.Quantity, ex.Message);
                    return false;
                }

                if (resultRemovingStock)
                {
                    ServiceEventSource.Current.ServiceMessage(Context, "Stock removed. OrderId: {0}. ProductId: {1}. Quantity: {2}.", dto.Id, dto.ProductId, dto.Quantity);

                    try
                    {
                        //NOTE Local transaction is failed, will to compensate
                        //throw new Exception("Local transaction is failed");

                        await tx.CommitAsync();

                        ServiceEventSource.Current.ServiceMessage(Context, "Order submitted. OrderId: {0}. ProductId: {1}. Quantity: {2}.", dto.Id, dto.ProductId, dto.Quantity);

                        return true;
                    }
                    catch (Exception ex)
                    {
                        ServiceEventSource.Current.ServiceMessage(Context, "Submitting order is error. OrderId: {0}. ProductId: {1}. Quantity: {2}. ErrorMessage: {3}.", dto.Id, dto.ProductId, dto.Quantity, ex.Message);

                        //compensation, add stock, maybe with retry
                        try
                        {
                            await inventoryService.CompensateStockAsync(dto);
                        }
                        catch (Exception ex1)
                        {
                            //if compensating stock raise ex, just write log, then person deal with this error
                            ServiceEventSource.Current.ServiceMessage(Context, "Compensating stock is error. OrderId: {0}. ProductId: {1}. Quantity: {2}. ErrorMessage: {3}.", dto.Id, dto.ProductId, dto.Quantity, ex1.Message);
                        }
                        return false;
                    }
                }
                else
                {
                    ServiceEventSource.Current.ServiceMessage(Context, "Removing stock is failed. OrderId: {0}. ProductId: {1}. Quantity: {2}.", dto.Id, dto.ProductId, dto.Quantity);
                    return false;
                }
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
    }
}
