using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OptTorg.Models
{
    public class Polzovateli
    {
        public int Polzovatel_id { get; set; }
        public string Familiya { get; set; }
        public string Imya { get; set; }
        public string Otchestvo { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }

        public Avtorizaciya Avtorizaciya { get; set; }
    }
}