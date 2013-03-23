using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Scraping
{
    class Scraping
    {
        private const string MCI = "http://magiccards.info";

        public static string ScrapeMCIForCard(string cardname)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            StreamReader reader = null;
            string html = null;
            var url = CreateMCIQueryURL(cardname);

            try
            {
                request = (HttpWebRequest) WebRequest.Create(url);

                response = (HttpWebResponse) request.GetResponse();
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

        //Create the MagicCards.info query url:
        private static string CreateMCIQueryURL(string cardname)
        {
            var vals = new NameValueCollection {{"q", cardname.Replace(' ', '+')}};
            var result = new StringBuilder();
            result.AppendFormat("{0}/query?", MCI);
            result.Append(string.Join("&", vals.AllKeys.Select(key => string.Format("{0}={1}", key, vals[key]))));
            return result.ToString();
        }
    }
}
