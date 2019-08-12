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

namespace org.htmlparser.tags.data
{
    public class TagData
    {
        public int TagBegin
        {
            get { return tagBegin; }
        }

        public string TagContents
        {
            get { return tagContents; }
            set { this.tagContents = value; }
        }

        public int TagEnd
        {
            get { return tagEnd; }
        }

        public string TagLine
        {
            get { return tagLine; }
        }

        public string UrlBeingParsed
        {
            get { return urlBeingParsed; }
            set { this.urlBeingParsed = value; }
        }

        public bool EmptyXmlTag
        {
            get { return isXmlEndTag; }
        }

        /// <summary> Returns the line number where the tag starts in the HTML. At the moment this
        /// will only be valid for tags created with the
        /// <code>CompositeTagScanner</code> or a subclass of it.
        /// </summary>
        public int StartLine
        {
            get { return startLine; }
        }

        /// <summary> Returns the line number where the tag ends in the HTML. At the moment this
        /// will only be valid for tags created with the
        /// <code>CompositeTagScanner</code> or a subclass of it.
        /// </summary>
        public int EndLine
        {
            get { return endLine; }
        }

        private int tagBegin;
        private int tagEnd;
        private int startLine;
        private int endLine;
        private string tagContents;
        private string tagLine;
        private string urlBeingParsed;
        private bool isXmlEndTag;

        public TagData(int tagBegin, int tagEnd, string tagContents, string tagLine) :
            this(tagBegin, tagEnd, 0, 0, tagContents, tagLine, "", false)
        {
        }

        public TagData(int tagBegin, int tagEnd, string tagContents, string tagLine, string urlBeingParsed) :
            this(tagBegin, tagEnd, 0, 0, tagContents, tagLine, urlBeingParsed, false)
        {
        }

        public TagData(int tagBegin, int tagEnd, int startLine, int endLine, string tagContents, string tagLine,
                       string urlBeingParsed, bool isXmlEndTag)
        {
            this.tagBegin = tagBegin;
            this.tagEnd = tagEnd;
            this.startLine = startLine;
            this.endLine = endLine;
            this.tagContents = tagContents;
            this.tagLine = tagLine;
            this.urlBeingParsed = urlBeingParsed;
            this.isXmlEndTag = isXmlEndTag;
        }
    }
}
