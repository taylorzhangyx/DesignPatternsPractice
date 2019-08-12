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
using System.IO;
using System.Text;
using org.htmlparser.parserHelper;
using org.htmlparser.scanners;
using org.htmlparser.tags;
using org.htmlparser.util;

namespace org.htmlparser
{
    /**
	 * NodeReader builds on the StreamReader, providing methods to read one element
	 * at a time
	 */

    public class NodeReader
    {
        protected int posInLine = -1;
        protected string line;
        protected Node node = null;
        protected TagScanner previousOpenScanner = null;
        protected string url;
        private Parser parser;
        private int lineCount;
        private string previousLine;
        private StringParser stringParser;
        private RemarkNodeParser remarkNodeParser;
        private NodeList nextParsedNode;
        private bool dontReadNextLine = false;
        private TextReader myReader;

        private void InitBlock()
        {
            stringParser = new StringParser();
            remarkNodeParser = new RemarkNodeParser();
            nextParsedNode = new NodeList();
        }

        /**
		 * The constructor takes in a reader object, its length and the url to be read.
		 */

        public NodeReader(TextReader reader, int len, string url)
        {
            myReader = reader;
            InitBlock();
            this.url = url;
            this.parser = null;
            this.lineCount = 1;
        }

        /**
		 * This constructor basically overrides the existing constructor in the
		 * BufferedReader class.
		 * The URL defaults to an empty string.
		 * @see #NodeReader(Reader,int,string)
		 */

        public NodeReader(TextReader reader, int len) : this(reader, len, "")
        {
        }

        /**
		 * The constructor takes in a reader object, and the url to be read.
		 * The buffer size defaults to 8192.
		 * @see #NodeReader(Reader,int,string)
		 */

        public NodeReader(TextReader reader, string url) : this(reader, 8192, url)
        {
        }

        /**
		 * Get the url for this reader.
		 * @return The url specified in the constructor;
		 */

        public string URL
        {
            get { return (url); }
        }

        /**
		 * This method is intended to be called only by scanners, when a situation of dirty html has arisen,
		 * and action has been taken to correct the parsed tags. For e.g. if we have html of the form :
		 * <pre>
		 * <a href="somelink.html"><img src=...><td><tr><a href="someotherlink.html">...</a>
		 * </pre>
		 * Now to salvage the first link, we'd probably like to insert an end tag somewhere (typically before the
		 * second begin link tag). So that the parsing continues uninterrupted, we will need to change the existing
		 * line being parsed, to contain the end tag in it.
		 */

        public void ChangeLine(string line)
        {
            this.line = line;
        }

        public string CurrentLine
        {
            get { return line; }
        }

        /**
		 * Get the last line number that the reader has read
		 * @return int last line number read by the reader
		 */

        public int LastLineNumber
        {
            get { return lineCount - 1; }
        }

        /**
		 * This method is useful when designing your own scanners. You might need to find out what is the location where the
		 * reader has stopped last.
		 * @return int Last position read by the reader
		 */

        public int LastReadPosition
        {
            get
            {
                if (node != null)
                    return node.ElementEnd;
                else
                    return 0;
            }
        }

        /*
		 * Read the next line
		 * @return string containing the line
		 */

        public string GetNextLine()
        {
            try
            {
                previousLine = line;
                line = myReader.ReadLine();
                if (line != null)
                    lineCount++;
                else
                    myReader.Close();
                posInLine = 0;
                return line;
            }
            catch (IOException)
            {
                System.Console.Error.WriteLine("I/O Exception occurred while reading!");
            }
            return null;
        }

        /**
		 * Returns the parser object for which this reader exists
		 * @return org.htmlparser.Parser
		 */

        public Parser Parser
        {
            get { return parser; }

            set { parser = value; }
        }

        /// <summary> Gets the previousOpenScanner.
        /// </summary>
        /// <returns> Returns a TagScanner
        ///
        /// </returns>
        /// <summary> Sets the previousOpenScanner.
        /// </summary>
        /// <param name="previousOpenScanner">The previousOpenScanner to set
        ///
        /// </param>
        public TagScanner PreviousOpenScanner
        {
            get { return previousOpenScanner; }

            set { this.previousOpenScanner = value; }
        }

        public bool DontReadNextLine
        {
            get { return dontReadNextLine; }

            set { this.dontReadNextLine = value; }
        }

        public static string LineSeparator
        {
            get { return (Parser.LineSeparator); }

            set { Parser.LineSeparator = value; }
        }

        /// <summary> Returns the lineCount.
        /// </summary>
        /// <returns> int
        ///
        /// </returns>
        /// <summary> Sets the lineCount.
        /// </summary>
        /// <param name="lineCount">The lineCount to set
        ///
        /// </param>
        public int LineCount
        {
            get { return lineCount; }

            set { this.lineCount = value; }
        }

        /**
		 * Returns the previousLine.
		 * @return string
		 */

        public string PreviousLine
        {
            get { return previousLine; }
        }

        /// <summary> Returns the line.
        /// </summary>
        /// <returns> String
        ///
        /// </returns>
        public string Line
        {
            get { return line; }
        }

        public int PosInLine
        {
            set { this.posInLine = value; }
        }

        /**
		 * Returns the string parser.
		 */

        public StringParser StringParser
        {
            get { return stringParser; }
        }

        /**
		 * Returns true if the text at <code>pos</code> in <code>line</code> should be scanned as a tag.
		 * Basically an open angle followed by a known special character or a letter.
		 * @param line The current line being parsed.
		 * @param pos The position in the line to examine.
		 * @return <code>true</code> if we think this is the start of a tag.
		 */

        private bool BeginTag(string line, int pos)
        {
            char ch;
            bool ret;

            ret = false;

            if (pos + 2 <= line.Length)
            {
                if ('<' == line[pos])
                {
                    ch = line[pos + 1];
                    // the order of these tests might be optimized for speed
                    if ('/' == ch || '%' == ch || System.Char.IsLetter(ch) || '!' == ch)
                        ret = true;
                }
            }
            return (ret);
        }

        /**
		 * Read the next element
		 * @return Node - The next node
		 */

        public Node ReadElement()
        {
            return (ReadElement(false));
        }

        /**
		 * Read the next element
		 * @param balance_quotes If <code>true</code> string nodes are parsed
		 * paying attention to single and double quotes, such that tag-like
		 * strings are ignored if they are quoted.
		 * @return Node - The next node
		 */

        public Node ReadElement(bool balance_quotes)
        {
            try
            {
                if (nextParsedNode.Size > 0)
                {
                    node = nextParsedNode[0];
                    nextParsedNode.Remove(0);
                    return node;
                }

                if (ReadNextLine())
                {
                    do
                    {
                        line = GetNextLine();
                    } while (line != null && line.Length == 0);
                }
                else if (dontReadNextLine)
                {
                    dontReadNextLine = false;
                }
                else
                    posInLine = LastReadPosition + 1;

                if (line == null)
                    return null;

                if (BeginTag(line, posInLine))
                {
                    node = remarkNodeParser.Find(this, line, posInLine);
                    if (node != null)
                        return node;

                    node = Tag.Find(this, line, posInLine);
                    if (node != null)
                    {
                        Tag tag = (Tag) node;
                        try
                        {
                            node = tag.Scan(parser.Scanners, url, this);
                            return node;
                        }
                        catch (Exception e)
                        {
                            StringBuilder msgBuffer = new StringBuilder();
                            msgBuffer.Append(
                                "NodeReader.ReadElement() : Error occurred while trying to decipher the tag using scanners\n" +
                                "    Tag being processed : " + tag.TagName + "\n" +
                                "    Current Tag Line : " + tag.TagLine
                                );
                            AppendLineDetails(msgBuffer);
                            ParserException ex = new ParserException(msgBuffer.ToString(), e);

                            parser.Feedback.Error(msgBuffer.ToString(), ex);
                            throw ex;
                        }
                    }

                    node = EndTag.Find(line, posInLine);
                    if (node != null)
                        return node;
                }
                else
                {
                    node = stringParser.Find(this, line, posInLine, balance_quotes);
                    if (node != null)
                        return node;
                }

                return null;
            }
            catch (Exception e)
            {
                StringBuilder msgBuffer =
                    new StringBuilder("NodeReader.ReadElement() : Error occurred while trying to read the next element,");
                AppendLineDetails(msgBuffer);
                msgBuffer.Append("\n Caused by:\n").Append(msgBuffer.ToString());
                ParserException ex = new ParserException(msgBuffer.ToString(), e);
                parser.Feedback.Error(msgBuffer.ToString(), ex);
                throw ex;
            }
        }

        public void AppendLineDetails(StringBuilder msgBuffer)
        {
            msgBuffer.Append("\nat Line ");
            msgBuffer.Append(LineCount);
            msgBuffer.Append(" : ");
            msgBuffer.Append(Line);
            msgBuffer.Append("\nPrevious Line ").Append(LineCount - 1);
            msgBuffer.Append(" : ").Append(PreviousLine);
        }

        /**
		 * Do we need to read the next line ?
		 * @return true - yes/ false - no
		 */

        protected bool ReadNextLine()
        {
            if (dontReadNextLine)
                return false;

            if (posInLine == -1 || (line != null && node.ElementEnd + 1 >= line.Length))
                return true;
            else
                return false;
        }

        /**
		 * Adds the given node on the front of an internal list of pre-parsed nodes.
		 * Used in recursive calls where downstream nodes have been recognized in
		 * order to parse the current node.
		 * @param nextParsedNode The node that will be returned next by the reader.
		 */

        public void AddNextParsedNode(Node nextParsedNode)
        {
            this.nextParsedNode.Prepend(nextParsedNode);
        }
    }
}
