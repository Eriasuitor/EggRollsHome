using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Newegg.API.Client;
using Newegg.MIS.API.Utilities.Entities;
using Newegg.MIS.API.Utilities.Exceptions;
using ValidationErrorResponse = Newegg.MIS.API.Utilities.Entities.ValidationErrorResponse;

namespace Newegg.MIS.API.Utilities.Helpers
{
    public sealed class APIResourceProxy
    {
        public const string EaaSManagementAPIBaseUrl = "EaaS_Management_API_BaseUrl";
        public const string EaaSManagementAPIAuthorization = "EaaS_Management_API_Authorization";
        public const string EaaSManagementAPIToken = "EaaS_Management_API_AuthorizationToken";

        public const string AuthorizationTokenHeader = "AuthorizationToken";

        private static readonly ConcurrentDictionary<string, APIResourceProxy> APIProxyPool = 
            new ConcurrentDictionary<string, APIResourceProxy>();

        public static APIResourceProxy GetProxy(string configurationPrefix)
        {
            return APIProxyPool.GetOrAdd(configurationPrefix,
                                  key => new APIResourceProxy(key));
        }

        public static APIResourceProxy GetProxy()
        {
            return GetProxy(string.Empty);
        }

        private string _configurationPrefix;
        private RestAPIClient client;
        
        private APIResourceProxy(string prefix)
        {
            _configurationPrefix = prefix;

            var baseUrl = GetConfigurationWithPrefix(EaaSManagementAPIBaseUrl);

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                throw new ApplicationException(
                    string.Format("Failed to retrieve base url from configuration by [{0}{1}].",
                    prefix,
                    EaaSManagementAPIBaseUrl));
            }

            client = new RestAPIClient(baseUrl);

            var authorization = GetConfigurationWithPrefix(EaaSManagementAPIAuthorization);

            if (!String.IsNullOrWhiteSpace(authorization))
            {
                client.Authorization = authorization;
            }

            var token = GetConfigurationWithPrefix(EaaSManagementAPIToken);

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.AddCustomHeader(AuthorizationTokenHeader, token);
            }
        }

        private string GetConfigurationWithPrefix(string configurationKey)
        {
            return ConfigurationHelper.GetConfiguration(
                _configurationPrefix + configurationKey
                );
        }

        public TResponse GetRemoteAPIResource<TResponse>(
            string uri,
            QueryRequestBase query)
            where TResponse : GeneralResponse
        {
            return GetRemoteAPIResource<TResponse>(client, uri, query);
        }

        private TResponse GetRemoteAPIResource<TResponse>(
            RestAPIClient restAPIClient,
            string uri, QueryRequestBase query)
            where TResponse : GeneralResponse
        {
            try
            {
                var response =
                    restAPIClient.Get<TResponse>(BuildAPIUrl(uri, query), null);

                HandleResponse(response);

                return response;
            }
            catch (Exception ex)
            {

                throw new APIResourceProxyException(ex);
            }
        }

        public TResponse Post<TResponse>(string uri, object request) 
            where TResponse : GeneralResponse
        {
            try
            {
                var response =
                    client.Post<TResponse>(uri, request);

                HandleResponse(response);

                return response;
            }
            catch (Exception ex)
            {
                throw new APIResourceProxyException(ex);
            }
        }

        public TResponse Put<TResponse>(string uri, object request)
            where TResponse : GeneralResponse
        {
            try
            {
                var response =
                    client.Put<TResponse>(uri, request);

                HandleResponse(response);

                return response;
            }
            catch (Exception ex)
            {
                throw new APIResourceProxyException(ex);
            }
        }

        private static void HandleResponse(GeneralResponse response)
        {
            if (null == response)
            {
                throw new APIResourceProxyException(
                    "The response retrieved from remote resource was empty.");
            }

            if (!response.Succeeded.HasValue)
            {
                throw new APIResourceProxyException(
                    "The response not returning a value to property Succeeded " +
                    "to identity the request was processed or not.");
            }

            if (response.Succeeded.Value) return;

            const string failureMessageTemplate = "The remote API failed to process the request due to {0}.";

            if (response.Errors != null && response.Errors.Count > 0)
            {
                throw new APIResourceProxyException(
                    string.Format(failureMessageTemplate,
                    BuildErrorMessage(response.Errors)));
            }

            if (response.ValidationErrors != null && response.ValidationErrors.Count > 0)
            {
                throw new APIResourceProxyException(
                    string.Format(failureMessageTemplate,
                    BuildErrorMessage(response.ValidationErrors)));
            }

            throw new APIResourceProxyException(
                    string.Format(failureMessageTemplate,
                    "Unknown error"));
        }

        private static string BuildErrorMessage(
            IEnumerable<ValidationErrorResponse> list)
        {
            var content = new StringBuilder();

            foreach (var validationErrorResponse in list)
            {
                content.AppendFormat("{0};", validationErrorResponse.ErrorMessage);
            }

            return content.ToString();
        }

        private static string BuildErrorMessage(IEnumerable<Error> list)
        {
            var content = new StringBuilder();

            foreach (var error in list)
            {
                content.AppendFormat("[{0}]{1};", error.Code, error.Message);
            }

            return content.ToString();
        }

        public static string BuildAPIUrl(string baseUri, QueryRequestBase request)
        {
            if (string.IsNullOrWhiteSpace(baseUri))
            {
                throw new APIResourceProxyException("The parameter baseUri must be provided.");
            }

            if (null == request) return baseUri;


            var properties = request.GetType().GetProperties(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.DeclaredOnly);

            var parameters = new List<string>();

            foreach (var propertyInfo in properties)
            {
                AppendQueryParameter(parameters, propertyInfo, request);
            }

            if (!string.IsNullOrWhiteSpace(QueryRequestBase.RequestID))
            {
                parameters.Add("RequestID=" + QueryRequestBase.RequestID.Trim());
            }

            if (request.IsRetry())
            {
                parameters.Add("retry-api-call=true");
            }

            if (parameters.Count == 0) return baseUri;

            return baseUri + "?" + string.Join("&", parameters);
        }

        private static void AppendQueryParameter(
            ICollection<string> parameterCollection,
            PropertyInfo propertyInfo,
            QueryRequestBase request)
        {
            var value =
                propertyInfo.GetValue(request,
                                      BindingFlags.Instance |
                                      BindingFlags.Public,
                                      null,
                                      null,
                                      CultureInfo.CurrentCulture);

            if (null == value) return;

            if (propertyInfo.PropertyType == typeof(string))
            {
                value = Uri.EscapeDataString(value.ToString());
            }

            parameterCollection.Add(propertyInfo.Name + "=" + value);
        }
    }
}
