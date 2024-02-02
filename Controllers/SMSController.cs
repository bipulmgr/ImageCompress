using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Xml;

namespace ImageCompressApi.Controllers;



[ApiController]
[Route("[controller]")]
public class SMSController : ControllerBase
{
    private readonly string _sparrowSMSUrl = "https://api.sparrowsms.com/v2/sms/";
    private readonly string _token = "v2_5rp1f0doKLOzKnJHX82UpTRIQdE.tpE6"; // sparrow SMS token
    private readonly string _from = "Demo"; // identity


    [HttpPost("SendOTP")]
    public async Task<IActionResult> SendOTP([FromBody] string to)
    {
        try
        {
            var generator = new Random();
            var r = generator.Next(0, 1000000).ToString("D6");
            var text = $"Your login verify OTP code for Aloe Herbal is {r}";
            //var response = PostSendSMS(_from, _token, to, text);
            var response = SendSMS(to, text);
            return Ok(response);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private string PostSendSMS(string from, string token, string to, string text)
    {
        try
        {
            using (var client = new WebClient())
            {
                var values = new NameValueCollection();
                values["from"] = from;
                values["token"] = token;
                values["to"] = to;
                values["text"] = text;
                var response = client.UploadValues("https://api.sparrowsms.com/v2/sms/", "Post", values);
                var responseString = Encoding.Default.GetString(response);
                return responseString;
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }

    }

    private static string GetSendSMS(string from, string token, string to, string text)
    {
        try
        {
            using (var client = new WebClient())
            {
                string parameters = "?";
                parameters += "from=" + from;
                parameters += "&to=" + to;
                parameters += "&text=" + text;
                parameters += "&token=" + token;
                var responseString = client.DownloadString("https://api.sparrowsms.com/v2/sms/" + parameters);
                return responseString;
            }
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    private async Task<string> SendSMS(string to, string text)
    {
        try
        {
            var client = new RestClient(_sparrowSMSUrl);
            var request = new RestRequest(_sparrowSMSUrl, Method.Post);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("token", _token);
            request.AddParameter("from", _from);
            request.AddParameter("to", to);
            request.AddParameter("text", text);

            var response = await client.PostAsync(request).ConfigureAwait(false);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = JsonConvert.DeserializeObject<ResponseModel>(response.Content);
                // Check whether the response code is success or not
                return result.Response;

            }
            else
            {
                return response.ErrorMessage;
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }



    private class ResponseModel
    {
        public string Response { get; set; }
        public int Code { get; set; }
        public int Count { get; set; }
    }
}
