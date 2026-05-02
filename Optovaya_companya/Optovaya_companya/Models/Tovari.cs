using System;

namespace Optovaya_companya.Models
{
    public class Tovar
    {
        public int Tovar_id { get; set; }
        public string Naimenovanie { get; set; }
        public decimal Artikul { get; set; }
        public string Opisanie { get; set; }
        public string Proizvoditel { get; set; }
        public string Edinica_izmereniya { get; set; }
        public int Kategoriya_id { get; set; }
        public string Nazvanie_kategorii { get; set; }
        public string ImagePath { get; set; }

        // Для отображения в DataGrid
        public string FullName => $"{Naimenovanie} (Арт. {Artikul})";
    }
}