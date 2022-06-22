using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DGCP.APPMobile.Data.Enum;

namespace DGCP.APPMobile.Web.Services
{
    public class UrlShortenerService
    {
        private const string GoogleApiKey = "AIzaSyDRCUVp68Z5cPTB1Eccb-3MZ2asrE9Dh20";
        private const string GoogleApiURL = "https://www.googleapis.com/urlshortener/v1/url?key={0}";

        public bool ShortUrl(int serviceId, string URL, ref string tinyUrl)
        {
            var result = false;  
            try
            {
                switch (serviceId)
                {
                    case (int)UrlShortenerServices.Google:
                        result = GoogleShortener(URL, ref tinyUrl);
                    break;

                }

                
            } catch(Exception e)
            {
                result = false;
            }
            return result;
        }

        private bool GoogleShortener(string URL, ref string tinyUrl)
        {
            var result = false;  
            string post = "{\"longUrl\": \"" + URL + "\"}";
            string urlRequest = String.Format(GoogleApiURL, GoogleApiKey);
            var request = (HttpWebRequest)WebRequest.Create(urlRequest);

            try
            {
                request.ServicePoint.Expect100Continue = false;
                request.Method = "POST";
                request.ContentLength = post.Length;
                request.ContentType = "application/json";
                request.Headers.Add("Cache-Control", "no-cache");

                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] postBuffer = Encoding.ASCII.GetBytes(post);
                    requestStream.Write(postBuffer, 0, postBuffer.Length);
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var responseReader = new StreamReader(responseStream))
                            {
                                string json = responseReader.ReadToEnd();
                                tinyUrl = Regex.Match(json, @"""id"": ?""(?<id>.+)""").Groups["id"].Value;
                                result = true;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                // if Google's URL Shortner is down...
                result = false;
            }
            return result;
        }

    }
}
