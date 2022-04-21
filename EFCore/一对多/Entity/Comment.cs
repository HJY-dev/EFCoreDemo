using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public Article C_Article { get; set; }
        public long C_ArticleId { get; set; }
    }
}
