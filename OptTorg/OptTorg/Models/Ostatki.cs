using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OptTorg.Models
{
    public class Ostatki
    {
        public int Ostatki_id { get; set; }
        public int Kolichestvo { get; set; }
        public decimal Reserv { get; set; }
        public int Partiya_tovara_id { get; set; }
        public int Yachejka_hraneniya_id { get; set; }
    }
}