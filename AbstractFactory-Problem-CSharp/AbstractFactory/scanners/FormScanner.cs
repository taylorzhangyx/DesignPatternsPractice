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
using Parser = org.htmlparser.Parser;
using FormTag = org.htmlparser.tags.FormTag;
using Tag = org.htmlparser.tags.Tag;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;

namespace org.htmlparser.scanners
{
    /// <summary> Scans for the Image Tag. This is a subclass of TagScanner, and is called using a
    /// variant of the template method. If the Evaluate() method returns true, that means the
    /// given string contains an image tag. Extraction is done by the scan method thereafter
    /// by the user of this class.
    /// </summary>
    public class FormScanner : CompositeTagScanner
    {
        /// <seealso cref="">org.htmlparser.scanners.TagScanner#ID
        ///
        /// </seealso>
        public override string[] ID
        {
            get { return MATCH_ID; }
        }

        private static readonly string[] MATCH_ID = new string[] {"FORM"};

        public const string PREVIOUS_DIRTY_LINK_MESSAGE =
            "Encountered a form tag after an open link tag.\nThere should have been an end tag for the link before the form tag began.\nCorrecting this..";

        private static readonly string[] formTagEnders = new string[] {"HTML", "BODY"};
        private System.Collections.Stack stack;

        /// <summary> HTMLFormScanner constructor comment.
        /// </summary>
        public FormScanner(Parser parser) : this("", parser)
        {
        }

        /// <summary> Overriding the constructor to accept the filter
        /// </summary>
        public FormScanner(string filter, Parser parser) : base(filter, MATCH_ID, formTagEnders, false)
        {
            stack = new System.Collections.Stack();
            parser.AddScanner(new InputTagScanner("-i"));
        }

        /// <summary> Extract the location of the image, given the string to be parsed, and the url
        /// of the html page in which this tag exists.
        /// </summary>
        /// <param name="s">String to be parsed
        /// </param>
        /// <param name="url">URL of web page being parsed
        ///
        /// </param>
        public virtual string ExtractFormLocn(Tag tag, string url)
        {
            try
            {
                string formURL = tag["ACTION"];
                if (formURL == null)
                    return "";
                else
                    return (new LinkProcessor()).Extract(formURL, url);
            }
            catch (System.Exception e)
            {
                string msg;
                if (tag != null)
                    msg = tag.Text;
                else
                    msg = "";
                throw new ParserException(
                    "HTMLFormScanner.extractFormLocn() : Error in extracting form location, tag = " + msg + ", url = " +
                    url, e);
            }
        }

        public virtual string ExtractFormName(Tag tag)
        {
            return tag["NAME"];
        }

        public virtual string ExtractFormMethod(Tag tag)
        {
            string method = tag["METHOD"];
            if (method == null)
                method = FormTag.GET;
            return method.ToUpper();
        }

        /// <summary> Scan the tag and extract the information related to the <IMG> tag. The url of the
        /// initiating scan has to be provided in case relative links are found. The initial
        /// url is then prepended to it to give an absolute link.
        /// The NodeReader is provided in order to do a lookahead operation. We assume that
        /// the identification has already been performed using the Evaluate() method.
        /// </summary>
        /// <param name="tag">HTML Tag to be scanned for identification
        /// </param>
        /// <param name="url">The initiating url of the scan (Where the html page lies)
        /// </param>
        /// <param name="reader">The reader object responsible for reading the html page
        /// </param>
        /// <param name="currentLine">The current line (automatically provided by Tag)
        ///
        /// </param>
        public override bool Evaluate(string s, TagScanner previousOpenScanner)
        {
            if (previousOpenScanner is LinkScanner)
            {
                System.Text.StringBuilder msg = new System.Text.StringBuilder();
                msg.Append("<");
                msg.Append(s);
                msg.Append(">");
                msg.Append(PREVIOUS_DIRTY_LINK_MESSAGE);
                feedback.Warning(msg.ToString());
                // This is dirty HTML. Assume the current tag is
                // not a new link tag - but an end tag. This is actually a really wild bug -
                // Internet Explorer actually parses such tags.
                // So - we shall then proceed to fool the scanner into sending an endtag of type </A>
                // For this - set the dirty flag to true and return
            }
            return base.Evaluate(s, previousOpenScanner);
        }

        public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
        {
            string formUrl = ExtractFormLocn(compositeTagData.StartTag, tagData.UrlBeingParsed);
            if (formUrl != null && formUrl.Length > 0)
                compositeTagData.StartTag["ACTION"] = formUrl;
            if (!(stack.Count == 0) && (this == stack.Peek()))
                stack.Pop();
            return new FormTag(tagData, compositeTagData);
        }

        public override void BeforeScanningStarts()
        {
            stack.Push(this);
        }
    }
}
