using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductServer.Models
{
    public class Order
    {
        public Order() { this.OrderItems = new HashSet<OrderItem>(); }
        public int Id { get; set; }
        public string email { get; set; }
        public decimal precoFrete { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public DateTime dataPedido { get; set; }
        public DateTime dataEntrega { get; set; }
        public string status { get; set; }
        public decimal precoPedido { get; set; }
        public decimal peso { get; set; }
    }
}