using System.Xml.Serialization;

namespace OrderAPI.Models
{
    [XmlRoot("Orders")]
    public class Orders
    {
        [XmlElement("Order")]
        public List<Order> OrderList { get; set; }
    }

    public class Order
    {
        [XmlElement("OrderNumber")]
        public string OrderNumber { get; set; }

        [XmlElement("OrderDate")]
        public string OrderDate { get; set; }

        [XmlElement("CustomerName")]
        public string CustomerName { get; set; }

        [XmlElement("CustomerNumber")]
        public string CustomerNumber { get; set; }

        [XmlElement("Orderline")]
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>(); // Initialize to avoid null reference
    }

    public class OrderLine
    {
        [XmlElement("OrderLineNumber")]
        public string OrderLineNumber { get; set; }

        [XmlElement("ProductNumber")]
        public string ProductNumber { get; set; }

        [XmlElement("Quantity")]
        public int Quantity { get; set; }

        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }

        [XmlElement("ProductGroup")]
        public string ProductGroup { get; set; }
    }
}