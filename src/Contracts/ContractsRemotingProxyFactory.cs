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
    public class ContractsRemotingProxyFactory
    {
        private static readonly ServiceProxyFactory _proxyFactory = new ServiceProxyFactory((c) => new FabricTransportServiceRemotingClientFactory());
        private const string _applicationName = "DTXonSF";


        public static IRemotingOrderService CreateRemotingOrderService()
        {
            var fabricUri = new Uri("fabric:/" + _applicationName + "/" + "OrderService");
            return _proxyFactory.CreateServiceProxy<IRemotingOrderService>(fabricUri,new ServicePartitionKey(Int64.MaxValue));
        }
    }
}
