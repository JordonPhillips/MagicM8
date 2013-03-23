// -
// <copyright file="OcrText.cs" company="Microsoft Corporation">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -

namespace Microsoft.Hawaii.Ocr.Client
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    
    /// <summary>
    /// Contains one text item that is obtained after a Hawaii OCR call.
    /// </summary>
    [DataContract(Namespace = "")]
    public class OcrText
    {
        /// <summary>
        /// Initializes a new instance of the OcrText class.
        /// </summary>
        public OcrText()
        {
            this.Words = new List<OcrWord>();
        }
        
        /// <summary>
        /// Gets or sets the orientation of the text.
        /// </summary>
        [DataMember]
        public string Orientation { get; set; }

        /// <summary>
        /// Gets or sets the skewness of the text.
        /// </summary>
        [DataMember]
        public string Skew { get; set; }

        /// <summary>
        /// Gets or sets the list of words that are contained in the text.
        /// </summary>
        [DataMember]
        public List<OcrWord> Words { get; set; }

        /// <summary>
        /// Gets the text of all the words (this.Words) separated by 
        /// space and combined in a single string.
        /// </summary>
        public string Text
        {
            get
            {
                if (this.Words.Count == 0)
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();
                foreach (var word in this.Words)
                {
                    sb.Append(word.Text);
                    sb.Append(' ');
                }

                sb.Length--;
                return sb.ToString();
            }
        }
    }
}
