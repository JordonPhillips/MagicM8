// -
// <copyright file="OcrAgent.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// This class provides helper methods used to communicate with the Hawaii OCR Service.
    /// for character recognition. This class accepts an image buffer as an input, sends it to the 
    /// OCR service and receives the result of the OCR processing.
    /// </summary>
    internal class OcrAgent : ServiceAgent<OcrServiceResult>
    {
        /// <summary>
        /// Initializes a new instance of the OcrAgent class.
        /// </summary>
        /// <param name="hostName">Specifies the host name of the OCR service.</param>
        /// <param name="clientIdentity">Specifies the client identity.</param>
        /// <param name="imageBuffer">
        /// Specifies a buffer containing an image that has to be processed.
        /// The image must be in JPEG format.
        /// </param>
        /// <param name="stateObject">Specifies a user defined object which will be provided in the call to the "on complete" calback.</param>
        public OcrAgent(string hostName, ClientIdentity clientIdentity, byte[] imageBuffer, object stateObject) :
            base(HttpMethod.Post, stateObject)
        {
            if (string.IsNullOrEmpty(hostName))
            {
                throw new ArgumentNullException("hostName");
            }

            if (imageBuffer == null || imageBuffer.Length == 0)
            {
                throw new ArgumentNullException("audioBuffer");
            }

            this.ImageBuffer = imageBuffer;

            // Set the client identity.
            this.ClientIdentity = clientIdentity;

            // Create the service uri.
            this.Uri = this.CreateServiceUri(hostName);
        }

        /// <summary>
        /// Gets the buffer containing the image that has to be processed.
        /// </summary>
        public byte[] ImageBuffer { get; private set; }

        /// <summary>
        /// An overriden property to set the request content type to be Json.
        /// </summary>
        protected override string RequestContentType
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// An overridden abstract method. It provides the POST (as in Http POST) data that has to be sent to the service.
        /// This method will be called by the base class when it needs data during a Http POST method call.
        /// </summary>
        /// <returns>
        /// The POST data as an array of bytes.
        /// </returns>
        protected override byte[] GetPostData()
        {
            // This service client is for OCR, so the method returns the image buffer. 
            return this.ImageBuffer;
        }

        /// <summary>
        /// An overridden abstract method. This method is called after the response sent by the server is received by the client.
        /// </summary>
        /// <param name="responseStream">
        /// The response stream containing response data that is received from the server.
        /// </param>
        protected override void ParseOutput(Stream responseStream)
        {
            if (responseStream == null)
            {
                throw new ArgumentException("Response stream is null");
            }

            Debug.Assert(this.Result != null, "result is null");
            this.Result.OcrResult = DeserializeResponse<OcrResult>(responseStream);
        }

        /// <summary>
        /// An overriden method. 
        /// It is invoked after completing the service request. It does some processing of the OCR service call result 
        /// and it calls the client's "on complete" callback method.
        /// </summary>
        protected override void OnCompleteRequest()
        {
            if (this.OnComplete != null)
            {
                // Create the event argument object based on success/failure.
                if (this.Result.Exception == null)
                {
                    // A successful recognition.
                    OcrServiceResult ocrResult = this.Result as OcrServiceResult;
                    Debug.Assert(ocrResult != null, "ocrResult is null");

                    // But an error at server side.
                    if (ocrResult.OcrResult.InternalErrorMessage != null)
                    {
                        this.Result.Exception = new Exception("A server side error occured while processing the OCR request.");
                        this.Result.Status = Status.InternalServerError;
                    }
                }

                // Call the UI's on completion method.
                this.OnComplete(this.Result);
            }
        }

        /// <summary>
        /// It creates the service uri.
        /// </summary>
        /// <param name="hostName">Specifies a host name of the service.</param>
        /// <returns>A valid service uri object.</returns>
        private Uri CreateServiceUri(string hostName)
        {
            // Create the service Uri.
            ServiceUri uri = new ServiceUri(hostName, OcrService.ServiceSignature);

            // Return the URI object.
            return new Uri(uri.ToString());
        }
    }
}
