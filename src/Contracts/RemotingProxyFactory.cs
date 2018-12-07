using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace Contracts
{
    public static class RemotingProxyFactory
    {
        private static readonly ServiceProxyFactory _proxyFactory = new ServiceProxyFactory((c) => new FabricTransportServiceRemotingClientFactory());
        private const string _applicationName = "DTXonSF";


        public static IRemotingOrderService CreateOrderService()
        {
            var fabricUri = new Uri("fabric:/" + _applicationName + "/" + "OrderService");
            return _proxyFactory.CreateServiceProxy<IRemotingOrderService>(fabricUri);
        }

        public static IRemotingInventoryService CreateInventoryService()
        {
            var fabricUri = new Uri("fabric:/" + _applicationName + "/" + "InventoryService");
            return _proxyFactory.CreateServiceProxy<IRemotingInventoryService>(fabricUri);
        }
    }
}
