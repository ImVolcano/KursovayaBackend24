using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameShopModel
{
    public class Transaction
    {
        public int Id { get; set; }
        public int User_id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
