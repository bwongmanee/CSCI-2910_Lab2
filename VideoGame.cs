using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WongmaneeB_AdvancedCSharp
{
    public class VideoGame : IComparable<VideoGame>
    {
        // ======================== PROPERTIES ======================== //

        // BASIC PROPERTIES //
        public string Name { get; set; }
        public string Platform { get; set; }
        public int Year { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }

        // SALES //
        public decimal NASales { get; set; }
        public decimal EUSales { get; set; }
        public decimal JPSales { get; set; }
        public decimal OtherSales { get; set; }
        public decimal GlobalSales { get; set; }



        // ======================== DEFAULT CONSTRUCTOR ======================== //
        public VideoGame()
        {
            Name = "VG_Name";
            Platform = "VG_Platform";
            Year = 0;
            Genre = "VG_Genre";
            Publisher = "VG_Publisher";

            NASales = 0;
            EUSales = 0;
            JPSales = 0;
            OtherSales = 0;
            GlobalSales = 0;
        }

        // ======================== PARAMETERIZED CONSTRUCTOR ======================== //
        public VideoGame(string name, string platform, int year, string genre, string publisher,
            decimal naSales, decimal euSales, decimal jpSales, decimal otherSales, decimal globalSales)
        {
            Name = name;
            Platform = platform;
            Year = year;
            Genre = genre;
            Publisher = publisher;

            NASales = naSales;
            EUSales = euSales;
            JPSales = jpSales;
            OtherSales = otherSales;
            GlobalSales = globalSales;
        }



        // ======================== COMPARETO METHOD ======================== //
        public int CompareTo(VideoGame? other)
        {
            int comparisonValue = Name.CompareTo(other.Name);

            return comparisonValue;
        }



        // ======================== TOSTRING METHOD ======================== //
        public override string ToString()
        {
            string header = $"===== {Name.ToUpper()} ({Year}) =====";
            string footer = new string('=', header.Length);

            string str = Environment.NewLine;
            str += header;
            str += Environment.NewLine;
            str += $"\nGENRE: {Genre}";
            str += Environment.NewLine;
            str += $"\nPLATFORM: {Platform}";
            str += $"\nPUBLISHER: {Publisher}";
            str += Environment.NewLine;
            str += $"\nNA SALES:  ${NASales.ToString("0.00")}";
            str += $"\nEU SALES:  ${EUSales.ToString("0.00")}";
            str += $"\nJP SALES:  ${JPSales.ToString("0.00")}";
            str += $"\nOther SALES:  ${OtherSales.ToString("0.00")}";
            str += $"\nGlobal SALES:  ${GlobalSales.ToString("0.00")}";
            str += Environment.NewLine;
            str += Environment.NewLine;
            str += footer;
            str += Environment.NewLine;

            return str;
        }
    }
}
