using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Organization
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public Organization Parent { get; set; }
        public List<Organization> Children { get; set; } = new List<Organization>();
    }
}
