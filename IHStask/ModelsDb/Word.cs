using System;
using System.Collections.Generic;

namespace IHStask
{
    public partial class Word
    {
        public Word()
        {
            Inverse = new HashSet<Inverse>();
        }

        public int WordId { get; set; }
        public string Content { get; set; }

        public ICollection<Inverse> Inverse { get; set; }
    }
}
