using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Repositories
{
    internal class Movies
    {
        public long year { get; set; }
        public long length { get; set; }
        public string title { get; set; }
        public string subject { get; set; }
        public string actor { get; set; }
        public string actress { get; set; }
        public string director { get; set; }
        public long popularity { get; set; }
        public bool awards { get; set; }
        public string image { get; set; }

    }
}
