using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Optovaya_companya.Models
{
    public class Avtorizaciya
    {
        public int Avtorizaciya_id { get; set; }
        public int Polzovatel_id { get; set; }
        public string Login { get; set; }
        public string Parol { get; set; }

        public Polzovateli Polzovatel { get; set; }
    }
}