// -
// <copyright file="OcrWord.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Contains one word item that is obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract(Namespace = "")]
    public class OcrWord
    {
        /// <summary>
        /// Initializes a new instance of the OcrWord class.
        /// </summary>
        public OcrWord()
        {
        }
        
        /// <summary>
        /// Gets or sets the text of the word.
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the confidence of the word.
        /// </summary>
        [DataMember]
        public string Confidence { get; set; }

        /// <summary>
        /// Gets or sets the bounding box of the word in a string format.
        /// The box is described as X0,Y0,Width,Height. 
        /// X0,Y0 are the coordinates of the top left corner of the word relative to the top left corner of the image.
        /// </summary>
        [DataMember]
        public string Box { get; set; }

        /// <summary>
        /// Returns a System.String that represents this OcrWord instance.
        /// </summary>
        /// <returns>
        /// A System.String that represents this OcrWord instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Text, this.Confidence);
        }
    }
}
