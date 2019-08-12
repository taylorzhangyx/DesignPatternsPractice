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
using LinkTag = org.htmlparser.tags.LinkTag;
using Tag = org.htmlparser.tags.Tag;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using LinkData = org.htmlparser.tags.data.LinkData;
using TagData = org.htmlparser.tags.data.TagData;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;
using ParserUtils = org.htmlparser.util.ParserUtils;

namespace org.htmlparser.scanners
{
    /// <summary> Scans for the Link Tag. This is a subclass of TagScanner, and is called using a
    /// variant of the template method. If the Evaluate() method returns true, that means the
    /// given string contains an image tag. Extraction is done by the scan method thereafter
    /// by the user of this class.
    /// </summary>
    public class LinkScanner : CompositeTagScanner
    {
        /// <seealso cref="">org.htmlparser.scanners.TagScanner#ID
        ///
        /// </seealso>
        public override string[] ID
        {
            get { return MATCH_NAME; }
        }

        private static readonly string[] MATCH_NAME = new string[] {"A"};
        public const string LINK_SCANNER_ID = "A";

        public const string DIRTY_TAG_MESSAGE =
            " is a dirty link tag - the tag was not closed. \nWe encountered an open tag, before the previous end tag was found.\nCorrecting this..";

        private LinkProcessor processor;
        private static readonly string[] ENDERS = new string[] {"TD", "TR", "FORM", "LI", "BODY", "HTML"};
        private static readonly string[] ENDTAG_ENDERS = new string[] {"TD", "TR", "FORM", "LI", "BODY", "HTML"};

        /// <summary> Overriding the default constructor
        /// </summary>
        public LinkScanner() : this("")
        {
        }

        /// <summary> Overriding the constructor to accept the filter
        /// </summary>
        public LinkScanner(string filter) : base(filter, MATCH_NAME, ENDERS, ENDTAG_ENDERS, false)
        {
            processor = new LinkProcessor();
        }

        public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
        {
            string link = ExtractLink(compositeTagData.StartTag, tagData.UrlBeingParsed);
            int mailto = link.IndexOf("mailto");
            bool mailLink = false;
            if (mailto == 0)
            {
                // yes it is
                mailto = link.IndexOf(":");
                link = link.Substring(mailto + 1);
                mailLink = true;
            }
            int javascript = link.IndexOf("javascript:");
            bool javascriptLink = false;
            if (javascript == 0)
            {
                link = link.Substring(11); // this magic number is "javascript:".length()
                javascriptLink = true;
            }
            string accessKey = GetAccessKey(compositeTagData.StartTag);
            string myLinkText = compositeTagData.Children.ToString();

            LinkTag linkTag = new LinkTag(tagData, compositeTagData,
                                          new LinkData(link, myLinkText, accessKey, mailLink, javascriptLink));
            linkTag.ThisScanner = this;
            return linkTag;
        }

        /// <summary> Template Method, used to decide if this scanner can handle the Link tag type. If
        /// the evaluation returns true, the calling side makes a call to scan().
        /// </summary>
        /// <param name="s">The complete text contents of the Tag.
        /// </param>
        /// <param name="previousOpenScanner">Indicates any previous scanner which hasnt completed, before the current
        /// scan has begun, and hence allows us to write scanners that can work with dirty html
        ///
        /// </param>
        public override bool Evaluate(string s, TagScanner previousOpenScanner)
        {
            char ch;
            bool ret;

            // eat up leading blanks
            s = AbsorbLeadingBlanks(s);
            if (5 > s.Length)
                ret = false;
            else
            {
                ch = s[0];
                if ((ch == 'a' || ch == 'A') && System.Char.IsWhiteSpace(s[1]))
                    ret = - 1 != s.ToUpper().IndexOf("HREF");
                else
                    ret = false;
            }

            return (ret);
        }

        /// <summary> Extract the link from the given string. The URL of the actual html page is also
        /// provided.
        /// </summary>
        public virtual string ExtractLink(Tag tag, string url)
        {
            try
            {
                System.Collections.Hashtable table = tag.Attributes;
                string relativeLink = (string) table["HREF"];
                if (relativeLink != null)
                {
                    relativeLink = ParserUtils.RemoveChars(relativeLink, '\n');
                    relativeLink = ParserUtils.RemoveChars(relativeLink, '\r');
                }
                return processor.Extract(relativeLink, url);
            }
            catch (System.Exception e)
            {
                string msg;
                if (tag != null)
                    msg = tag.Text;
                else
                    msg = "null";
                throw new ParserException(
                    "HTMLLinkScanner.ExtractLink() : Error while extracting link from tag " + msg + ", url = " + url, e);
            }
        }

        /// <summary> Extract the access key from the given tag.
        /// </summary>
        /// <param name="tag">Text to be parsed to pick out the access key.
        /// </param>
        /// <returns> The value of the ACCESSKEY attribute.
        ///
        /// </returns>
        private string GetAccessKey(Tag tag)
        {
            return tag["ACCESSKEY"];
        }

        public virtual BaseHrefScanner CreateBaseHREFScanner(string filter)
        {
            return new BaseHrefScanner(filter, processor);
        }

        public virtual ImageScanner CreateImageScanner(string filter)
        {
            return new ImageScanner(filter, processor);
        }
    }
}
