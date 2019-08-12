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
using StringNode = org.htmlparser.StringNode;
using EndTag = org.htmlparser.tags.EndTag;
using Tag = org.htmlparser.tags.Tag;
using TitleTag = org.htmlparser.tags.TitleTag;
using Translate = org.htmlparser.util.Translate;

namespace org.htmlparser.visitors
{
    /// <summary> Extracts text from a web page.
    /// Usage:
    /// <code>
    /// Parser parser = new Parser(...);
    /// TextExtractingVisitor visitor = new TextExtractingVisitor();
    /// parser.VisitAllNodesWith(visitor);
    /// String textInPage = visitor.getExtractedText();
    /// </code>
    /// </summary>
    public class TextExtractingVisitor : NodeVisitor
    {
        public string ExtractedText
        {
            get { return textAccumulator.ToString(); }
        }

        private System.Text.StringBuilder textAccumulator;
        private bool preTagBeingProcessed;

        public TextExtractingVisitor()
        {
            textAccumulator = new System.Text.StringBuilder();
            preTagBeingProcessed = false;
        }

        public override void VisitStringNode(StringNode stringNode)
        {
            string text = stringNode.Text;
            if (!preTagBeingProcessed)
            {
                text = Translate.Decode(text);
                text = ReplaceNonBreakingSpaceWithOrdinarySpace(text);
            }
            textAccumulator.Append(text);
        }

        public override void VisitTitleTag(TitleTag titleTag)
        {
            textAccumulator.Append(titleTag.Title);
        }

        private string ReplaceNonBreakingSpaceWithOrdinarySpace(string text)
        {
            return text.Replace('\u00a0', ' ');
        }

        public override void VisitEndTag(EndTag endTag)
        {
            if (IsPreTag(endTag))
                preTagBeingProcessed = false;
        }

        public override void VisitTag(Tag tag)
        {
            if (IsPreTag(tag))
                preTagBeingProcessed = true;
        }

        private bool IsPreTag(Tag tag)
        {
            return tag.TagName.Equals("PRE");
        }
    }
}
