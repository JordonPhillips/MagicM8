// -
// <copyright file="OcrService.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.IO;
    using System.Security;

    /// <summary>
    /// Helper class that provides access to the OCR service.
    /// </summary>
    public static class OcrService
    {
        /// <summary>
        /// Specifies the default service host name. This will be used to create the service url.
        /// </summary>
        public const string DefaultHostName = @"http://api.hawaii-services.net/Ocr/V1";

        /// <summary>
        /// Specifies a signature for the REST method that executes the OCR processing.
        /// </summary>
        public const string ServiceSignature = "OCR";

        /// <summary>
        /// The name of the config file that indicates where the Ocr staging service is located.  Used only as a test hook.
        /// </summary>
        private static readonly string stagingConfigFileName = @"C:\AzureStagingDeploymentConfig\OcrStagingHostConfig.ini";

        /// <summary>
        /// The service host name.  This is the private variable that is initialized on first
        /// access via the GetHostName() method.  If a config file is present to point to a
        /// staging server, that host will be stored.  Otherwise, the default is used.
        /// </summary>
        private static string hostName;

        /// <summary>
        /// The service scope.  This is the private variable that is initialized on first
        /// access via the ServiceScope get accessor.  If a config file is present to point to a
        /// staging server, that host will be stored.  Otherwise, the default is used.
        /// </summary>
        private static string serviceScope;

        /// <summary>
        /// Gets the Host Name to be used when accessing the service.
        /// </summary>
        public static string HostName
        {
            get
            {
                return OcrService.GetHostName();
            }
        }

        /// <summary>
        /// Gets the service scope to be used when accessing the adm OAuth service.
        /// </summary>
        private static string ServiceScope
        {
            get
            {
                if (string.IsNullOrEmpty(OcrService.serviceScope))
                {
                    OcrService.serviceScope = AdmAuthClientIdentity.GetServiceScope(OcrService.HostName);
                }

                return OcrService.serviceScope;
            }
        }

        /// <summary>
        /// Helper method to recognize an image.
        /// </summary>
        /// <param name="hawaiiAppId">Specifies the Hawaii Application Id.</param>
        /// <param name="imageBuffer">
        /// Specifies a buffer containing an image that has to be processed.
        /// The image must be in JPEG format.
        /// </param>
        /// <param name="onComplete">Specifies an on complete callback method.</param>
        /// <param name="stateObject">Specifies a user defined object which will be provided in the call of the callback method.</param>
        public static void RecognizeImageAsync(
            string hawaiiAppId, 
            byte[] imageBuffer,
            ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete, 
            object stateObject = null)
        {
            if (string.IsNullOrEmpty(hawaiiAppId))
            {
                throw new ArgumentNullException("hawaiiAppId");
            }

            RecognizeImageAsync(
                new GuidAuthClientIdentity(hawaiiAppId),
                imageBuffer,
                onComplete,
                stateObject);
        }

        /// <summary>
        /// Helper method to recognize an image.
        /// </summary>
        /// <param name="clientIdentity">The hawaii client identity.</param>
        /// <param name="imageBuffer">
        /// Specifies a buffer containing an image that has to be processed.
        /// The image must be in JPEG format.
        /// </param>
        /// <param name="onComplete">Specifies an on complete callback method.</param>
        /// <param name="stateObject">Specifies a user defined object which will be provided in the call of the callback method.</param>
        private static void RecognizeImageAsync(
            ClientIdentity clientIdentity,
            byte[] imageBuffer,
            ServiceAgent<OcrServiceResult>.OnCompleteDelegate onComplete,
            object stateObject = null)
        {
            OcrAgent agent = new OcrAgent(
                OcrService.HostName,
                clientIdentity,
                imageBuffer,
                stateObject);

            agent.ProcessRequest(onComplete);
        }

        /// <summary>
        /// Returns the Host Name to be used when accessing the service.  This will generally
        /// be the value specified in the DefaultHostName, but it can conditionally be set with
        /// the presence of a config file on first access.
        /// </summary>
        /// <returns>A string containing the host name of the service</returns>
        [SecuritySafeCritical]
        private static string GetHostName()
        {
            if (string.IsNullOrEmpty(OcrService.hostName))
            {
                OcrService.hostName = ClientLibraryUtils.LookupHostNameFromConfig(stagingConfigFileName, OcrService.DefaultHostName);
            }

            return OcrService.hostName;
        }
    }
}