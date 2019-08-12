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
using NodeList = org.htmlparser.util.NodeList;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser
{
    /// <summary> The remark tag is identified and represented by this class.
    /// </summary>
    public class RemarkNode : AbstractNode
    {
        /// <summary> Returns the text contents of the comment tag.
        /// </summary>
        public override string Text
        {
            get { return tagContents; }
        }

        public const string REMARK_NODE_FILTER = "-r";

        /// <summary> Tag contents will have the contents of the comment tag.
        /// </summary>
        internal string tagContents;

        /// <summary> The RemarkNode is constructed by providing the beginning posn, ending posn
        /// and the tag contents.
        /// </summary>
        /// <param name="nodeBegin">beginning position of the tag
        /// </param>
        /// <param name="nodeEnd">ending position of the tag
        /// </param>
        /// <param name="tagContents">contents of the remark tag
        /// </param>
        /// <param name="tagLine">The current line being parsed, where the tag was found
        /// 
        /// </param>
        public RemarkNode(int tagBegin, int tagEnd, string tagContents) : base(tagBegin, tagEnd)
        {
            this.tagContents = tagContents;
        }

        public override string ToPlainTextString()
        {
            return tagContents;
        }

        public override string ToHtml()
        {
            return "<!--" + tagContents + "-->";
        }

        /// <summary> Print the contents of the remark tag.
        /// </summary>
        public override string ToString()
        {
            return "Comment Tag : " + tagContents + "; begins at : " + ElementBegin + "; ends at : " + ElementEnd + "\n";
        }

        public override void CollectInto(NodeList collectionList, string filter)
        {
            if (filter == REMARK_NODE_FILTER)
                collectionList.Add(this);
        }

        public override void Accept(NodeVisitor visitor)
        {
            visitor.VisitRemarkNode(this);
        }
    }
}
