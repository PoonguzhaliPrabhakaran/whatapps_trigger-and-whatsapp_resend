using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Configuration;

namespace whatsapptrigger
{
    public partial class Appointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Display all query string keys and values for debugging
            foreach (string key in Request.QueryString.AllKeys)
            {
                Response.Write($"Key: {key}, Value: {Request.QueryString[key]}<br/>");
            }

            string strappid = Request.QueryString["strappid"];
            if (string.IsNullOrEmpty(strappid))
            {
                Response.Write("strappid is null or empty.");
            }
            else
            {
                Response.Write("strappid: " + strappid);

                // Call the async method without GetAwaiter().GetResult() to avoid deadlock
                Task.Run(async () =>
                {
                    string result = await Get_appt_whatsappAsync(strappid);

                    //// Display the result (optional)
                    //Response.Write("<br/>Result: " + result);
                    // Display the result (optional)
                    Response.Write("<br/>Result: Message sent successfully " );
                    // Delay execution before calling GetSMS
                    await Task.Delay(TimeSpan.FromSeconds(5)); // 10-second delay

                  
                }).GetAwaiter().GetResult();
                GetSMS(strappid);

            }
        }

        public async Task<string> Get_appt_whatsappAsync(string strappid)
        {
            string apiKey = "599a75f750ee49b6828f43e288aceae44c5162bf631a5b465a";

            // Define the API endpoint (replace with your actual endpoint URL)
            //string apiUrl = "http://172.16.1.39:52755/api/WhatsApp/SendMessage";

            string apiUrl = "http://localhost:52755/api/WhatsApp/SendMessage";
            //string apiUrl = "http://172.17.18.27/Webapipublish/api/WhatsApp/SendMessage";

           // string apiUrl = "http://172.16.1.10/Webapipublish/api/WhatsApp/SendMessage";



            // Create the request body object. Adjust the property name based on your API's expected model.
            var requestBody = new
            {
                AppointmentId = strappid
            };

            // Serialize the object to JSON
            string jsonRequest = JsonConvert.SerializeObject(requestBody);

            // Create the StringContent to send in the POST request
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

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
                        string responseContent = await response.Content.ReadAsStringAsync();

                        //// Log or display the response content
                        //Response.Write("<br/>Response Content: " + responseContent);

                        // Return success message with response content
                        return "Message Sent Successfully! Response: " + responseContent;
                    }
                    else
                    {
                        // Capture the error response
                        string errorResponse = await response.Content.ReadAsStringAsync();

                        //// Log the error response
                        //Response.Write("<br/>Error Response: " + errorResponse);

                        // Return the error reason
                        return "Failed to send message. Error: " + response.ReasonPhrase + " - " + errorResponse;
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Capture and log HTTP request errors
                    //Response.Write($"<br/>Request Error: {ex.Message}");
                    return "Request failed: " + ex.Message;
                }
                catch (Exception ex)
                {
                    // Capture any other exceptions
                    //Response.Write($"<br/>General Error: {ex.Message}");
                    return "Error: " + ex.Message;
                }
            }
        }
        public string GetSMS(string strappid)
        {
            // Define connection string
            string sConn1 = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ConnectionString;

            // Use using statement for SqlConnection
            using (SqlConnection con = new SqlConnection(sConn1))
            {
                // Open the connection
               

                // Define the query
                string sqluptquery = @"SELECT wwbh_errorcode 
                               FROM snlive.dbo.WhatsAppWebhookLogs 
                               INNER JOIN SN_WhatsAppMessageResponse_register 
                               ON WA_Message_WAMId = WWBH_MessageId 
                               AND WA_Contact_WA_Id = WWBH_RecipientId 
       
                               WHERE Appointment_Id = @ApptID 
                             
                               AND wwbh_errorcode <> 0;";

                // Initialize the command with connection
                using (SqlCommand cmd = new SqlCommand(sqluptquery, con))
                {
                    // Parameterized query to prevent SQL Injection
                    cmd.Parameters.AddWithValue("@ApptID", strappid);
                    con.Open();
                    // Use SqlDataAdapter with the initialized command
                    using (SqlDataAdapter Da1 = new SqlDataAdapter(cmd))
                    {
                        DataSet watDS1 = new DataSet();
                        Da1.Fill(watDS1);

                        // Check if any rows are returned
                        if (watDS1.Tables[0].Rows.Count >= 1)
                        {
                            string rtnresult = watDS1.Tables[0].Rows[0]["wwbh_errorcode"].ToString();

                            // Encode the Appointment ID for URL
                            string encodedID = HttpUtility.UrlEncode(strappid);

                            // Construct the URL
                            string url = "http://172.17.18.24/AppoinmentSMSTIMES/appointment_SMS.aspx?strappid=" + encodedID;

                            // JavaScript to open a new window
                            string script = $"<script type='text/javascript'>window.open('{url}', '_blank', 'toolbar=no,width=20,height=20');</script>";

                            // Ensure HttpContext is available
                            if (HttpContext.Current != null)
                            {
                                HttpContext.Current.Response.Write(script);
                            }
                            else
                            {
                                return "HttpContext is not available";
                            }
                        }
                        else
                        {
                            return "No error codes found";
                        }
                    }
                }

                // Close connection (optional since using statement handles it)
                con.Close();
                return "Completed";
            }
        }


    }
}
