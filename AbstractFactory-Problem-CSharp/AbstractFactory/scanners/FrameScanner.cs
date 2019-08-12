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
using FrameTag = org.htmlparser.tags.FrameTag;
using Tag = org.htmlparser.tags.Tag;
using TagData = org.htmlparser.tags.data.TagData;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;

namespace org.htmlparser.scanners
{
    /// <summary> Scans for the Frame Tag. This is a subclass of TagScanner, and is called using a
    /// variant of the template method. If the Evaluate() method returns true, that means the
    /// given string contains an image tag. Extraction is done by the scan method thereafter
    /// by the user of this class.
    /// </summary>
    public class FrameScanner : TagScanner
    {
        /// <seealso cref="">org.htmlparser.scanners.TagScanner#ID
        ///
        /// </seealso>
        public override string[] ID
        {
            get { return (new string[] {"FRAME"}); }
        }

        /// <summary> Overriding the default constructor
        /// </summary>
        public FrameScanner() : base()
        {
        }

        /// <summary> Overriding the constructor to accept the filter
        /// </summary>
        public FrameScanner(string filter) : base(filter)
        {
        }

        /// <summary> Extract the location of the image, given the string to be parsed, and the url
        /// of the html page in which this tag exists.
        /// </summary>
        /// <param name="tag">String to be parsed
        /// </param>
        /// <param name="url">URL of web page being parsed
        ///
        /// </param>
        public virtual string ExtractFrameLocn(Tag tag, string url)
        {
            try
            {
                System.Collections.Hashtable table = tag.Attributes;
                string relativeFrame = (string) table["SRC"];
                if (relativeFrame == null)
                    return "";
                else
                    return (new LinkProcessor()).Extract(relativeFrame, url);
            }
            catch (System.Exception e)
            {
                string msg;
                if (tag != null)
                    msg = tag.Text;
                else
                    msg = "null";
                throw new ParserException(
                    "HTMLFrameScanner.ExtractFrameLocn() : Error in extracting frame location from tag " + msg, e);
            }
        }

        public virtual string ExtractFrameName(Tag tag, string url)
        {
            return tag["NAME"];
        }

        protected override Tag CreateTag(TagData tagData, Tag tag, string url)
        {
            string frameUrl = ExtractFrameLocn(tag, url);
            string frameName = ExtractFrameName(tag, url);

            return new FrameTag(tagData, frameUrl, frameName);
        }
    }
}
