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


using System;
using System.Text;
using org.htmlparser.util;
using org.htmlparser.visitors;

namespace org.htmlparser
{
    /// <summary> Normal text in the html document is identified and represented by this class.
    /// </summary>
    public class StringNode : AbstractNode
    {
        /// <summary> Returns the text of the string line
        /// </summary>
        /// <summary> Sets the string contents of the node.
        /// </summary>
        /// <param name="The">new text for the node.
        ///
        /// </param>
        public override string Text
        {
            get { return textBuffer.ToString(); }
            set { textBuffer = new StringBuilder(value); }
        }

        public const string STRING_FILTER = "-string";

        /// <summary> The text of the string.
        /// </summary>
        protected StringBuilder textBuffer;

        ///
        /// <summary> Constructor takes in the text string, beginning and ending posns.
        /// </summary>
        /// <param name="text">The contents of the string line
        /// </param>
        /// <param name="textBegin">The beginning position of the string
        /// </param>
        /// <param name="textEnd">The ending positiong of the string
        ///
        /// </param>
        public StringNode(StringBuilder textBuffer, int textBegin, int textEnd) : base(textBegin, textEnd)
        {
            this.textBuffer = textBuffer;
        }

        public override string ToPlainTextString()
        {
            return textBuffer.ToString();
        }

        public override string ToHtml()
        {
            return ToPlainTextString();
        }

        public override string ToString()
        {
            return "Text = " + Text + "; begins at : " + ElementBegin + "; ends at : " + ElementEnd;
        }

        public override void CollectInto(NodeList collectionList, string filter)
        {
            if (filter == STRING_FILTER)
                collectionList.Add(this);
        }

        public override void Accept(NodeVisitor visitor)
        {
            visitor.VisitStringNode(this);
        }

        public static Node CreateStringNode(StringBuilder textBuffer, int textBegin, int textEnd, bool shouldDecode)
        {
            Node node = new StringNode(textBuffer, textBegin, textEnd);
            if (shouldDecode)
                node = new org.htmlparser.decorators.DecodingNode(node);

            return (node);
        }
    }
}
