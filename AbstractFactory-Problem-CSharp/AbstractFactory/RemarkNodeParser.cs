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

namespace org.htmlparser
{
    public class RemarkNodeParser
    {
        public const int REMARK_NODE_BEFORE_PARSING_STATE = 0;
        public const int REMARK_NODE_OPENING_ANGLE_BRACKET_STATE = 1;
        public const int REMARK_NODE_EXCLAMATION_RECEIVED_STATE = 2;
        public const int REMARK_NODE_FIRST_DASH_RECEIVED_STATE = 3;
        public const int REMARK_NODE_ACCEPTING_STATE = 4;
        public const int REMARK_NODE_CLOSING_FIRST_DASH_RECEIVED_STATE = 5;
        public const int REMARK_NODE_CLOSING_SECOND_DASH_RECEIVED_STATE = 6;
        public const int REMARK_NODE_ACCEPTED_STATE = 7;
        public const int REMARK_NODE_ILLEGAL_STATE = 8;
        public const int REMARK_NODE_FINISHED_PARSING_STATE = 2;

        /// <summary> Locate the remark tag withing the input string, by parsing from the given position
        /// </summary>
        /// <param name="reader">HTML reader to be provided so as to allow reading of next line
        /// </param>
        /// <param name="input">Input String
        /// </param>
        /// <param name="position">Position to start parsing from
        ///
        /// </param>
        public virtual RemarkNode Find(NodeReader reader, string input, int position)
        {
            int state = REMARK_NODE_BEFORE_PARSING_STATE;
            System.Text.StringBuilder tagContents = new System.Text.StringBuilder();
            int tagBegin = 0;
            int tagEnd = 0;
            int i = position;
            int inputLen = input.Length;
            char ch, prevChar = ' ';
            while (i < inputLen && state < REMARK_NODE_ACCEPTED_STATE)
            {
                ch = input[i];
                if (state == REMARK_NODE_CLOSING_SECOND_DASH_RECEIVED_STATE)
                {
                    if (ch == '>')
                    {
                        state = REMARK_NODE_ACCEPTED_STATE;
                        tagEnd = i;
                    }
                    else if (ch == '-')
                    {
                        tagContents.Append(prevChar);
                    }
                    else
                    {
                        // Rollback last 2 characters (assumed same)
                        state = REMARK_NODE_ACCEPTING_STATE;
                        tagContents.Append(prevChar);
                        tagContents.Append(prevChar);
                    }
                }

                if (state == REMARK_NODE_CLOSING_FIRST_DASH_RECEIVED_STATE)
                {
                    if (ch == '-')
                    {
                        state = REMARK_NODE_CLOSING_SECOND_DASH_RECEIVED_STATE;
                    }
                    else
                    {
                        // Rollback
                        state = REMARK_NODE_ACCEPTING_STATE;
                        tagContents.Append(prevChar);
                    }
                }
                if (state == REMARK_NODE_ACCEPTING_STATE)
                {
                    if (ch == '-')
                    {
                        state = REMARK_NODE_CLOSING_FIRST_DASH_RECEIVED_STATE;
                    }
                }
                if (state == REMARK_NODE_ACCEPTING_STATE)
                {
                    // We can append contents now
                    tagContents.Append(ch);
                }

                if (state == REMARK_NODE_FIRST_DASH_RECEIVED_STATE)
                {
                    if (ch == '-')
                    {
                        state = REMARK_NODE_ACCEPTING_STATE;
                        // Do a lookahead and see if the next char is >
                        if (input.Length > i + 1 && input[i + 1] == '>')
                        {
                            state = REMARK_NODE_ACCEPTED_STATE;
                            tagEnd = i + 1;
                        }
                    }
                    else
                        state = REMARK_NODE_ILLEGAL_STATE;
                }
                if (state == REMARK_NODE_EXCLAMATION_RECEIVED_STATE)
                {
                    if (ch == '-')
                        state = REMARK_NODE_FIRST_DASH_RECEIVED_STATE;
                    else if (ch == '>')
                    {
                        state = REMARK_NODE_ACCEPTED_STATE;
                        tagEnd = i;
                    }
                    else
                        state = REMARK_NODE_ILLEGAL_STATE;
                }
                if (state == REMARK_NODE_OPENING_ANGLE_BRACKET_STATE)
                {
                    if (ch == '!')
                        state = REMARK_NODE_EXCLAMATION_RECEIVED_STATE;
                    else
                        state = REMARK_NODE_ILLEGAL_STATE;
                    // This is not a remark tag
                }
                if (state == REMARK_NODE_BEFORE_PARSING_STATE)
                {
                    if (ch == '<')
                    {
                        // Transition from State 0 to State 1 - Record data till > is encountered
                        tagBegin = i;
                        state = REMARK_NODE_OPENING_ANGLE_BRACKET_STATE;
                    }
                    else if (ch != ' ')
                    {
                        // Its not a space, hence this is probably a string node, not a remark node
                        state = REMARK_NODE_ILLEGAL_STATE;
                    }
                }
                //			if (state > REMARK_NODE_OPENING_ANGLE_BRACKET_STATE && state < REMARK_NODE_ACCEPTED_STATE && i == input.length() - 1)
                if (state >= REMARK_NODE_ACCEPTING_STATE && state < REMARK_NODE_ACCEPTED_STATE && i == input.Length - 1)
                {
                    // We need to continue parsing to the next line
                    tagContents.Append(Parser.LineSeparator);
                    do
                    {
                        input = reader.GetNextLine();
                    } while (input != null && input.Length == 0);
                    if (input != null)
                        inputLen = input.Length;
                    else
                        inputLen = - 1;
                    i = - 1;
                }
                if (state == REMARK_NODE_ILLEGAL_STATE)
                {
                    return null;
                }
                i++;
                prevChar = ch;
            }
            if (state == REMARK_NODE_ACCEPTED_STATE)
                return new RemarkNode(tagBegin, tagEnd, tagContents.ToString());
            else
                return null;
        }
    }
}
