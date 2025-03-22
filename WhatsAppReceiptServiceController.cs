using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WhatsappWebapi.Services; // Import your service namespace

using System;


namespace WhatsappWebapi.Controllers
{

    //[RoutePrefix("api/whatsapp")]
    public class WhatsAppReceiptServiceController : ApiController
    {

        private readonly WhatsAppReceiptService _whatsAppApiService;
        private readonly WhatsAppMessageRequest _whatsAppMessageRequest;
        private readonly ReceiptService _ReceiptService;

        // Injecting Service into the Controller
        public WhatsAppReceiptServiceController()
        {
            _whatsAppApiService = new WhatsAppReceiptService();
            _whatsAppMessageRequest = new WhatsAppMessageRequest();
            _ReceiptService = new ReceiptService();



        }




        public async Task<IHttpActionResult> SendMessage([FromBody] ReceiptRequestquery request)
        {
            if (request == null)
                return BadRequest("Invalid appointment ID.");


            var ReceiptData = _ReceiptService.GetReciptWithPdf(request.ReceiptID);
            if (ReceiptData == null)
                return BadRequest("Appointment not found.");
            //string fullAddress = $"{appointmentData.HospitalAddress}, {appointmentData.HospitalAddress1} - {appointmentData.HospitalPIN}";
            //string Reportingdatetime = $"{appointmentData.AppointmentDate}, {appointmentData.Appointmenttime}";
            //string googleMapsUrl = GenerateGoogleMapsLink(fullAddress);
            //string Appointmenturl = await _whatsAppApiService.ShortenUrl(googleMapsUrl);

            // Construct the URL (PDFs are in the root folder)



            //string pdfUrl = $"https://snmail.in/FileUploadHandler/uploads/WhatsApp/{appointmentData.AppointmentPDF}";

            //var messageRequest = new WhatsAppMessageRequest
            //{
            //    MessagingProduct = "whatsapp",
            //    RecipientType = "individual",
            //    To = ReceiptData.Phonenumber /*"9840364189"*/,
            //    Type = "template",

            //    Template = new Template
            //    {
            //        Name = appointmentData.TemplateName,
            //        Language = new Language { Code = "en" },
            //        Components = new List<Component>
            //{
            //    new Component
            //    {
            //        Type = "header",
            //        Parameters = new List<Parameter>
            //        {
            //            new Parameter
            //            {
            //                Type = "document", // ✅ Correct type for document attachment
            //                Document = new WhatsappDocument
            //                {
            //                    Link = pdfUrl, // ✅ Publicly accessible PDF link
            //                    FileName = Path.GetFileName(appointmentData.AppointmentPDF) // ✅ Extracted file name
            //                }
            //            }
            //        }
            //    },
            //    new Component
            //    {
            //        Type = "body",
            //        Parameters = new List<Parameter>
            //        {
            //            new Parameter { Type = "text", Text = appointmentData.Location_desc },
            //            new Parameter { Type = "text", Text = fullAddress },
            //            new Parameter { Type = "text", Text = appointmentData.DoctorName },
            //            new Parameter { Type = "text", Text = appointmentData.PatientName },
            //            new Parameter { Type = "text", Text = appointmentData.AppointmentReference },
            //            new Parameter { Type = "text", Text = appointmentData.AppointmentNumber},
            //            new Parameter { Type = "text", Text = Reportingdatetime },
            //            new Parameter { Type = "text", Text = appointmentData.serviceamount },
            //               new Parameter { Type = "text", Text = appointmentData.Contact_no },
            //            new Parameter { Type = "text", Text = Appointmenturl }
            //        }
            //    }
            //}
            //    }
            //};
            //    var messageRequest = new WhatsAppMessageRequest
            //    {
            //        MessagingProduct = "whatsapp",
            //        RecipientType = "individual",
            //        To = appointmentData.PatientPhone  /*"9840364189"*/,
            //        Type = "template",

            //        Template = new Template
            //        {
            //            Name = appointmentData.TemplateName,
            //            Language = new Language { Code = "en" },
            //            Components = new List<Component>
            //{
            //    new Component
            //    {
            //        Type = "header",
            //        Parameters = new List<Parameter>
            //        {
            //            new Parameter
            //            {
            //                Type = "document", // ✅ Correct type for document attachment
            //                Document = new WhatsappDocument
            //                {
            //                    Link = pdfUrl, // ✅ Publicly accessible PDF link
            //                    FileName = Path.GetFileName(appointmentData.AppointmentPDF) // ✅ Extracted file name
            //                }
            //            }
            //        }
            //    }

            //}
            //        }
            //    };



       //     string json = JsonConvert.SerializeObject(messageRequest);
       //     var response = await _whatsAppApiService.SendWhatsAppMessage(json);
       //     if (string.IsNullOrEmpty(response) || response.Contains("Error"))
       //         return BadRequest($"WhatsApp API Error: {response}");

       //     var deserializedResponse = JsonConvert.DeserializeObject<WhatsAppApiResponse>(response);

       ////     _whatsAppApiService.SaveDeserializedResponse(deserializedResponse, request.AppointmentId);
       //     var status = deserializedResponse.Status;
       //     //return Ok(response);

       //     return Redirect("http://localhost:55760/WebForm1.aspx?status=" + status);

            return Redirect("http://localhost:55760/WebForm1.aspx?status=" );




        }















    }

}
