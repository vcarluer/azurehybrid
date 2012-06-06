namespace ProductsPortal.Controllers
{
	using System.Linq;
	using System.ServiceModel;
	using System.Web.Mvc;
	using Microsoft.ServiceBus;
	using Models;
	using ProductsServer;

	public class HomeController : Controller
	{
		// Declare the channel factory
		static ChannelFactory<IProductsChannel> channelFactory;

		static HomeController()
		{
			// Create shared secret token credentials for authentication 
			channelFactory = new ChannelFactory<IProductsChannel>(new NetTcpRelayBinding(),
				"sb://ortemstestbus.servicebus.windows.net/products");
			channelFactory.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior
			{
				TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(
					"owner", "8caczFZW31OFbnTK8wAj9r/0C4eI4nGmv4WojL8ztH0=")
			});
		}

		public ActionResult Index()
		{
			using (IProductsChannel channel = channelFactory.CreateChannel())
			{
				// Return a view of the products inventory
				return this.View(from prod in channel.GetProducts()
								 select
									 new Product
									 {
										 Id = prod.Id,
										 Name = prod.Name,
										 Quantity = prod.Quantity
									 });
			}
		}
	}
}