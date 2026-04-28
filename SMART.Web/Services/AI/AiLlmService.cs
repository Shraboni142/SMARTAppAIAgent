using System;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using SMART.Web.Models.AI;

namespace SMART.Web.Services.AI
{
    public interface IAiLlmService
    {
        string AskAi(AiAgentSetting setting, string userMessage);
    }

    public class AiLlmService : IAiLlmService
    {
        public string AskAi(AiAgentSetting setting, string userMessage)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);

                    var url = setting.ApiBaseUrl.TrimEnd('/') + "/chat/completions";

                    var requestBody = new
                    {
                        model = setting.ModelName,
                        messages = new[]
{
    new
    {
        role = "system",
        content = "You are Smart ERP AI Assistant. Answer professionally. If the question is ERP related, explain based on HRM, Employee, Complain, Attendance, Finance, Inventory, and Admin module context. Do not expose confidential data unless user has permission."
    },
    new
    {
        role = "user",
        content = userMessage
    }
}
                    };

                    var json = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    if (!string.IsNullOrEmpty(setting.ApiKey))
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + setting.ApiKey);
                    }

                    var response = client.PostAsync(url, content).Result;

                    var responseString = response.Content.ReadAsStringAsync().Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return "API Error: " + responseString;
                    }

                    dynamic result = JsonConvert.DeserializeObject(responseString);

                    return result.choices[0].message.content.ToString();
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }
    }
}