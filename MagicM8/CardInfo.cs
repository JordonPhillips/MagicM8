using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scraping
{
    public class CardInfo
    {
        public string CardName { get; set; }
        public List<string> Legality { get; set; }
        public string CardText { get; set; }
        public string CardType { get; set; }
        public double Low { get; set; }
        public double Medium { get; set; }
        public double High { get; set; }

        public CardInfo(string name)
        {
            CardName = name;
            Legality = new List<string>();
        }
    }
}
