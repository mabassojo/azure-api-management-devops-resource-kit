﻿// --------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the MIT License.
// --------------------------------------------------------------------------

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.API.Clients.Abstractions;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.Constants;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.Templates.Apis;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extractor.Models;
using Microsoft.Azure.Management.ApiManagement.ArmTemplates.Extractor.Utilities.DataProcessors.Absctraction;

namespace Microsoft.Azure.Management.ApiManagement.ArmTemplates.Common.API.Clients.Apis
{
    public class ApisClient : ApiClientBase, IApisClient
    {
        const string GetSingleApiRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/apis/{4}?api-version={5}";
        const string GetAllApisRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/apis?api-version={4}";
        const string GetAllCurrentApisRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/apis?api-version={4}&$filter=isCurrent";
        const string GetAllApisLinkedToProductRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/products/{4}/apis?api-version={5}";
        const string GetApisLinkedToGatewayRequest = "{0}/subscriptions/{1}/resourceGroups/{2}/providers/Microsoft.ApiManagement/service/{3}/gateways/{4}/apis?api-version={5}";

        readonly IApiDataProcessor apiDataProcessor;

        public ApisClient(IHttpClientFactory httpClientFactory, IApiDataProcessor apiDataProcessor) : base(httpClientFactory)
        {
            this.apiDataProcessor = apiDataProcessor;
        }

        public async Task<ApiTemplateResource> GetSingleAsync(string apiName, ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();
            string requestUrl = string.Format(GetSingleApiRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, apiName, GlobalConstants.ApiVersion);

            var api = await this.GetResponseAsync<ApiTemplateResource>(azToken, requestUrl);
            this.apiDataProcessor.ProcessSingleData(api);

            return api;
        }

        public async Task<List<ApiTemplateResource>> GetAllAsync(ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();
            
            string requestUrl = string.Format(GetAllApisRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, GlobalConstants.ApiVersion);

            var apis = await this.GetPagedResponseAsync<ApiTemplateResource>(azToken, requestUrl);
            this.apiDataProcessor.ProcessData(apis);
            
            return apis;
        }

        public async Task<List<ApiTemplateResource>> GetAllCurrentAsync(ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();

            string requestUrl = string.Format(GetAllCurrentApisRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, GlobalConstants.ApiVersion);

            var apis = await this.GetPagedResponseAsync<ApiTemplateResource>(azToken, requestUrl);
            this.apiDataProcessor.ProcessData(apis);

            return apis;
        }

        public async Task<List<ApiTemplateResource>> GetAllLinkedToProductAsync(string productName, ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();
            string requestUrl = string.Format(GetAllApisLinkedToProductRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, productName, GlobalConstants.ApiVersion);

            var apis = await this.GetPagedResponseAsync<ApiTemplateResource>(azToken, requestUrl);
            this.apiDataProcessor.ProcessData(apis);

            return apis;
        }

        public async Task<List<ApiTemplateResource>> GetAllLinkedToGatewayAsync(string gatewayName, ExtractorParameters extractorParameters)
        {
            var (azToken, azSubId) = await this.Auth.GetAccessToken();
            string requestUrl = string.Format(GetApisLinkedToGatewayRequest,
                this.BaseUrl, azSubId, extractorParameters.ResourceGroup, extractorParameters.SourceApimName, gatewayName, GlobalConstants.ApiVersion);

            var apis = await this.GetPagedResponseAsync<ApiTemplateResource>(azToken, requestUrl);
            this.apiDataProcessor.ProcessData(apis);

            return apis;
        }
    }
}
