using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FDBK_IP : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // Display all query string keys and values for debugging
        //foreach (string key in Request.QueryString.AllKeys)
        //{
        //    Response.Write($"Key: {key}, Value: {Request.QueryString[key]}<br/>");
        //}

        string strid = Request.QueryString["IPNUM"];

        //string strid = "IP/250224/10122";


        if (string.IsNullOrEmpty(strid))
        {
            Response.Write("IPNumber is null or empty.");
        }
        else
        {
            Response.Write("IPNumber: " + strid);

            // Call the async method synchronously
           // string result = Get_appt_whatsappAsync(strid).GetAwaiter().GetResult();
            Task.Run(async () =>
            {
                string result = await Get_appt_whatsappAsync(strid);

                // Display the result (optional)
                Response.Write("<br/>Result: " + result);

            }).GetAwaiter().GetResult();
            // Display the result (optional)
          //  Response.Write("<br/>Result: " + result);
        }

    }



    public async Task<string> Get_appt_whatsappAsync(string strid)
    {
        string apiKey = "599a75f750ee49b6828f43e288aceae44c5162bf631a5b465a";

        // Define the API endpoint (replace with your actual endpoint URL)

        //string apiUrl = "http://172.16.1.20:57044/api/WhatsApp/SendMessage";

        // string apiUrl = "http://172.16.1.20:2503/api/WhatsApp/SendMessage";

        //string apiUrl = "http://172.16.1.20/Api_whatsapp/api/WhatsApp/SendMessage";

        //string apiUrl = "http://172.16.1.20/Api_whatsapp/api/WhatsApp/SendMessage_FDBK_IP";

        string apiUrl = "http://172.17.18.27/Api_whatsapp/api/WhatsApp/SendMessage_FDBK_IP";


        // Create the request body object. Adjust the property name based on your API's expected model.
        var requestBody = new
        {
            IPID = strid
        };

        // Serialize the object to JSON
        string jsonRequest = JsonConvert.SerializeObject(requestBody);

        // Create the StringContent to send in the POST request
        var content = new StringContent(jsonRequest, System.Text.Encoding.UTF8, "application/json");

        using (var client = new HttpClient())
        {
            // Add Authorization header
            client.DefaultRequestHeaders.Add("Authorization", apiKey);

            // Add Accept header
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Send the POST request asynchronously
                HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                // Check the response status and include the content in the result
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content
                    string responseContent = await response.Content.ReadAsStringAsync();


                    ClientScript.RegisterStartupScript(this.GetType(), "CloseWindow", "window.close();", true);

                    //Log or display the response content
                    Console.WriteLine("Response: " + responseContent);

                    //Return success message
                    return "Message Sent Successfully!";


                    //string script = "<script type='text/javascript'>alert('Message Sent Successfully!'); window.close();</script>";

                    //// Send the script to the client
                    //HttpContext.Current.Response.Write(script);
                    //HttpContext.Current.Response.End();

                }
                else
                {
                    // Capture the error response
                    string errorResponse = await response.Content.ReadAsStringAsync();

                    // Log the error response
                    Console.WriteLine("Error Response: " + errorResponse);

                    // Return the error reason
                    return "Failed to send message. Error: " + response.ReasonPhrase;
                }
            }
            catch (HttpRequestException ex)
            {
                // Capture and log HTTP request errors
                Console.WriteLine($"Request Error: {ex.Message}");
                return "Request failed: " + ex.Message;
            }
            catch (Exception ex)
            {
                // Capture any other exceptions
                Console.WriteLine($"General Error: {ex.Message}");
                return "Error: " + ex.Message;
            }
        }


        //Window close


    }


}