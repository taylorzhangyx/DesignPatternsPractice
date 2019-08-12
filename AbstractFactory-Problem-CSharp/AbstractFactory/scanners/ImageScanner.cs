/// ***************************************************************************
/// Copyright (c) 2009, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************

// HTMLParser Library v1_4_20030601 - A java-based parser for HTML
// Copyright (C) Dec 31, 2000 Somik Raha
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// For any questions or suggestions, you can write to me at :
// Email :somik@industriallogic.com
//
// Postal Address :
// Somik Raha
// Extreme Programmer & Coach
// Industrial Logic, Inc.
// 2583 Cedar Street, Berkeley,
// CA 94708, USA
// Website : http://www.industriallogic.com

using System;
using ImageTag = org.htmlparser.tags.ImageTag;
using Tag = org.htmlparser.tags.Tag;
using TagData = org.htmlparser.tags.data.TagData;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;
using ParserUtils = org.htmlparser.util.ParserUtils;

namespace org.htmlparser.scanners
{
    /// <summary> Scans for the Image Tag. This is a subclass of TagScanner, and is called using a
    /// variant of the template method. If the Evaluate() method returns true, that means the
    /// given string contains an image tag. Extraction is done by the scan method thereafter
    /// by the user of this class.
    /// </summary>
    public class ImageScanner : TagScanner
    {
        public override string[] ID
        {
            get { return (new string[] {IMAGE_SCANNER_ID}); }
        }

        public const string IMAGE_SCANNER_ID = "IMG";
        private System.Collections.Hashtable table;
        private LinkProcessor processor;

        /// <summary> Overriding the default constructor
        /// </summary>
        public ImageScanner() : base()
        {
            processor = new LinkProcessor();
        }

        /// <summary> Overriding the constructor to accept the filter
        /// </summary>
        public ImageScanner(string filter, LinkProcessor processor) : base(filter)
        {
            this.processor = processor;
        }

        /// <summary> Extract the location of the image, given the string to be parsed, and the url
        /// of the html page in which this tag exists.
        /// </summary>
        /// <param name="tag">String to be parsed
        /// </param>
        /// <param name="url">URL of web page being parsed
        ///
        /// </param>
        public virtual string ExtractImageLocn(Tag tag, string url)
        {
            string relativeLink = null;
            try
            {
                table = tag.Attributes;
                relativeLink = (string) table["SRC"];
                if (relativeLink != null)
                {
                    relativeLink = ParserUtils.RemoveChars(relativeLink, '\n');
                    relativeLink = ParserUtils.RemoveChars(relativeLink, '\r');
                }
                if (relativeLink == null || relativeLink.Length == 0)
                {
                    // try fix
                    string tagText = tag.Text.ToUpper();
                    int indexSrc = tagText.IndexOf("SRC");
                    if (indexSrc != - 1)
                    {
                        // There is a missing equals.
                        tag.Text = tag.Text.Substring(0, (indexSrc + 3) - (0)) + "=" +
                                   tag.Text.Substring(indexSrc + 3, (tag.Text.Length) - (indexSrc + 3));
                        table = tag.RedoParseAttributes();
                        relativeLink = (string) table["SRC"];
                    }
                }
                if (relativeLink == null)
                    return "";
                else
                    return processor.Extract(relativeLink, url);
            }
            catch (System.Exception e)
            {
                throw new ParserException(
                    "HTMLImageScanner.ExtractImageLocn() : Error in extracting image location, relativeLink = " +
                    relativeLink + ", url = " + url, e);
            }
        }

        protected override Tag CreateTag(TagData tagData, Tag tag, string url)
        {
            string link = ExtractImageLocn(tag, url);
            return new ImageTag(tagData, link);
        }
    }
}
