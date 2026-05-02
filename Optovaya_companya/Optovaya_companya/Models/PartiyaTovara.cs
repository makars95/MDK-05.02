using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Optovaya_companya.Models
{
    public class PartiyaTovara
    {
        public int Partiya_tovara_id { get; set; }
        public DateTime Data_proizvodstva { get; set; }
        public DateTime Srok_godnosti { get; set; }
        public decimal Cena { get; set; }
        public decimal Cena_zakupki { get; set; }
        public int Tovar_id { get; set; }
    }
}