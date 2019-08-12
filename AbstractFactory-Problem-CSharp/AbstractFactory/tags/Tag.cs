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
using System.Collections;
using System.Collections.Specialized;
using AbstractNode = org.htmlparser.AbstractNode;
using NodeReader = org.htmlparser.NodeReader;
using AttributeParser = org.htmlparser.parserHelper.AttributeParser;
using TagParser = org.htmlparser.parserHelper.TagParser;
using TagScanner = org.htmlparser.scanners.TagScanner;
using TagData = org.htmlparser.tags.data.TagData;
using NodeList = org.htmlparser.util.NodeList;
using ParserException = org.htmlparser.util.ParserException;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser.tags
{
    /// <summary> Tag represents a generic tag. This class allows users to register specific
    /// tag scanners, which can identify links, or image references. This tag asks the
    /// scanners to run over the text, and identify. It can be used to dynamically
    /// configure a parser.
    /// </summary>
    /// <author>  Kaarle Kaila 23.10.2001
    ///
    /// </author>
    public class Tag : AbstractNode
    {
        /// <summary> Gets the attributes in the tag.
        /// </summary>
        /// <returns> Returns a Hashtable of attributes
        ///
        /// </returns>
        /// <summary> Sets the parsed.
        /// </summary>
        /// <param name="parsed">The parsed to set
        ///
        /// </param>
        public Hashtable Attributes
        {
            get
            {
                if (attributes == null)
                    attributes = ParseAttributes();
                return attributes;
            }
            set { this.attributes = value; }
        }

        public string TagName
        {
            get { return (string) Attributes[TAGNAME]; }
        }

        /// <summary> Returns the line where the tag was found
        /// </summary>
        /// <returns> string
        ///
        /// </returns>
        public string TagLine
        {
            get { return tagLine; }
            set
            {
                tagLine = value;

                // Note: Incur the overhead of resizing each time (versus
                // preallocating a larger array), since the average tag
                // generally doesn't span multiple lines
                string[] newTagLines = new string[tagLines.Length + 1];
                for (int i = 0; i < tagLines.Length; i++)
                    newTagLines[i] = tagLines[i];
                newTagLines[tagLines.Length] = value;
                tagLines = newTagLines;
            }
        }

        /// <summary> Returns the combined text of all the lines spanned by this tag
        /// </summary>
        /// <returns> string
        ///
        /// </returns>
        public string[] TagLines
        {
            get { return tagLines; }
        }

        /// <summary> Return the text contained in this tag
        /// </summary>
        public override string Text
        {
            get { return tagContents.ToString(); }
            set { tagContents = new StringBuilder(value); }
        }

        /// <summary> Return the scanner associated with this tag.
        /// </summary>
        public TagScanner ThisScanner
        {
            get { return thisScanner; }
            set { thisScanner = value; }
        }

        /// <summary> Gets the nodeBegin.
        /// </summary>
        /// <returns> The nodeBegin value.
        ///
        /// </returns>
        /// <summary> Sets the nodeBegin.
        /// </summary>
        ///
        public int TagBegin
        {
            get { return (nodeBegin); }
            set { this.nodeBegin = value; }
        }

        /// <summary> Gets the nodeEnd.
        /// </summary>
        /// <returns> The nodeEnd value.
        ///
        /// </returns>
        /// <summary> Sets the nodeEnd.
        /// </summary>
        public int TagEnd
        {
            get { return (nodeEnd); }
            set { this.nodeEnd = value; }
        }

        /// <summary> Gets the line number on which this tag starts.
        /// </summary>
        /// <returns> the start line number
        ///
        /// </returns>
        public int TagStartLine
        {
            get { return startLine; }
        }

        /// <summary> Gets the line number on which this tag ends.
        /// </summary>
        /// <returns> the end line number
        ///
        /// </returns>
        public int TagEndLine
        {
            get { return startLine + tagLines.Length - 1; }
        }

        /// <summary> Sets the tagParser.
        /// </summary>
        public static TagParser TagParser
        {
            set { Tag.tagParser = value; }
        }

        public virtual string Type
        {
            get { return TYPE; }
        }

        /// <summary> Is this an empty xml tag of the form<br>
        /// &lt;tag/&gt;
        /// </summary>
        /// <returns> bool
        ///
        /// </returns>
        public bool EmptyXmlTag
        {
            get { return emptyXmlTag; }
            set { this.emptyXmlTag = value; }
        }

        public const string TYPE = "TAG";

        /// <summary> Constants used as value for the value of the tag name
        /// in parseParameters  (Kaarle Kaila 3.8.2001)
        /// </summary>
        public const string TAGNAME = "$<TAGNAME>$";

        public const string EMPTYTAG = "$<EMPTYTAG>$";
        private const int TAG_BEFORE_PARSING_STATE = 1;
        private const int TAG_BEGIN_PARSING_STATE = 2;
        private const int TAG_FINISHED_PARSING_STATE = 3;
        private const int TAG_ILLEGAL_STATE = 4;
        private const int TAG_IGNORE_DATA_STATE = 5;
        private const int TAG_IGNORE_BEGIN_TAG_STATE = 6;
        private const string EMPTY_STRING = "";

        private static AttributeParser paramParser;
        private static TagParser tagParser;

        /// <summary> Tag contents will have the contents of the comment tag.
        /// </summary>
        protected StringBuilder tagContents;

        private bool emptyXmlTag = false;

        /// <summary> Tag parameters parsed into this Hashtable
        /// not implemented yet
        /// added by Kaarle Kaila 23.10.2001
        /// </summary>
        protected Hashtable attributes = null;

        /// <summary> Scanner associated with this tag (useful for extraction of filtering data from a
        /// HTML node)
        /// </summary>
        protected TagScanner thisScanner = null;

        private string tagLine;

        /// <summary> The combined text of all the lines spanned by this tag
        /// </summary>
        private string[] tagLines;

        /// <summary> The line number on which this tag starts
        /// </summary>
        private int startLine;

        /// <summary> Set of tags that breaks the flow.
        /// </summary>
        protected static StringCollection breakTags;

        /// <summary> Set the Tag with the beginning posn, ending posn and tag contents (in
        /// a tagData object.
        /// </summary>
        /// <param name="tagData">The data for this tag
        ///
        /// </param>
        public Tag(TagData tagData) : base(tagData.TagBegin, tagData.TagEnd)
        {
            this.startLine = tagData.StartLine;
            this.tagContents = new StringBuilder();
            this.tagContents.Append(tagData.TagContents);
            this.tagLine = tagData.TagLine;
            this.tagLines = new string[] {tagData.TagLine};
            this.emptyXmlTag = tagData.EmptyXmlTag;
        }

        public virtual void Append(char ch)
        {
            tagContents.Append(ch);
        }

        public virtual void Append(string ch)
        {
            tagContents.Append(ch);
        }

        /// <summary> Locate the tag withing the input string, by parsing from the given position
        /// </summary>
        /// <param name="reader">HTML reader to be provided so as to allow reading of next line
        /// </param>
        /// <param name="input">Input String
        /// </param>
        /// <param name="position">Position to start parsing from
        ///
        /// </param>
        public static Tag Find(NodeReader reader, string input, int position)
        {
            return tagParser.Find(reader, input, position);
        }

        /// <summary> This method is not to be called by any scanner or tag. It is
        /// an expensive method, hence it has been made private. However,
        /// there might be some circumstances when a scanner wishes to force
        /// parsing of attributes over and above what has already been parsed.
        /// To make the choice clear - we have a method - redoParseAttributes(),
        /// which can be used.
        /// </summary>
        /// <returns> Hashtable
        ///
        /// </returns>
        private Hashtable ParseAttributes()
        {
            return paramParser.ParseAttributes(this);
        }

        /// <summary> In case the tag is parsed at the scan method this will return value of a
        /// parameter not implemented yet
        /// </summary>
        /// <param name="name">of parameter
        ///
        /// </param>
        public string this[string attribute]
        {
            get { return (string) Attributes[attribute.ToUpper()]; }
            set { Attributes[attribute.ToUpper()] = value; }
        }

        /// <summary> Extract the first word from the given string.
        /// Words are delimited by whitespace or equals signs.
        /// </summary>
        /// <param name="s">The string to get the word from.
        /// </param>
        /// <returns> The first word.
        ///
        /// </returns>
        public static string ExtractWord(string s)
        {
            int length;
            bool parse;
            char ch;
            StringBuilder ret;

            length = s.Length;
            ret = new StringBuilder(length);
            parse = true;
            for (int i = 0; i < length && parse; i++)
            {
                ch = s[i];
                if (System.Char.IsWhiteSpace(ch) || ch == '=')
                    parse = false;
                else
                    ret.Append(System.Char.ToUpper(ch));
            }

            return (ret.ToString());
        }

        /// <summary> Scan the tag to see using the scanners, and attempt identification.
        /// </summary>
        /// <param name="url">URL at which HTML page is located
        /// </param>
        /// <param name="reader">The NodeReader that is to be used for reading the url
        ///
        /// </param>
        public virtual AbstractNode Scan(IDictionary scanners, string url, NodeReader reader)
        {
            if (tagContents.Length == 0)
                return this;
            try
            {
                bool found = false;
                AbstractNode retVal = null;
                // Find the first word in the scanners
                string firstWord = ExtractWord(tagContents.ToString());
                // Now, get the scanner associated with this.
                TagScanner scanner = (TagScanner) scanners[firstWord];

                // Now do a deep check
                if (scanner != null && scanner.Evaluate(tagContents.ToString(), reader.PreviousOpenScanner))
                {
                    found = true;
                    TagScanner save;
                    save = reader.PreviousOpenScanner;
                    reader.PreviousOpenScanner = scanner;
                    retVal = scanner.CreateScannedNode(this, url, reader, tagLine);
                    reader.PreviousOpenScanner = save;
                }

                if (!found)
                    return this;
                else
                    return retVal;
            }
            catch (System.Exception e)
            {
                string errorMsg;
                if (tagContents != null)
                    errorMsg = tagContents.ToString();
                else
                    errorMsg = "null";
                throw new ParserException(
                    "Tag.scan() : Error while scanning tag, tag contents = " + errorMsg + ", tagLine = " + tagLine, e);
            }
        }

        public override string ToPlainTextString()
        {
            return EMPTY_STRING;
        }

        /// <summary> A call to a tag's ToHTML() method will render it in HTML
        /// Most tags that do not have children and inherit from Tag,
        /// do not need to override ToHTML().
        /// </summary>
        /// <seealso cref="">org.htmlparser.Node#ToHTML()
        ///
        /// </seealso>
        public override string ToHtml()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<");
            sb.Append(TagName);
            if (ContainsMoreThanOneKey())
                sb.Append(" ");
            string value;
            string empty = null;
            int i = 0;
            foreach (string key in attributes.Keys)
            {
                i++;
                if (!key.Equals(TAGNAME))
                {
                    if (key.Equals(EMPTYTAG))
                    {
                        empty = "/";
                    }
                    else
                    {
                        value = this[key];
                        sb.Append(key + "=\"" + value + "\"");
                        if (i < attributes.Count)
                            sb.Append(" ");
                    }
                }
            }
            if (empty != null)
                sb.Append(empty);
            if (EmptyXmlTag)
                sb.Append("/");
            sb.Append(">");
            return sb.ToString();
        }

        private bool ContainsMoreThanOneKey()
        {
            return attributes.Keys.Count > 1;
        }

        /// <summary> Print the contents of the tag
        /// </summary>
        public override string ToString()
        {
            return "Begin Tag : " + tagContents + "; begins at : " + ElementBegin + "; ends at : " + ElementEnd;
        }

        /// <summary> Determines if the given tag breaks the flow of text.
        /// </summary>
        /// <returns> <code>true</code> if following text would start on a new line,
        /// <code>false</code> otherwise.
        ///
        /// </returns>
        public virtual bool BreaksFlow()
        {
            return (breakTags.Contains(Text.ToUpper()));
        }

        /// <summary> This method verifies that the current tag matches the provided
        /// filter. The match is based on the string object and not its contents,
        /// so ensure that you are using static final filter strings provided
        /// in the tag classes.
        /// </summary>
        /// <seealso cref="">String)
        ///
        /// </seealso>
        public override void CollectInto(NodeList collectionList, string filter)
        {
            if (thisScanner != null && thisScanner.Filter == filter)
                collectionList.Add(this);
        }

        /// <summary> Sometimes, a scanner may need to request a re-evaluation of the
        /// attributes in a tag. This may happen when there is some correction
        /// activity. An example of its usage can be found in ImageTag.
        /// <br>
        /// <B>Note:<B> This is an intensive task, hence call only when
        /// really necessary
        /// </summary>
        /// <returns> Hashtable
        ///
        /// </returns>
        public virtual Hashtable RedoParseAttributes()
        {
            return ParseAttributes();
        }

        public override void Accept(NodeVisitor visitor)
        {
            visitor.VisitTag(this);
        }

        static Tag()
        {
            paramParser = new AttributeParser();
            {
                breakTags = new StringCollection();
                breakTags.Add("BLOCKQUOTE");
                breakTags.Add("BODY");
                breakTags.Add("BR");
                breakTags.Add("CENTER");
                breakTags.Add("DD");
                breakTags.Add("DIR");
                breakTags.Add("DIV");
                breakTags.Add("DL");
                breakTags.Add("DT");
                breakTags.Add("FORM");
                breakTags.Add("H1");
                breakTags.Add("H2");
                breakTags.Add("H3");
                breakTags.Add("H4");
                breakTags.Add("H5");
                breakTags.Add("H6");
                breakTags.Add("HEAD");
                breakTags.Add("HR");
                breakTags.Add("HTML");
                breakTags.Add("ISINDEX");
                breakTags.Add("LI");
                breakTags.Add("MENU");
                breakTags.Add("NOFRAMES");
                breakTags.Add("OL");
                breakTags.Add("P");
                breakTags.Add("PRE");
                breakTags.Add("TD");
                breakTags.Add("TH");
                breakTags.Add("TITLE");
                breakTags.Add("UL");
            }
        }
    }
}
