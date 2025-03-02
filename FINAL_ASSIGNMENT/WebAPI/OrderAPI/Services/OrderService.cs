using System.Xml.Serialization;
using OrderAPI.Models;

namespace OrderAPI.Services
{
    public class OrderService
    {
        private readonly string _filePath;

        public OrderService(IConfiguration configuration)
        {
            // Read the file path from configuration
            _filePath = configuration["FileSettings:XmlFilePath"];
        }

        public async Task<Order> GetOrderAsync(string orderNumber)
        {
            // Check if the file exists
            if (!File.Exists(_filePath))
            {
                return null; // File not found
            }

            // Read the file asynchronously
            using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            {
                var serializer = new XmlSerializer(typeof(Orders));
                Orders orders;

                // Deserialize the XML data
                orders = (Orders)await Task.Run(() => serializer.Deserialize(stream));

                // Find and return the order by number
                return orders.OrderList.Find(order => order.OrderNumber == orderNumber);
            }
        }
    }
}
