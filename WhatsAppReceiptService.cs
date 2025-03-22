using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;

namespace WhatsappWebapi.Services
{
    public class WhatsAppReceiptService
    {
        private readonly HttpClient _httpClient;
        private readonly string _connectionString;
        //private readonly string FTPSERVER = ConfigurationManager.AppSettings["FTPSERVER"].ToString();
        //private readonly string UPDUSERDOMAIN = ConfigurationManager.AppSettings["FTPDOMAIN"].ToString();
        //private readonly string UPDUSERID = ConfigurationManager.AppSettings["FTPUSERID"].ToString();
        //private readonly string UPDUSERPASS = ConfigurationManager.AppSettings["FTPPASSWORD"].ToString();
        public WhatsAppReceiptService()
        {
            _httpClient = new HttpClient();
            _connectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ConnectionString;
        }

    }

    public class ReceiptService
    {
        private readonly string _connectionString;

        public ReceiptService()
        {
            // Get the connection string from Web.config
            _connectionString = ConfigurationManager.ConnectionStrings["SQLDbConnection"].ConnectionString;
        }
        public async Task<ReceiptRequest> GetReciptWithPdf(string ReceiptId)
        {
            // Fetch Receipt details
            ReceiptRequest Receipt = GetReciptData(ReceiptId);

            if (Receipt != null)
            {
                // Generate PDF separately
                Receipt.ReceiptPDF = await GenerateReceiptPdf(Receipt);
            }

            return Receipt;
        }

        public async Task<string> GenerateReceiptPdf(ReceiptRequest Receipt)
        {
            string pdfurl = Receipt.ReceiptPDF;

     
            string FILEURL = ConfigurationManager.AppSettings["FileuploadURL"].ToString();
            string FTPUSERNAME = ConfigurationManager.AppSettings["FTPUSERID"].ToString();
            string FTPPASSWORD = ConfigurationManager.AppSettings["FTPPASSWORD"].ToString();

          
            string remoteFileUrl = pdfurl.Replace(@"\\FILEUPLOADURL", FILEURL).Replace(@"\", "/");

           

            string uploadUrl = "https://snmail.in/FileUploadHandler/upload.ashx";

            return await UploadPdfToServerAsync(remoteFileUrl, uploadUrl,FTPUSERNAME,FTPPASSWORD);
        }
        public static async Task<byte[]> DownloadFileFromFtpAsync(string ftpUrl, string ftpUsername, string ftpPassword)
        {
            using (WebClient ftpClient = new WebClient())
            {
           
                ftpClient.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                try
                {
                   
                    byte[] fileBytes = await ftpClient.DownloadDataTaskAsync(new Uri(ftpUrl));
                    return fileBytes;
                }
                catch (WebException ex)
                {
                    Console.WriteLine($"FTP Download Failed: {ex.Message}");
                    return null;
                }
            }
        }


        public static async Task<string> UploadPdfToServerAsync(string remoteFileUrl, string uploadUrl,string ftpusername,string ftppassword)
        {
            using (HttpClient httpClient = new HttpClient())
            {
             
                //remoteFileUrl = remoteFileUrl.Replace("://", "TEMP").Replace("//", "/").Replace("TEMP", "://");
                try
                {
                   
                    byte[] fileBytes;
                    using (HttpClient downloadClient = new HttpClient())
                    {
                        fileBytes = await DownloadFileFromFtpAsync(remoteFileUrl, ftpusername, ftppassword);
                    }

                    if (fileBytes == null || fileBytes.Length == 0)
                    {
                        return "Failed to download the file.";
                    }

                    // 🔹 Step 2: Get the filename from the URL
                    string pdfFileName = Path.GetFileName(new Uri(remoteFileUrl).LocalPath);

                    using (var content = new MultipartFormDataContent())
                    {
                       
                        var fileContent = new ByteArrayContent(fileBytes);
                        fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

                    
                        content.Add(fileContent, "file", pdfFileName);

                        HttpResponseMessage response = await httpClient.PostAsync(uploadUrl, content).ConfigureAwait(false); ;

                     
                        if (response.IsSuccessStatusCode)
                        {
                            return $"{pdfFileName}";
                        }
                        else
                        {
                            return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    return $"Error: {ex.Message}";
                }
            }
        }




        //public byte[] DownloadFileFromFTP(string ftpUrl, string username, string password)
        //{
        //    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
        //    request.Method = WebRequestMethods.Ftp.DownloadFile;
        //    request.Credentials = new NetworkCredential(username, password);

        //    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        //    using (Stream responseStream = response.GetResponseStream())
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        responseStream.CopyTo(memoryStream);
        //        return memoryStream.ToArray();
        //    }
        //}



        //public string UploadPdfToServer(string filePath, string uploadUrl)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        using (var content = new MultipartFormDataContent())
        //        {
        //            byte[] fileBytes = File.ReadAllBytes(filePath);
        //            var fileContent = new ByteArrayContent(fileBytes);
        //            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        //            string pdfFileName = Path.GetFileName(filePath);
        //            content.Add(fileContent, "file", pdfFileName);

        //            HttpResponseMessage response = client.PostAsync(uploadUrl, content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                return $"{pdfFileName}";
        //            }
        //            else
        //            {
        //                return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
        //            }
        //        }
        //    }
        //}

        //public string UploadPdfToServer(string filePath, string uploadUrl)
        //{
        //    using (HttpClient client = new HttpClient())
        //    {
        //        using (var content = new MultipartFormDataContent())
        //        {
        //            byte[] fileBytes = File.ReadAllBytes(filePath);
        //            var fileContent = new ByteArrayContent(fileBytes);
        //            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        //            string pdfFileName = Path.GetFileName(filePath);
        //            content.Add(fileContent, "file", pdfFileName);

        //            HttpResponseMessage response = client.PostAsync(uploadUrl, content).Result;

        //            if (response.IsSuccessStatusCode)
        //            {
        //                return $"{pdfFileName}";
        //            }
        //            else
        //            {
        //                return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
        //            }
        //        }
        //    }
        //}

        //public static async Task<string> UploadPdfToServerAsync(string remoteFileUrl, string uploadUrl)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        try
        //        {
        //            // 🔹 Step 1: Download the PDF file from the remote server
        //            byte[] fileBytes;
        //            using (HttpClient downloadClient = new HttpClient())
        //            {
        //                fileBytes = await downloadClient.GetByteArrayAsync(remoteFileUrl);
        //            }

        //            if (fileBytes == null || fileBytes.Length == 0)
        //            {
        //                return "Failed to download the file.";
        //            }

        //            // 🔹 Step 2: Get the filename from the URL
        //            string pdfFileName = Path.GetFileName(new Uri(remoteFileUrl).LocalPath);

        //            using (var content = new MultipartFormDataContent())
        //            {
        //                // 🔹 Step 3: Convert byte array into HTTP content
        //                var fileContent = new ByteArrayContent(fileBytes);
        //                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");

        //                // 🔹 Step 4: Attach the file to form data
        //                content.Add(fileContent, "file", pdfFileName);

        //                // 🔹 Step 5: Upload the file
        //                HttpResponseMessage response = await httpClient.PostAsync(uploadUrl, content);

        //                // 🔹 Step 6: Handle response
        //                if (response.IsSuccessStatusCode)
        //                {
        //                    return $"{pdfFileName} uploaded successfully!";
        //                }
        //                else
        //                {
        //                    return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return $"Error: {ex.Message}";
        //        }
        //    }
        //}

        //public string UploadPdfToServer(string filePath, string uploadUrl)
        //{
        //    try
        //    {
        //        // Check if the file path is an FTP URL
        //        if (filePath.StartsWith("ftp:\\"))
        //        {
        //            // Download the file from the FTP server first
        //            string localFilePath = DownloadFileFromFtp(filePath);

        //            // Now read the file bytes from the local path
        //            byte[] fileBytes = File.ReadAllBytes(localFilePath);

        //            // Delete the local file after reading (if desired)
        //            File.Delete(localFilePath);

        //            using (HttpClient client = new HttpClient())
        //            {
        //                using (var content = new MultipartFormDataContent())
        //                {
        //                    var fileContent = new ByteArrayContent(fileBytes);
        //                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        //                    string pdfFileName = Path.GetFileName(filePath);
        //                    content.Add(fileContent, "file", pdfFileName);

        //                    HttpResponseMessage response = client.PostAsync(uploadUrl, content).Result;

        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        return $"{pdfFileName}";
        //                    }
        //                    else
        //                    {
        //                        return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // Handle the case where the filePath is not FTP, but a local path
        //            byte[] fileBytes = File.ReadAllBytes(filePath);

        //            using (HttpClient client = new HttpClient())
        //            {
        //                using (var content = new MultipartFormDataContent())
        //                {
        //                    var fileContent = new ByteArrayContent(fileBytes);
        //                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
        //                    string pdfFileName = Path.GetFileName(filePath);
        //                    content.Add(fileContent, "file", pdfFileName);

        //                    HttpResponseMessage response = client.PostAsync(uploadUrl, content).Result;

        //                    if (response.IsSuccessStatusCode)
        //                    {
        //                        return $"{pdfFileName}";
        //                    }
        //                    else
        //                    {
        //                        return $"Upload failed: {response.StatusCode} - {response.ReasonPhrase}";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return $"Error uploading file: {ex.Message}";
        //    }
        //}

        //private string DownloadFileFromFtp(string ftpUrl)
        //{
        //    try
        //    {
        //        // Get the file name from the URL
        //        string fileName = Path.GetFileName(ftpUrl);

        //        // Define the local file path to store the downloaded file
        //        string localFilePath = Path.Combine(@"C:\test\", fileName);

        //        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
        //        request.Method = WebRequestMethods.Ftp.DownloadFile;
        //        string UPDUSERID = ConfigurationManager.AppSettings["FTPUSERID"].ToString();
        //        string UPDUSERPASS = ConfigurationManager.AppSettings["FTPPASSWORD"].ToString();
        //        // Provide credentials if required
        //        request.Credentials = new NetworkCredential(UPDUSERID, UPDUSERPASS);
        //        request.UsePassive = true;
        //        request.UseBinary = true;
        //        request.KeepAlive = false;

        //        using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
        //        using (Stream responseStream = response.GetResponseStream())
        //        using (FileStream fs = new FileStream(localFilePath, FileMode.Create))
        //        {
        //            responseStream.CopyTo(fs);
        //        }

        //        // Return the local file path
        //        return localFilePath;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Error downloading file from FTP: {ex.Message}");
        //    }
        //}

        public ReceiptRequest GetReciptData(string ReceiptId)
        {
            ReceiptRequest Receipt = null;

         
            string query = @"Select MPDD_PDF_NAME,MPDD_MOBILE  from MR_PRINT_DOCUMENT_DETAILS where  MPDD_ID=@ReceiptId";

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ReceiptId", ReceiptId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                // PatientPhone = reader["APD_CONTACT_INFO_DTLS"].ToString(),

                if (reader.Read())
                {
                    Receipt = new ReceiptRequest
                    {
                        ReceiptPDF = reader["MPDD_PDF_NAME"].ToString(),
                        Phonenumber = reader["MPDD_MOBILE"].ToString(),
                      
                        //TemplateName = "appointment_confirmation_temp",

                    };
                }
            }

            return Receipt;





        }

    }
}