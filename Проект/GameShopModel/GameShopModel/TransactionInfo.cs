using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShopModel
{
    public class TransactionInfo
    {
        public int Id { get; set; }
        public int Transaction_id { get; set; }
        public int Product_id { get; set; }
        public string Product_type { get; set; }
        public int Quantity { get; set; }
        public int Product_price { get; set; }
    }
}
