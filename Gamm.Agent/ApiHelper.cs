using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

namespace Gamm.Agent
{
    internal static class ApiHelper
    {
        public static string URL;
        public static string TOKEN;
        private const string CONTROLLER_NAME = "api/agentapi";

        internal static void AddAgentMain(Models.AgentMainModel model)
        {
            var response = Call("AgentMain", model);
            if (response.Data != null)
            {
                model.Id = new Guid(response.Data.ToString());
            }
        }

        internal static void AddAgentPart(IEnumerable<Models.AgentPartModel> model)
        {
            Call("AgentPart", model);
        }

        internal static void AddAgentSoftware(IEnumerable<Models.AgentSoftwareModel> model)
        {
            Call("AgentSoftware", model);
        }

        internal static bool ShouldScan(Models.AgentMainModel model)
        {
            var response = Call("ShouldScan", model);
            if (response.Data != null
                && (bool)response.Data == true) return true;
            return false;
        }

        internal static void SetConflict(Models.AgentMainModel model)
        {
            Call("SetConflict", model);
        }

        private static Models.AjaxResponse Call<T>(string methodName, T model)
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls;

            // Optional: SSL certificate validation callback (only if needed)
            //ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertificate;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
                var fullUrl = $"{URL}/{CONTROLLER_NAME}/{methodName}";
                client.Headers.Add("content-type", "application/json");
                client.Headers.Add("token", TOKEN);

                string requestData = Serialize(model);
                byte[] responseData = client.UploadData(fullUrl, "POST", Encoding.UTF8.GetBytes(requestData));
                string responseString = Encoding.ASCII.GetString(responseData);
                var response = Deserialize<Models.AjaxResponse>(responseString);
                return response;
            }
        }

        private static T Deserialize<T>(string json)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            T model = js.Deserialize<T>(json);
            return model;
        }

        private static string Serialize<T>(T model)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string result = js.Serialize(model);
            return result;
        }
    }
}
