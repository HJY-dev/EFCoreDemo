using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Order
    {
        public long Id { get; set; }
        public string GoodsName { get; set; }
        public string Address { get; set; }
        public Delivery Delivery { get; set; }
    }
}
