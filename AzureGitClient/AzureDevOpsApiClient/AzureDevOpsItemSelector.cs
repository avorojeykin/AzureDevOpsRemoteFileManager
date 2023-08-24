using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Net;
using RestSharp.Authenticators;
using AzureGitClient.Models;
using Microsoft.Extensions.Logging;

namespace AzureGitClient.AzureDevOpsApiClient
{
    internal class AzureDevOpsItemSelector
    {
        #region Properties
        public ProcessResponse Response { get; set; }
        #endregion

        #region Fields
        private readonly string _pat;
        private string _url;
        #endregion

        #region Constants
        private const string URL_BASE = "https://dev.azure.com/PlaceHolder/_apis/git/repositories/{0}/blobs/";
        private const string URL_BASE_ALL_ITEMS = "https://dev.azure.com/PlaceHolder/_apis/git/repositories/{0}/items?recursionLevel=Full&api-version=5.0";
        private const string ERROR_RECEIVE_NOT_OK_HTTP_STATUS = "Received not OK status after http call. Response content: {0}\r\nResponse error: {1}";
        #endregion

        #region Constructors
        public AzureDevOpsItemSelector(string pat)
        {
            _pat = pat;
        }
        #endregion

        // Get the contents of any file in a Repo given its SHA1
        public string GetIndividualRepoFiles(string sha1, string repoName)
        {
            initializeResponse();
            try
            {
                _url = string.Format(URL_BASE, repoName) + $"{sha1}?download=true&fileName=testDownload&$format=octetstream&api-version=6.0";
                Response.ReturnedResponseString = callGetRequestOnApi();
                return Response.ReturnedResponseString;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = ProcessResponse.GetExceptionString(ref ex);
                return Response.SystemError;
            }
        }

        public string GetRepoFilesSHA1(string repoName)
        {
            initializeResponse();
            try
            {
                _url = string.Format(URL_BASE_ALL_ITEMS, repoName);
                Response.ReturnedResponseString = callGetRequestOnApi();
                return Response.ReturnedResponseString;
            }
            catch (Exception ex)
            {
                Response.IsSuccess = false;
                Response.SystemError = ProcessResponse.GetExceptionString(ref ex);
                return Response.SystemError;
            }
        }


        #region Private Methods
        private void initializeResponse()
        {
            Response = new ProcessResponse
            {
                IsSuccess = true,
                ErrorDescription = string.Empty,
                SystemError = string.Empty,
                WarningDescription = string.Empty,
                ReturnedResponseObject = null,
                ProcessDetails = string.Empty
            };
        }

        private string callGetRequestOnApi()
        {
            string result = string.Empty;
            using (RestClient client = new RestClient(_url))
            {
                client.Authenticator = new HttpBasicAuthenticator("", _pat);
                var request = new RestRequest(_url, Method.Get);
                request.Timeout = (300 * 1000);
                RestResponse response;
                response = client.ExecuteGetAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    result = response.Content;
                }
                else
                {
                    string responseError = response.ErrorMessage ?? string.Empty;
                    throw new Exception(string.Format(ERROR_RECEIVE_NOT_OK_HTTP_STATUS, response.Content, responseError));
                }
            }
            return result;
        }
        #endregion
    }
}
