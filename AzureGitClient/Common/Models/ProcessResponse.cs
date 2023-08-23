﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGitClient.Models
{
    public class ProcessResponse
    {
        private const string LOG_DATE_TIME_STAMP_FORMAT = "MM/dd/yyyy hh:mm:ss tt";

        public bool IsSuccess { get; set; }
        public object ReturnedResponseObject { get; set; }
        public List<object> ReturnedResponseObjectList { get; set; }
        public List<string> ReturnedResponseList { get; set; }
        public string ReturnedResponseString { get; set; }
        public string ErrorDescription { get; set; }
        public string SystemError { get; set; }
        public string SuccessDescription { get; set; }
        public string WarningDescription { get; set; }
        private string log;
        public string Log
        {
            get
            {
                return log;
            }
            set
            {
                log += $"{value} @{DateTime.Now.ToString(LOG_DATE_TIME_STAMP_FORMAT)}\r\n";
            }
        }
        private string processDetails;
        public string ProcessDetails
        {
            get
            {
                return processDetails;
            }
            set
            {
                processDetails += $"{value}\r\n";
            }
        }

        public static string GetExceptionString(ref Exception oE)
        {
            StringBuilder oSB = new StringBuilder(300);
            oSB.Append("Error type of System.Exception occured:\r\n");
            oSB.Append("InnerException:\t" + oE.InnerException + "\r\n");
            oSB.Append("Message:\t" + oE.Message + "\r\n");
            oSB.Append("Source:\t" + oE.Source + "\r\n");
            oSB.Append("TargetSite:\t" + oE.TargetSite + "\r\n");
            oSB.Append("Stack:\t" + oE.StackTrace + "\r\n");
            return oSB.ToString();
        }
    }
}
