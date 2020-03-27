namespace QModManager.Utility
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    internal static class LogUploader
    {
        internal const string ServerURL = "https://qmodlogs.glitch.me";

        //internal static string Upload(string log, string previousLog = "")
        internal static HttpStatusCode Upload(string log, out string url)
        {
            url = "";
            if (!NetworkUtilities.IsOnline()) return HttpStatusCode.Unused;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ServerURL);

            string stringData = "log=" + Uri.EscapeDataString(log);
            //if (previousLog != null && previousLog.Length > 0) stringData += "&prev=" + Uri.EscapeDataString(previousLog);
            byte[] data = Encoding.ASCII.GetBytes(stringData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (Stream stream = request.GetRequestStream())
                stream.Write(data, 0, data.Length);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode != HttpStatusCode.OK) return response.StatusCode;

            url = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return HttpStatusCode.OK;
        }
    }
}
