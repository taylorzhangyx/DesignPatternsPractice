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
using AbstractNode = org.htmlparser.AbstractNode;
using NodeReader = org.htmlparser.NodeReader;
using Parser = org.htmlparser.Parser;
using StringNode = org.htmlparser.StringNode;

namespace org.htmlparser.parserHelper
{
    public class StringParser
    {
        private const int BEFORE_PARSE_BEGINS_STATE = 0;
        private const int PARSE_HAS_BEGUN_STATE = 1;
        private const int PARSE_COMPLETED_STATE = 2;
        private const int PARSE_IGNORE_STATE = 3;

        /// <summary> Returns true if the text at <code>pos</code> in <code>line</code> should be scanned as a tag.
        /// Basically an open angle followed by a known special character or a letter.
        /// </summary>
        /// <param name="line">The current line being parsed.
        /// </param>
        /// <param name="pos">The position in the line to examine.
        /// </param>
        /// <returns> <code>true</code> if we think this is the start of a tag.
        ///
        /// </returns>
        private bool BeginTag(string line, int pos)
        {
            char ch;
            bool ret;

            ret = false;

            if (pos + 2 <= line.Length)
                if ('<' == line[pos])
                {
                    ch = line[pos + 1];
                    // the order of these tests might be optimized for speed
                    if ('/' == ch || '%' == ch || System.Char.IsLetter(ch) || '!' == ch)
                        ret = true;
                }

            return (ret);
        }

        /// <summary> Locate the StringNode within the input string, by parsing from the given position
        /// </summary>
        /// <param name="reader">HTML reader to be provided so as to allow reading of next line
        /// </param>
        /// <param name="input">Input String
        /// </param>
        /// <param name="position">Position to start parsing from
        /// </param>
        /// <param name="balance_quotes">If <code>true</code> enter ignoring state on
        /// encountering quotes.
        ///
        /// </param>
        public virtual Node Find(NodeReader reader, string input, int position, bool balance_quotes)
        {
            StringBuilder textBuffer = new StringBuilder();
            int state = BEFORE_PARSE_BEGINS_STATE;
            int textBegin = position;
            int textEnd = position;
            int inputLen = input.Length;
            char ch;
            char ignore_ender = '\"';
            for (int i = position; (i < inputLen && state != PARSE_COMPLETED_STATE); i++)
            {
                ch = input[i];
                if (ch == '<' && state != PARSE_IGNORE_STATE)
                {
                    if (BeginTag(input, i))
                    {
                        state = PARSE_COMPLETED_STATE;
                        textEnd = i - 1;
                    }
                }
                if (balance_quotes && (ch == '\'' || ch == '"'))
                {
                    if (state == PARSE_IGNORE_STATE)
                    {
                        if (ch == ignore_ender)
                            state = PARSE_HAS_BEGUN_STATE;
                    }
                    else
                    {
                        ignore_ender = ch;
                        state = PARSE_IGNORE_STATE;
                    }
                }
                if (state == BEFORE_PARSE_BEGINS_STATE)
                {
                    state = PARSE_HAS_BEGUN_STATE;
                }
                if (state == PARSE_HAS_BEGUN_STATE || state == PARSE_IGNORE_STATE)
                {
                    textBuffer.Append(input[i]);
                }
                // Patch by Cedric Rosa
                if (state == BEFORE_PARSE_BEGINS_STATE && i == inputLen - 1)
                    state = PARSE_HAS_BEGUN_STATE;
                if (state == PARSE_HAS_BEGUN_STATE && i == inputLen - 1)
                {
                    do
                    {
                        input = reader.GetNextLine();
                        if (input != null && input.Length == 0)
                            textBuffer.Append(Parser.LineSeparator);
                    } while (input != null && input.Length == 0);

                    if (input == null)
                    {
                        textEnd = i;
                        state = PARSE_COMPLETED_STATE;
                    }
                    else
                    {
                        textBuffer.Append(Parser.LineSeparator);
                        inputLen = input.Length;
                        i = - 1;
                    }
                }
            }
            return StringNode.CreateStringNode(textBuffer, textBegin, textEnd, reader.Parser.ShouldDecodeNodes);
        }
    }
}
