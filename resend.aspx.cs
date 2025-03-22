using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace whatsapp_resend
{
    public partial class resend : System.Web.UI.Page
    {
        private static readonly string apiKey = "599a75f750ee49b6828f43e288aceae44c5162bf631a5b465a";
        private static readonly string apiUrl = "http://172.17.18.27/Webapipublish/api/WhatsApp/SendMessage";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Page.RegisterAsyncTask(new PageAsyncTask(async () => await ExecuteTask()));

            }
          
            string closeScript = "<script language='javascript'> window.close() </script>";

            Literal1.Text = closeScript;
        }

        private async Task ExecuteTask()
        {
            string appt_log = "";

            string connectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"]?.ConnectionString;
            if (string.IsNullOrEmpty(connectionString))
            {
                Console.WriteLine("Database connection string is missing.");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //string query = @"
                //    SELECT hum_user_name, a.apd_appointment_id 
                //    FROM AP_APPOINTMENT_sms a
                //    LEFT OUTER JOIN as_user_master ON apd_crt_uid = hum_user_id  
                //    WHERE APD_SOURCE_INFO <> 'ONLN' 
                //    AND CONVERT(VARCHAR(10), APD_CRT_DT, 121) = CONVERT(VARCHAR(10), GETDATE(), 121) 

                //    AND APD_APPOINTMENT_ID NOT IN (SELECT Appointment_Id FROM SN_WhatsAppMessageResponse_register) 
                //    ORDER BY hum_user_name, APD_CONSULTATION_DT";

                //                string query = @"

                // select top 100  APD_APPOINTMENT_ID from AP_APPOINTMENT_DTLS where CONVERT(Varchar(10),apd_crt_dt,121)='2025-03-04' and apd_appointment_status = 'ASBO'
                // and DAY(APD_CONSULTATION_dt)<=12 and CONVERT(Varchar(10),APD_CONSULTATION_dt,121)<>'2025-03-04'  AND APD_APPOINTMENT_ID NOT IN (SELECT Appointment_Id FROM SN_WhatsAppMessageResponse_register)
                //AND APD_APPOINTMENT_ID IN (SELECT Appointment_Id FROM dmr_SN_WhatsAppMessageResponse_register)
                //order by APD_CONSULTATION_dt, apd_crt_dt ";
//                string query = @"

//                  select TOP 100 APD_APPOINTMENT_ID from AP_APPOINTMENT_DTLS where CONVERT(Varchar(10),apd_crt_dt,121)='2025-03-01'
//  and CONVERT(Varchar(10),APD_CONSULTATION_dt,121)>  '2025-03-04' 
// and APD_APPOINTMENT_ID NOT in (select appointment_id from SN_WhatsAppMessageResponse_register)  AND APD_APPOINTMENT_ID  in (select appointment_id from DMR_SN_WhatsAppMessageResponse_register)
// and APD_CRT_DT<='2025-03-01 16:03:25.620'
// and apd_appointment_status='asbo' 
//order by APD_CRT_DT";

//                string query = @"

//                    select top 100 APD_APPOINTMENT_ID from  SN_WhatsAppMessageResponse_register_notsent04_03_2025  where 
//   APD_APPOINTMENT_ID not  IN  
//(select appointment_id from SN_WhatsAppMessageResponse_register) ";

//                string query = @"

//                    select APD_APPOINTMENT_ID from AP_APPOINTMENT_SMS where CONVERT(Varchar(10),apd_crt_dt,121)='2025-03-04' AND 
//   APD_APPOINTMENT_ID NOT IN  
//(select appointment_id from SN_WhatsAppMessageResponse_register)and CONVERT(Varchar(10),APD_CONSULTATION_dt,121)>  '2025-03-04' 
//  AND APD_SOURCE_INFO <>'ONLN'
// ORDER BY APD_CRT_DT ";


                string query = @"
				select APD_APPOINTMENT_ID from     AP_APPOINTMENT_sms where    convert(varchar(10),APD_CRT_DT,121)=convert(varchar(10),getdate(),121)
 AND APD_SOURCE_INFO <>'ONLN'
 and APD_APPOINTMENT_ID NOT IN (SELECT Appointment_Id FROM SN_WhatsAppMessageResponse_register ) order by APD_CONSULTATION_DT ";



                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    cmd.CommandTimeout = 180;
                    await conn.OpenAsync();
                    using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                    {
                        using (var client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", apiKey);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            while (await reader.ReadAsync())
                            {
                                string appointmentId = reader["apd_appointment_id"].ToString();
                                appt_log += "','" + appointmentId;

                                // Create JSON request body
                                var requestBody = new { AppointmentId = appointmentId };
                                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                             
                                try
                                {
                                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                                    string responseContent = await response.Content.ReadAsStringAsync();
                                    
                                    if (response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine($"Message Sent Successfully! Response: {responseContent}");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Failed to send message. Error: {response.ReasonPhrase} - {responseContent}");
                                    }
                                }
                                catch (HttpRequestException ex)
                                {
                                    Console.WriteLine($"Request failed: {ex.Message}");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error: {ex.Message}");
                                }
                            }
                        }
                    }
                }
                conn.Close();
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