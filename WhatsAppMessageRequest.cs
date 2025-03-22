using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WhatsappWebapi.Services
{

    //public class WhatsAppMessageRequest
    //{
    //    public string MessagingProduct { get; set; }
    //    public string MessageId { get; set; }
    //    public string RecipientType { get; set; }
    //    public string To { get; set; }
    //    public string Type { get; set; }
    //    public Template Template { get; set; }
    //}

    //public class Template
    //{
    //    public string Name { get; set; }
    //    public Language Language { get; set; }
    //}

    //public class Language
    //{
    //    public string Code { get; set; }
    //}



    using Newtonsoft.Json;

    //public class WhatsAppMessageRequest
    //{
    //    [JsonProperty("messaging_product")]
    //    public string MessagingProduct { get; set; }

    //    [JsonProperty("message_id")]
    //    public string MessageId { get; set; }

    //    [JsonProperty("recipient_type")]
    //    public string RecipientType { get; set; }

    //    [JsonProperty("to")]
    //    public string To { get; set; }

    //    [JsonProperty("type")]
    //    public string Type { get; set; }

    //    [JsonProperty("template")]
    //    public Template Template { get; set; }
    //}

    //public class Template
    //{
    //    [JsonProperty("name")]
    //    public string Name { get; set; }

    //    [JsonProperty("language")]
    //    public Language Language { get; set; }
    //}

    //public class Language
    //{
    //    [JsonProperty("code")]
    //    public string Code { get; set; }
    //}
    //public class WhatsAppMessageRequest
    //{
    //    public string MessagingProduct { get; set; }
    //    public string RecipientType { get; set; }
    //    public string To { get; set; }
    //    public string Type { get; set; }
    //    public Template Template { get; set; }
    //}

    //public class Template
    //{
    //    public string Name { get; set; }
    //    public Language Language { get; set; }
    //    public List<Component> Components { get; set; }
    //}

    //public class Language
    //{
    //    public string Code { get; set; }
    //}

    //public class Component
    //{
    //    public string Type { get; set; }
    //    public List<Parameter> Parameters { get; set; }
    //}

    //public class Parameter
    //{
    //    public string Type { get; set; }
    //    public string Text { get; set; }
    //}


    public class WhatsAppMessageRequest
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonProperty("recipient_type")]
        public string RecipientType { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("template")]
        public Template Template { get; set; }

        
    }

    public class Template
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public Language Language { get; set; }

        [JsonProperty("components")]
        public List<Component> Components { get; set; }
    }
    public class WhatsappDocument
    {
        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("filename")]
        public string FileName { get; set; } // Name of the document file
    }

    public class Language
    {
        [JsonProperty("code")]
        public string Code { get; set; }
    }

    public class Component
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "header"; // "body" is usually required

        [JsonProperty("parameters")]
        public List<Parameter> Parameters { get; set; }
    }

    public class Parameter
    {
        [JsonProperty("type")]
        public string Type { get; set; } = "document"; // "text" for string parameters

        [JsonProperty("document")]
        public WhatsappDocument Document { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }
    }



    public class AppointmentData
    {
        public string HospitalAddress { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public string PatientPhone { get; set; }
        public string AppointmentNumber { get; set; }
        public string AppointmentDate { get; set; }
        public string AppointmentReference { get; set; }
        public string AppointmentType { get; set; }
        public string TemplateName { get; set; }
        public string AppointmentPDF { get; set; }
        public string HospitalAddress1 { get;  set; }
        public string AppointmentLocation { get;  set; }
        public string HospitalPIN { get;  set; }
        public string serviceamount { get; set; }
        public string HospitalEmail { get;  set; }
        public string Appointmenttime { get;  set; }
        public string Location_desc { get;  set; }
        public string Contact_no { get;  set; }
    }


    public class AppointmentRequest
    {
        public string AppointmentId { get; set; }
    }

    public class WhatsAppApiResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("message_id")]
        public string MessageId { get; set; }

        [JsonProperty("code")]
        public int Code { get; set; }

        [JsonProperty("res_json")]
        public ResJson ResJson { get; set; }
    }

    public class ResJson
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; set; }

        [JsonProperty("contacts")]
        public List<Contact> Contacts { get; set; }

        [JsonProperty("messages")]
        public List<Message> Messages { get; set; }


    }

    public class Contact
    {
        [JsonProperty("input")]
        public string Input { get; set; }

        [JsonProperty("wa_id")]
        public string WaId { get; set; }
    }

    public class Message
    {
        [JsonProperty("id")]
        public string WAMId { get; set; }

        [JsonProperty("message_status")]
        public string MessageStatus { get; set; }
    }

    public class ReceiptRequestquery
    {
        public string ReceiptID { get; set; }
    }

        public class ReceiptRequest
    {
       
        public string ReceiptPDF { get; set; }
        public string Phonenumber { get; set; }
    }









}