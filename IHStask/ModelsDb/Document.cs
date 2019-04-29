using System;
using System.Collections.Generic;

namespace IHStask
{
    public partial class Document
    {
        public Document()
        {
            Inverse = new HashSet<Inverse>();
        }

        public int DocId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }

        public ICollection<Inverse> Inverse { get; set; }
    }
}
