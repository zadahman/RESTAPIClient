using System;
using CsvHelper.Configuration.Attributes;

namespace Client.Repositories
{
	public class Record
    {
        [Index(0)]
        public long id { get; set; }
        [Index(1)]
        public long year { get; set; }
        [Index(2)]
        public long length { get; set; }
        [Index(3)]
        public string title { get; set; }
        [Index(4)]
        public string subject { get; set; }
        [Index(5)]
        public string actor { get; set; }
        [Index(6)]
        public string actress { get; set; }
        [Index(7)]
        public string director { get; set; }
        [Index(8)]
        public long popularity { get; set; }
        [Index(9)]
        public bool awards { get; set; }
        [Index(10)]
        public string image { get; set; }
    }
}

