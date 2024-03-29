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
using System.Text;
using AbstractNode = org.htmlparser.AbstractNode;
using TagData = org.htmlparser.tags.data.TagData;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser.tags
{
    /// <summary> EndTag can identify closing tags, like &lt;/A&gt;, &lt;/FORM&gt;, etc.
    /// </summary>
    public class EndTag : Tag
    {
        public override string Type
        {
            get { return TYPE; }
        }

        public new const string TYPE = "END_TAG";
        public const int ENDTAG_BEFORE_PARSING_STATE = 0;
        public const int ENDTAG_WAIT_FOR_SLASH_STATE = 1;
        public const int ENDTAG_BEGIN_PARSING_STATE = 2;
        public const int ENDTAG_FINISHED_PARSING_STATE = 3;

        public EndTag(TagData tagData) : base(tagData)
        {
        }

        /// <summary> Locate the end tag withing the input string, by parsing from the given position
        /// </summary>
        /// <param name="input">Input String
        /// </param>
        /// <param name="position">Position to start parsing from
        ///
        /// </param>
        public static AbstractNode Find(string input, int position)
        {
            int state = ENDTAG_BEFORE_PARSING_STATE;
            StringBuilder tagContents = new StringBuilder();
            int tagBegin = 0;
            int tagEnd = 0;
            int inputLen = input.Length;
            char ch;
            int i;
            for (i = position; (i < inputLen && state != ENDTAG_FINISHED_PARSING_STATE); i++)
            {
                ch = input[i];
                if (ch == '>' && state == ENDTAG_BEGIN_PARSING_STATE)
                {
                    state = ENDTAG_FINISHED_PARSING_STATE;
                    tagEnd = i;
                }
                if (state == ENDTAG_BEGIN_PARSING_STATE)
                {
                    tagContents.Append(ch);
                }
                if (state == ENDTAG_WAIT_FOR_SLASH_STATE)
                {
                    if (ch == '/')
                    {
                        state = ENDTAG_BEGIN_PARSING_STATE;
                    }
                    else
                        return null;
                }

                if (ch == '<')
                {
                    if (state == ENDTAG_BEFORE_PARSING_STATE)
                    {
                        // Transition from State 0 to State 1 - Record data till > is encountered
                        tagBegin = i;
                        state = ENDTAG_WAIT_FOR_SLASH_STATE;
                    }
                    else if (state == ENDTAG_BEGIN_PARSING_STATE)
                    {
                        state = ENDTAG_FINISHED_PARSING_STATE;
                        tagEnd = i;
                    }
                }
                else if (state == ENDTAG_BEFORE_PARSING_STATE)
                    // text before the end tag
                    return (null);
            }
            // If parsing did not complete, it might be possible to accept
            if (state == ENDTAG_BEGIN_PARSING_STATE)
            {
                tagEnd = i;
                state = ENDTAG_FINISHED_PARSING_STATE;
            }
            if (state == ENDTAG_FINISHED_PARSING_STATE)
                return new EndTag(new TagData(tagBegin, tagEnd, tagContents.ToString(), input));
            else
                return null;
        }

        public override string ToPlainTextString()
        {
            return "";
        }

        public override string ToHtml()
        {
            return "</" + TagName + ">";
        }

        public override string ToString()
        {
            return "EndTag : " + tagContents + "; begins at : " + ElementBegin + "; ends at : " + ElementEnd;
        }

        public override void Accept(NodeVisitor visitor)
        {
            visitor.VisitEndTag(this);
        }
    }
}
