using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class House
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        //乐观锁
        public byte[] RowVersion { get; set; }
    }
}
