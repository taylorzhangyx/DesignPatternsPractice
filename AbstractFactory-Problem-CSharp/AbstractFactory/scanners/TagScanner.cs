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
using System.Collections;
using AbstractNode = org.htmlparser.AbstractNode;
using Node = org.htmlparser.Node;
using NodeReader = org.htmlparser.NodeReader;
using StringNode = org.htmlparser.StringNode;
using EndTag = org.htmlparser.tags.EndTag;
using Tag = org.htmlparser.tags.Tag;
using TagData = org.htmlparser.tags.data.TagData;
using ParserException = org.htmlparser.util.ParserException;
using ParserFeedback = org.htmlparser.util.ParserFeedback;

namespace org.htmlparser.scanners
{
    /// <summary> TagScanner is an abstract superclass which is subclassed to create specific
    /// scanners, that operate on a tag's strings, identify it, and can extract data from it.
    /// <br>
    /// If you wish to write your own scanner, then you must implement Scan().
    /// You MAY implement Evaluate() as well, if your evaluation logic is not based on a simple text match.
    /// You MUST implement the ID property, which identifies your scanner uniquely among the scanners.
    /// *
    /// <br>
    /// Also, you have a feedback object provided to you, should you want to send log messages. This object is
    /// instantiated by Parser when a scanner is added to its collection.
    /// *
    /// </summary>
    public abstract class TagScanner
    {
        public string Filter
        {
            get { return filter; }
        }

        public abstract string[] ID { get; }

        public ParserFeedback Feedback
        {
            set { this.feedback = value; }
        }

        /// <summary> A filter which is used to associate this tag. The filter contains a string
        /// that is used to match which tags are to be allowed to pass through. This can
        /// be useful when one wishes to dynamically filter out all tags except one type
        /// which may be programmed later than the parser. Is also useful for command line
        /// implementations of the parser.
        /// </summary>
        protected string filter;

        /// <summary> ParserFeedback object automatically initialized
        /// </summary>
        protected ParserFeedback feedback;

        /// <summary> Default Constructor, automatically registers the scanner into a static array of
        /// scanners inside Tag
        /// </summary>
        public TagScanner()
        {
            this.filter = "";
        }

        /// <summary> This constructor automatically registers the scanner, and sets the filter for this
        /// tag.
        /// </summary>
        /// <param name="filter">The filter which will allow this tag to pass through.
        ///
        /// </param>
        public TagScanner(string filter)
        {
            this.filter = filter;
        }

        public virtual string Absorb(string s, char c)
        {
            int index = s.IndexOf((System.Char) c);
            if (index != - 1)
                s = s.Substring(index + 1, (s.Length) - (index + 1));
            return s;
        }

        /// <summary> Remove whitespace from the front of the given string.
        /// </summary>
        /// <param name="s">The string to trim.
        /// </param>
        /// <returns> Either the same string or a string with whitespace chopped off.
        ///
        /// </returns>
        public static string AbsorbLeadingBlanks(string s)
        {
            int length;
            int i;
            string ret;

            i = 0;
            length = s.Length;
            while (i < length && System.Char.IsWhiteSpace(s[i]))
                i++;
            if (0 == i)
                ret = s;
            else if (length == i)
                ret = "";
            else
                ret = s.Substring(i);

            return (ret);
        }

        /// <summary> This method is used to decide if this scanner can handle this tag type. If the
        /// evaluation returns true, the calling side makes a call to Scan().
        /// <strong>This method has to be implemented meaningfully only if a first-word match with
        /// the scanner id does not imply a match (or extra processing needs to be done).
        /// Default returns true</strong>
        /// </summary>
        /// <param name="s">The complete text contents of the Tag.
        /// </param>
        /// <param name="previousOpenScanner">Indicates any previous scanner which hasn't completed, before the current
        /// scan has begun, and hence allows us to write scanners that can work with dirty html
        ///
        /// </param>
        public virtual bool Evaluate(string s, TagScanner previousOpenScanner)
        {
            return true;
        }

        public static string ExtractXMLData(Node node, string tagName, NodeReader reader)
        {
            try
            {
                string xmlData = "";

                bool xmlTagFound = IsXMLTagFound(node, tagName);
                if (xmlTagFound)
                {
                    try
                    {
                        do
                        {
                            node = reader.ReadElement();
                            if (node != null)
                            {
                                if (node is StringNode)
                                {
                                    StringNode stringNode = (StringNode) node;
                                    if (xmlData.Length > 0)
                                        xmlData += " ";
                                    xmlData += stringNode.Text;
                                }
                                else if (!(node is org.htmlparser.tags.EndTag))
                                    xmlTagFound = false;
                            }
                        } while (node is StringNode);
                    }
                    catch (System.Exception e)
                    {
                        throw new ParserException(
                            "HTMLTagScanner.extractXMLData() : error while trying to find xml tag", e);
                    }
                }
                if (xmlTagFound)
                {
                    if (node != null)
                    {
                        if (node is org.htmlparser.tags.EndTag)
                        {
                            org.htmlparser.tags.EndTag endTag = (org.htmlparser.tags.EndTag) node;
                            if (!endTag.Text.Equals(tagName))
                                xmlTagFound = false;
                        }
                    }
                }
                if (xmlTagFound)
                    return xmlData;
                else
                    return null;
            }
            catch (System.Exception e)
            {
                throw new ParserException(
                    "HTMLTagScanner.extractXMLData() : Error occurred while trying to extract xml tag", e);
            }
        }

        public static bool IsXMLTagFound(Node node, string tagName)
        {
            bool xmlTagFound = false;
            if (node is Tag)
            {
                Tag tag = (Tag) node;
                if (tag.Text.ToUpper().IndexOf(tagName) == 0)
                {
                    xmlTagFound = true;
                }
            }
            return xmlTagFound;
        }

        public Tag CreateScannedNode(Tag tag, string url, NodeReader reader, string currLine)
        {
            Tag thisTag = Scan(tag, url, reader, currLine);
            thisTag.ThisScanner = this;
            thisTag.Attributes = tag.Attributes;
            return thisTag;
        }

        /// <summary> Scan the tag and extract the information related to this type. The url of the
        /// initiating scan has to be provided in case relative links are found. The initial
        /// url is then prepended to it to give an absolute link.
        /// The NodeReader is provided in order to do a lookahead operation. We assume that
        /// the identification has already been performed using the Evaluate() method.
        /// </summary>
        /// <param name="tag">HTML Tag to be scanned for identification
        /// </param>
        /// <param name="url">The initiating url of the scan (Where the html page lies)
        /// </param>
        /// <param name="reader">The reader object responsible for reading the html page
        /// </param>
        public virtual Tag Scan(Tag tag, string url, NodeReader reader, string currLine)
        {
            return CreateTag(new TagData(tag.ElementBegin, tag.ElementEnd, tag.Text, currLine), tag, url);
        }

        public virtual string RemoveChars(string s, string occur)
        {
            System.Text.StringBuilder newString = new System.Text.StringBuilder();
            int index;
            do
            {
                index = s.IndexOf(occur);
                if (index != - 1)
                {
                    newString.Append(s.Substring(0, (index) - (0)));
                    s = s.Substring(index + occur.Length);
                }
            } while (index != - 1);
            newString.Append(s);
            return newString.ToString();
        }

        public static IDictionary AdjustScanners(NodeReader reader)
        {
            IDictionary tempScanners = new Hashtable();
            tempScanners = reader.Parser.Scanners;
            // Remove all existing scanners
            reader.Parser.FlushScanners();
            return tempScanners;
        }

        public static void RestoreScanners(NodeReader pReader, System.Collections.Hashtable tempScanners)
        {
            // Flush the scanners
            pReader.Parser.Scanners = tempScanners;
        }

        /// <summary> Insert an EndTag in the currentLine, just before the occurrence of the provided tag
        /// </summary>
        public virtual string InsertEndTagBeforeNode(AbstractNode node, string currentLine)
        {
            string newLine = currentLine.Substring(0, node.ElementBegin);
            newLine += "</A>";
            newLine += currentLine.Substring(node.ElementBegin, currentLine.Length - node.ElementBegin);
            return newLine;
        }

        /// <summary> Override this method to create your own tag type
        /// </summary>
        /// <param name="">tagData
        /// </param>
        /// <param name="">tag
        /// </param>
        /// <param name="">url
        /// </param>
        /// <returns> Tag
        /// Throws ParserException
        ///
        /// </returns>
        protected virtual Tag CreateTag(TagData tagData, Tag tag, string url)
        {
            return null;
        }

        protected virtual Tag getReplacedEndTag(Tag tag, NodeReader reader, string currentLine)
        {
            // Replace tag - it was a <A> tag - replace with </a>
            string newLine = ReplaceFaultyTagWithEndTag(tag, currentLine);
            reader.ChangeLine(newLine);
            return new EndTag(new TagData(tag.ElementBegin, tag.ElementBegin + 3, tag.TagName, currentLine));
        }

        public virtual string ReplaceFaultyTagWithEndTag(Tag tag, string currentLine)
        {
            string newLine = currentLine.Substring(0, tag.ElementBegin);
            newLine += "</" + tag.TagName + ">";
            newLine += currentLine.Substring(tag.ElementEnd + 1, currentLine.Length - (tag.ElementEnd + 1));

            return newLine;
        }

        protected virtual Tag GetInsertedEndTag(Tag tag, NodeReader reader, string currentLine)
        {
            // Insert end tag
            string newLine = InsertEndTagBeforeNode(tag, currentLine);
            reader.ChangeLine(newLine);
            return new EndTag(new TagData(tag.ElementBegin, tag.ElementBegin + 3, tag.TagName, currentLine));
        }
    }
}
