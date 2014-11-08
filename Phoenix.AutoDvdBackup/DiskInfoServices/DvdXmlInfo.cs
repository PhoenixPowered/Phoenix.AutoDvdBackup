using DirectShowLib;
using DirectShowLib.Dvd;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using Serilog;

namespace Phoenix.AutoDvdBackup.DiskInfoServices
{
    public class DvdXmlInfo : DvdInfo
    {
        private const string DvdIdRequestFormat = "<dvdXml><authentication><user>{0}</user><password>{1}</password></authentication><requests><request type=\"information\"><dvdId>{2}</dvdId></request></requests></dvdXml>";

        private readonly string _username;
        private readonly string _password;

        public DvdXmlInfo(ILogger logger)
            : base(logger)
        {
            _username = ConfigurationManager.AppSettings["dvdxml.username"];
            _password = ConfigurationManager.AppSettings["dvdxml.password"];
        }

        public override bool CanHandle(string diskIdentifier)
        {
            if(string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                return false;
            }

            return base.CanHandle(diskIdentifier);
        }

        public override DiskInfoData GetData(string diskIdentifier)
        {
            var dvdId = GetDvdId(diskIdentifier);
            var requestPayload = BuildRequestPayload(dvdId);
            var response = GetDvdXml(requestPayload);
            var title = GetTitleFromXml(response);
            var year = GetYearFromXml(response);

            return !string.IsNullOrEmpty(title) ? new DiskInfoData(title, year, Logger) : null;
        }

        private string GetTitleFromXml(string dvdXml)
        {
            Logger.Information("DvdXml Result: {0}", dvdXml);

            if(string.IsNullOrEmpty(dvdXml))
            {
                return null;
            }

            var document = new XmlDocument();
            document.LoadXml(dvdXml);

            var titles = document.GetElementsByTagName("title");
            
            if(titles.Count == 0)
            {
                return null;
            }

            return titles[0].InnerText;
        }

        private string GetYearFromXml(string dvdXml)
        {
            Logger.Information("DvdXml Result: {0}", dvdXml);

            if (string.IsNullOrEmpty(dvdXml))
            {
                return null;
            }

            var document = new XmlDocument();
            document.LoadXml(dvdXml);

            var titles = document.GetElementsByTagName("year");

            if (titles.Count == 0)
            {
                return null;
            }

            return titles[0].InnerText;
        }

        private string BuildRequestPayload(string dvdId)
        {
            if(string.IsNullOrEmpty(dvdId))
            {
                return null;
            }

            dvdId = string.Join(string.Empty, dvdId.Where(char.IsLetterOrDigit));

            return string.Format(DvdIdRequestFormat, _username, _password, dvdId);
        }

        private string GetDvdXml(string requestPayload)
        {
            Logger.Information("DvdXml Payload: {0}", requestPayload);

            if(string.IsNullOrEmpty(requestPayload))
            {
                return null;
            }

            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create("http://api.dvdxml.com");
            // Set the Method property of the request to POST.
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(requestPayload);
            // Set the ContentType property of the WebRequest.
            request.ContentType = "application/x-www-form-urlencoded";
            // Set the ContentLength property of the WebRequest.
            request.ContentLength = byteArray.Length;
            // Get the request stream.
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.
            dataStream.Close();
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            var reader = new StreamReader(dataStream);
            // Read the content.
            var responseFromServer = reader.ReadToEnd();
            // Display the content.
            //Console.WriteLine(responseFromServer);
            // Clean up the streams.
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }

        private string GetDvdId(string diskIdentifier)
        {
            if(string.IsNullOrEmpty(diskIdentifier))
            {
                return null;
            }

            var dvdVolume = diskIdentifier + "VIDEO_TS";

            string retval = string.Empty;

            try
            {
                IDvdGraphBuilder dvdGraphBuilder = (IDvdGraphBuilder)new DvdGraphBuilder();
                int hResult;

                // Build the DVD Graph
                AMDvdRenderStatus amDvdRenderStatus;
                hResult = dvdGraphBuilder.RenderDvdVideoVolume(dvdVolume,
                AMDvdGraphFlags.None, out amDvdRenderStatus);
                DsError.ThrowExceptionForHR(hResult);

                // Get the IDvDInfo2 interface
                object comObject;
                hResult = dvdGraphBuilder.GetDvdInterface(typeof(IDvdInfo2).GUID, out comObject);
                DsError.ThrowExceptionForHR(hResult);
                IDvdInfo2 dvdInfo2 = (IDvdInfo2)comObject;
                comObject = null;

                // Get the DVD ID.
                long discId;
                hResult = dvdInfo2.GetDiscID(dvdVolume, out discId);
                DsError.ThrowExceptionForHR(hResult);

                //  Print out the disc id if it's not 0:
                if (discId != 0)
                    retval = string.Format("{0,8:x}|{1,8:x}", (int)(discId >> 32), (int)(discId & 0xFFFFFFFF));
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Bad things happened while scanning the disc! {0}", ex.Message);
            }

            return retval;
        }
    }
}
