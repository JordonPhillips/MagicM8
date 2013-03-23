using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Scraping
{
    [TestFixture]
    class TestScraping
    {
        private const string card1 = "Exile";
        private CardInfo cardInfo1;

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void TestCardInfo()
        {
            var source = Scraping.MCISource(card1);
            var ci = Scraping.ScrapeMCIForCard(card1, source);
            Assert.AreEqual("", ci);
        }
    }
}
