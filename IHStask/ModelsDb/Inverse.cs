using System;
using System.Collections.Generic;

namespace IHStask
{
    public partial class Inverse
    {
        public int Id { get; set; }
        public int DocId { get; set; }
        public int WordId { get; set; }

        public Document Doc { get; set; }
        public Word Word { get; set; }
    }
}
