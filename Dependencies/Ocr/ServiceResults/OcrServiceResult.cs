// -
// <copyright file="OcrServiceResult.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Hawaii;

    /// <summary>
    /// This class represents the result of the OCR processing.
    /// </summary>
    public class OcrServiceResult : ServiceResult
    {
        /// <summary>
        /// Gets the result of the OCR processing.
        /// </summary>
        public OcrResult OcrResult { get; internal set; }
    }
}
