// -
// <copyright file="OcrResult.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class describes the result obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract(Namespace = "")]
    public class OcrResult
    {
        /// <summary>
        /// Initializes a new instance of the OcrResult class.
        /// </summary>
        public OcrResult()
        {
            this.InternalErrorMessage = null;
            this.OcrTexts = new List<OcrText>();
        }

        /// <summary>
        /// Gets or sets the error message if an error occures during the OCR process.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string InternalErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets OcrTexts items.
        /// </summary>
        [DataMember]
        public List<OcrText> OcrTexts { get; set; }
    }
}