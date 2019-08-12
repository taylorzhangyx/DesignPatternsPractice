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
using NodeReader = org.htmlparser.NodeReader;
using Parser = org.htmlparser.Parser;
using Tag = org.htmlparser.tags.Tag;
using TagData = org.htmlparser.tags.data.TagData;
using ParserFeedback = org.htmlparser.util.ParserFeedback;

namespace org.htmlparser.parserHelper
{
    public class TagParser
    {
        public const int TAG_BEFORE_PARSING_STATE = 1;
        public const int TAG_BEGIN_PARSING_STATE = 1 << 2;
        public const int TAG_FINISHED_PARSING_STATE = 1 << 3;
        public const int TAG_ILLEGAL_STATE = 1 << 4;
        public const int TAG_IGNORE_DATA_STATE = 1 << 5;
        public const int TAG_IGNORE_BEGIN_TAG_STATE = 1 << 6;
        public const int TAG_IGNORE_CHAR_SINGLE_QUOTE = 1 << 7;

        public const string ENCOUNTERED_QUERY_MESSAGE =
            "TagParser : Encountered > after a query. Accepting without correction and continuing parsing";

        private ParserFeedback feedback;

        public TagParser(ParserFeedback feedback)
        {
            this.feedback = feedback;
        }

        public virtual Tag Find(NodeReader reader, string input, int position)
        {
            int state = TAG_BEFORE_PARSING_STATE;
            int i = position;
            char ch;
            char[] ignorechar = new char[1]; // holds the character we're looking for when in TAG_IGNORE_DATA_STATE
            Tag tag = new Tag(new TagData(position, 0, reader.LastLineNumber, 0, "", input, "", false));

            Bool encounteredQuery = new Bool(false);
            while (i < tag.TagLine.Length && state != TAG_FINISHED_PARSING_STATE && state != TAG_ILLEGAL_STATE)
            {
                ch = tag.TagLine[i];
                state = AutomataInput(encounteredQuery, i, state, ch, tag, i, ignorechar);
                i = IncrementCounter(i, reader, state, tag);
            }
            if (state == TAG_FINISHED_PARSING_STATE)
            {
                string tagLine = tag.TagLine;
                if (i > 1 && tagLine[i - 2] == '/')
                {
                    tag.EmptyXmlTag = true;
                    string tagContents = tag.Text;
                    tag.Text = tagContents.Substring(0, tagContents.Length - 1);
                }
                return tag;
            }
            else
                return null;
        }

        private int AutomataInput(Bool encounteredQuery, int i, int state, char ch, Tag tag, int pos, char[] ignorechar)
        {
            state = CheckIllegalState(i, state, ch, tag);
            state = CheckFinishedState(encounteredQuery, i, state, ch, tag, pos);
            state = ToggleIgnoringState(state, ch, ignorechar);
            if (state == TAG_BEFORE_PARSING_STATE && ch != '<')
            {
                state = TAG_ILLEGAL_STATE;
            }
            if (state == TAG_IGNORE_DATA_STATE && ch == '<')
            {
                // If the next tag char is is close tag, then
                // this is legal, we should continue
                if (!IsWellFormedTag(tag, pos))
                    state = TAG_IGNORE_BEGIN_TAG_STATE;
            }
            if (state == TAG_IGNORE_BEGIN_TAG_STATE && ch == '>')
            {
                state = TAG_IGNORE_DATA_STATE;
            }
            CheckIfAppendable(encounteredQuery, state, ch, tag);
            state = CheckBeginParsingState(i, state, ch, tag);

            return state;
        }

        private int CheckBeginParsingState(int i, int state, char ch, Tag tag)
        {
            if (ch == '<' && (state == TAG_BEFORE_PARSING_STATE || state == TAG_ILLEGAL_STATE))
            {
                // Transition from State 0 to State 1 - Record data till > is encountered
                tag.TagBegin = i;
                state = TAG_BEGIN_PARSING_STATE;
            }
            return state;
        }

        private bool IsWellFormedTag(Tag tag, int pos)
        {
            string inputLine = tag.TagLine;
            int closeTagPos = inputLine.IndexOf((System.Char) '>', pos + 1);
            int openTagPos = inputLine.IndexOf((System.Char) '<', pos + 1);
            return openTagPos > closeTagPos || (openTagPos == -1 && closeTagPos != -1);
        }

        private int CheckFinishedState(Bool encounteredQuery, int i, int state, char ch, Tag tag, int pos)
        {
            if (ch == '>')
            {
                if (state == TAG_BEGIN_PARSING_STATE)
                {
                    state = TAG_FINISHED_PARSING_STATE;
                    tag.TagEnd = i;
                }
                else if (state == TAG_IGNORE_DATA_STATE)
                {
                    if (encounteredQuery.Boolean)
                    {
                        encounteredQuery.Boolean = false;
                        feedback.Info(ENCOUNTERED_QUERY_MESSAGE);
                        return state;
                    }
                    // Now, either this is a valid > input, and should be ignored,
                    // or it is a mistake in the html, in which case we need to correct it *sigh*
                    if (IsWellFormedTag(tag, pos))
                        return state;

                    state = TAG_FINISHED_PARSING_STATE;
                    tag.TagEnd = i;
                    // Do Correction
                    // Correct the tag - assuming its grouped into name value pairs
                    // Remove all inverted commas.
                    CorrectTag(tag);

                    System.Text.StringBuilder msg = new System.Text.StringBuilder();
                    msg.Append("HTMLTagParser : Encountered > inside inverted commas in line \n");
                    msg.Append(tag.TagLine);
                    msg.Append(", location ");
                    msg.Append(i);
                    msg.Append("\n");
                    for (int j = 0; j < i; j++)
                        msg.Append(' ');
                    msg.Append('^');
                    msg.Append("\nAutomatically corrected.");
                    feedback.Warning(msg.ToString());
                }
            }
            else if (ch == '<' && state == TAG_BEGIN_PARSING_STATE && tag.Text[0] != '%')
            {
                state = TAG_FINISHED_PARSING_STATE;
                tag.TagEnd = i - 1;
                i--;
            }
            return state;
        }

        private void CheckIfAppendable(Bool encounteredQuery, int state, char ch, Tag tag)
        {
            if (state == TAG_IGNORE_DATA_STATE || state == TAG_BEGIN_PARSING_STATE ||
                state == TAG_IGNORE_BEGIN_TAG_STATE)
            {
                if (ch == '?')
                    encounteredQuery.Boolean = true;
                tag.Append(ch);
            }
        }

        private int CheckIllegalState(int i, int state, char ch, Tag tag)
        {
            if (ch == '/' && i > 0 && tag.TagLine[i - 1] == '<' &&
                state != TAG_IGNORE_DATA_STATE &&
                state != TAG_IGNORE_BEGIN_TAG_STATE)
            {
                state = TAG_ILLEGAL_STATE;
            }

            return state;
        }

        public virtual void CorrectTag(Tag tag)
        {
            string tempText = tag.Text;
            StringBuilder absorbedText = new StringBuilder();
            foreach (char c in tempText)
            {
                if (c != '"')
                    absorbedText.Append(c);
            }
            // Go into the next stage.
            StringBuilder result = InsertInvertedCommasCorrectly(absorbedText);
            tag.Text = result.ToString();
        }

        public virtual StringBuilder InsertInvertedCommasCorrectly(StringBuilder absorbedText)
        {
            StringBuilder result = new StringBuilder();
            StringTokenizer tok = new StringTokenizer(absorbedText.ToString(), "=", false);
            string token;
            token = (string) tok.NextToken();
            result.Append(token + "=");
            for (; tok.HasMoreTokens();)
            {
                token = (string) tok.NextToken();
                token = PruneSpaces(token);
                result.Append('"');
                int lastIndex = token.LastIndexOf((System.Char) ' ');
                if (lastIndex != - 1 && tok.HasMoreTokens())
                {
                    result.Append(token.Substring(0, (lastIndex) - (0)));
                    result.Append('"');
                    result.Append(token.Substring(lastIndex, (token.Length) - (lastIndex)));
                }
                else
                    result.Append(token + '"');
                if (tok.HasMoreTokens())
                    result.Append("=");
            }
            return result;
        }

        public static string PruneSpaces(string token)
        {
            int firstSpace;
            int lastSpace;
            firstSpace = token.IndexOf((System.Char) ' ');
            while (firstSpace == 0)
            {
                token = token.Substring(1, (token.Length) - (1));
                firstSpace = token.IndexOf((System.Char) ' ');
            }
            lastSpace = token.LastIndexOf((System.Char) ' ');
            while (lastSpace == token.Length - 1)
            {
                token = token.Substring(0, (token.Length - 1) - (0));
                lastSpace = token.LastIndexOf((System.Char) ' ');
            }
            return token;
        }

        /// <summary> Check for quote character (" or ') and switch to TAG_IGNORE_DATA_STATE
        /// or back out to TAG_BEGIN_PARSING_STATE.
        /// </summary>
        /// <param name="state">The current state.
        /// </param>
        /// <param name="ch">The character to test.
        /// </param>
        /// <param name="ignorechar">The character that caused entry to TAG_IGNORE_DATA_STATE.
        ///
        /// </param>
        private int ToggleIgnoringState(int state, char ch, char[] ignorechar)
        {
            if (state == TAG_IGNORE_DATA_STATE)
            {
                if (ch == ignorechar[0])
                    state = TAG_BEGIN_PARSING_STATE;
            }
            else if (state == TAG_BEGIN_PARSING_STATE)
                if (ch == '"' || ch == '\'')
                {
                    state = TAG_IGNORE_DATA_STATE;
                    ignorechar[0] = ch;
                }

            return (state);
        }

        public virtual int IncrementCounter(int i, NodeReader reader, int state, Tag tag)
        {
            string nextLine = null;
            if ((state == TAG_BEGIN_PARSING_STATE || state == TAG_IGNORE_DATA_STATE ||
                 state == TAG_IGNORE_BEGIN_TAG_STATE) && i == tag.TagLine.Length - 1)
            {
                // The while loop below is a bug fix contributed by
                // Annette Doyle - see testcase HTMLImageScannerTest.testImageTagOnMultipleLines()
                // Further modified by Somik Raha, to remove bug - HTMLTagTest.testBrokenTag
                int numLinesAdvanced = 0;
                do
                {
                    nextLine = reader.GetNextLine();
                    numLinesAdvanced++;
                } while (nextLine != null && nextLine.Length == 0);
                if (nextLine == null)
                {
                    // This means we have a broken tag. Fill in an end tag symbol here.
                    nextLine = ">";
                }
                else
                {
                    // This means this is just a new line, hence add the new line character
                    tag.Append(Parser.LineSeparator);
                }

                // Ensure blank lines are included in tag's 'tagLines'
                while (--numLinesAdvanced > 0)
                    tag.TagLine = "";

                // We need to continue parsing to the next line
                tag.TagLine = nextLine;
                i = - 1;
            }
            return ++i;
        }

        // Class provided for thread safety in TagParser
        internal class Bool
        {
            public bool Boolean
            {
                get { return boolValue; }

                set { this.boolValue = value; }
            }

            private bool boolValue;

            internal Bool(bool boolValue)
            {
                this.boolValue = boolValue;
            }
        }
    }
}
