using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using HtmlAgilityPack;

namespace Scraping
{
    public class Scraping
    {
        private const string MCI = "http://magiccards.info";

        public static string MCISource(string cardname)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            string html = null;
            var url = CreateMCIQueryURL(cardname);

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);

                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();

                reader = new StreamReader(responseStream, Encoding.Default);
                html = reader.ReadToEnd();
            }
            catch
            {
                throw new Exception("Something bad happened");
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream.Dispose();
                }

                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }

            return html;
        }

        public static CardInfo ScrapeMCIForCard(string cardname, string source)
        {
            //some fixing:
            source = source.Substring(source.IndexOf("<body>"));
            source = source.Substring(0, source.IndexOf("</html>"));

            XDocument doc;
            using (TextReader sr = new StringReader(source))
            {
                doc = FromHtml(sr);
            }

            //var query = string.Format("/body/table/tr/td/img[@alt=\'{0}\']", cardname);
            //var table = doc.XPathEvaluate(query);


            var table = from node in doc.Descendants()
                        where node.Name == "img" &&
                              node.Attribute("alt").Value == cardname
                        select node.Parent.Parent;

            var legal = from node in table.Descendants()
                        where node.Name == "li" &&
                        node.HasAttributes &&
                              node.Attribute("class").Value == "legal"
                        select node.Value;

            var type = from node in table.Descendants()
                       where node.Name == "p" &&
                             !node.HasAttributes
                       select node.FirstNode;

            var text = from node in table.Descendants()
                       where node.Name == "p" &&
                       node.HasAttributes &&
                             node.Attribute("class").Value == "ctext"
                       select node.Descendants();

            //var print = from node in table.Descendants()
            //            where node.

            ///tbody/tr/td[3]/small/b[2]

            //var set = doc.XPathSelectElement("/body/table[3]/tr/td[3]");

            var ci = new CardInfo(cardname);

            ci.CardText = text.First().First().ToString();
            ci.CardText = ci.CardText.Replace("<b>", "");
            ci.CardText = ci.CardText.Replace("</b>", "");
            ci.CardType = type.ToList()[0].ToString().Replace("\r\n", "").Replace("â€”", "-").Trim();
            foreach (var l in legal) ci.Legality.Add(l);

            return ci;
        }

        public static XDocument FromHtml(TextReader reader)
        {

            // setup SgmlReader
            Sgml.SgmlReader sgmlReader = new Sgml.SgmlReader();
            sgmlReader.DocType = "HTML";
            sgmlReader.WhitespaceHandling = WhitespaceHandling.All;
            sgmlReader.CaseFolding = Sgml.CaseFolding.ToLower;
            sgmlReader.InputStream = reader;

            // create document
            XDocument doc = new XDocument();
            doc = XDocument.Load(sgmlReader);
            return doc;
        }

        //Create the MagicCards.info query url:
        private static string CreateMCIQueryURL(string cardname)
        {
            var vals = new NameValueCollection { { "q", cardname.Replace(' ', '+') } };
            var result = new StringBuilder();
            result.AppendFormat("{0}/query?", MCI);
            result.Append(string.Join("&", vals.AllKeys.Select(key => string.Format("{0}={1}", key, vals[key]))));
            return result.ToString();
        }
    }
}
