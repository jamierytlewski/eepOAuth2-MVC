using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Text;

namespace eepURL_MVC
{
    public class MCAPI
    {
        // Variables
        const string _version = "1.3";
        string _apiurl, _apikey;
        int _timeout = 300;
        const int _chunkSize = 8192;
        bool _secure = false;
        Uri _uri;

        /// <summary>
        /// This is the API for MailChimp
        /// </summary>
        /// <param name="apikey">This is value that's passed in. The string is the apikey-subdomain (split by a -)</param>
        /// <param name="secure">To use https or not</param>
        public MCAPI(string apikey, bool secure = true)
        {
            var apikeySplit = apikey.Split('-');
            _apikey = apikeySplit[0];
            _secure = secure;
            _apiurl = "http" + (secure ? "s" : "") + "://" + apikeySplit[1] + ".api.mailchimp.com/" + _version + "/?output=json";
            _uri = new Uri(_apiurl);
        }

        /// <summary>
        /// Sets the Time out
        /// </summary>
        /// <param name="seconds"></param>
        void setTimeout(int seconds)
        {
            _timeout = seconds;
        }

        /// <summary>
        /// Returns the timeout
        /// </summary>
        /// <returns></returns>
        int getTimeout()
        {
            return _timeout;
        }


        #region list
        /// <summary>
        /// lists
        /// <param name="filters">filters</param>
        /// <param name="start">start results at this list #, defaults at 1</param>
        /// <param name="start">number of lists to return, defaults at 25</param>
        /// </summary>
        public string lists(string filters, string start = "1", string limit = "25")
        {
            return makeCall("lists", new Dictionary<string, string>() { { "filters", filters }, { "start", start }, { "limit", limit } });
        }

        // listActivity
        public string listActivity(string id)
        {
            return makeCall("listActivity", new Dictionary<string, string>() { { "id", id } });
        }

        #endregion


        #region campaigns
        /// <summary>
        /// Get the content (both html and text) for a campaign either as it would appear in the campaign archive or as the raw, original content
        /// </summary>
        /// <param name="cid">the campaign id to get content for (can be gathered using campaigns())</param>
        /// <param name="for_archive">optional - controls whether we return the Archive version (true) or the Raw version (false), defaults to true</param>
        /// <returns></returns>
        public string campaignContent(string cid, bool for_archive = true)
        {
            return makeCall("campaignContent", new Dictionary<string, string>() { { "cid", cid }, { "for_archive", for_archive.ToString() }});
        }


        #endregion

        /// <summary>
        /// This is the section that makes the call to the MailChimp servers
        /// </summary>
        /// <param name="method">The MailChimp method (not POST/GET)</param>
        /// <param name="parameters">The parameters</param>
        /// <returns></returns>
        private string makeCall(string method, Dictionary<string, string> parameters)
        {
            // Create a request using a URL that can receive a post.
            Stream dataStream;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_uri + "&method=" + method + "&" + getQueryString(parameters) + "&apikey=" + _apikey);
            request.UserAgent = "MCAPImini/" + _version;
            // Set the Method property of the request to POST.
            request.Method = "POST";
            // Create POST data and convert it to a byte array.
            string postData = getQueryString(parameters);
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            string json = "";
            // Get the response.
            try
            {
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                json = reader.ReadToEnd();
                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return json;
        }
        /// <summary>
        /// Creates the query string based off parameters passed in
        /// </summary>
        /// <param name="parameters">Dictionary of parameters</param>
        /// <returns>The serialized version of the QueryString</returns>
        string getQueryString(Dictionary<string, string> parameters)
        {
            var queryString = new StringBuilder();
            foreach (var i in parameters)
            {
                queryString.Append(String.Format("{0}={1}", i.Key, HttpUtility.UrlEncode(i.Value)) + "&");
            }
            return queryString.ToString().TrimEnd('&');
        }
    }

}