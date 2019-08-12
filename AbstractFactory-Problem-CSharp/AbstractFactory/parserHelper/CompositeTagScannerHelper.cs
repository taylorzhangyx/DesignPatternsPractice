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
using Node = org.htmlparser.Node;
using NodeReader = org.htmlparser.NodeReader;
using CompositeTagScanner = org.htmlparser.scanners.CompositeTagScanner;
using CompositeTag = org.htmlparser.tags.CompositeTag;
using EndTag = org.htmlparser.tags.EndTag;
using Tag = org.htmlparser.tags.Tag;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using NodeList = org.htmlparser.util.NodeList;
using ParserException = org.htmlparser.util.ParserException;

namespace org.htmlparser.parserHelper
{
    public class CompositeTagScannerHelper
    {
        private CompositeTagScanner scanner;
        private Tag tag;
        private string url;
        private NodeReader reader;
        private string currLine;
        private Tag endTag;
        private NodeList nodeList;
        private bool endTagFound;
        private int startingLineNumber;
        private int endingLineNumber;
        private bool balance_quotes;

        public CompositeTagScannerHelper(CompositeTagScanner scanner, Tag tag, string url, NodeReader reader,
                                         string currLine, bool balance_quotes)
        {
            this.scanner = scanner;
            this.tag = tag;
            this.url = url;
            this.reader = reader;
            this.currLine = currLine;
            this.endTag = null;
            this.nodeList = new NodeList();
            this.endTagFound = false;
            this.balance_quotes = balance_quotes;
        }

        public virtual Tag Scan()
        {
            this.startingLineNumber = reader.LastLineNumber;
            if (ShouldCreateEndTagAndExit())
            {
                return CreateEndTagAndRepositionReader();
            }
            scanner.BeforeScanningStarts();
            Node currentNode = tag;

            DoEmptyXmlTagCheckOn(currentNode);
            if (!endTagFound)
            {
                do
                {
                    currentNode = reader.ReadElement(balance_quotes);
                    if (currentNode == null)
                        continue;
                    currLine = reader.CurrentLine;
                    if (currentNode is Tag)
                        DoForceCorrectionCheckOn((Tag) currentNode);

                    DoEmptyXmlTagCheckOn(currentNode);
                    if (!endTagFound)
                        DoChildAndEndTagCheckOn(currentNode);
                } while (currentNode != null && !endTagFound);
            }
            if (endTag == null)
            {
                CreateCorrectionEndTagBefore(reader.LastReadPosition + 1);
            }

            this.endingLineNumber = reader.LastLineNumber;
            return CreateTag();
        }

        private bool ShouldCreateEndTagAndExit()
        {
            return scanner.ShouldCreateEndTagAndExit();
        }

        private Tag CreateEndTagAndRepositionReader()
        {
            CreateCorrectionEndTagBefore(tag.ElementBegin);
            reader.PosInLine = tag.ElementBegin;
            reader.DontReadNextLine = true;
            return endTag;
        }

        private void CreateCorrectionEndTagBefore(int pos)
        {
            string endTagName = tag.TagName;
            int endTagBegin = pos;
            int endTagEnd = endTagBegin + endTagName.Length + 2;
            endTag = new EndTag(new TagData(endTagBegin, endTagEnd, endTagName, currLine));
        }

        private void CreateCorrectionEndTagBefore(Tag possibleEndTagCauser)
        {
            string endTagName = tag.TagName;
            int endTagBegin = possibleEndTagCauser.ElementBegin;
            int endTagEnd = endTagBegin + endTagName.Length + 2;
            possibleEndTagCauser.TagBegin = endTagEnd + 1;
            reader.AddNextParsedNode(possibleEndTagCauser);
            endTag = new EndTag(new TagData(endTagBegin, endTagEnd, endTagName, currLine));
        }

        private StringBuilder CreateModifiedLine(string endTagName, int endTagBegin)
        {
            StringBuilder newLine = new StringBuilder();
            newLine.Append(currLine.Substring(0, endTagBegin - 0));
            newLine.Append("</");
            newLine.Append(endTagName);
            newLine.Append(">");
            newLine.Append(currLine.Substring(endTagBegin, currLine.Length - endTagBegin));
            return newLine;
        }

        private Tag CreateTag()
        {
            CompositeTag newTag =
                (CompositeTag)
                scanner.CreateTag(
                    new TagData(tag.ElementBegin, endTag.ElementEnd, startingLineNumber, endingLineNumber, tag.Text,
                                currLine, url, tag.EmptyXmlTag), new CompositeTagData(tag, endTag, nodeList));
            for (int i = 0; i < newTag.ChildCount; i++)
            {
                Node child = newTag[i];
                child.Parent = newTag;
            }
            return newTag;
        }

        private void DoChildAndEndTagCheckOn(Node currentNode)
        {
            if (currentNode is EndTag)
            {
                EndTag possibleEndTag = (EndTag) currentNode;
                if (IsExpectedEndTag(possibleEndTag))
                {
                    endTagFound = true;
                    endTag = possibleEndTag;
                    return;
                }
            }
            nodeList.Add(currentNode);
            scanner.ChildNodeEncountered(currentNode);
        }

        private bool IsExpectedEndTag(EndTag possibleEndTag)
        {
            return possibleEndTag.TagName.Equals(tag.TagName);
        }

        private void DoEmptyXmlTagCheckOn(Node currentNode)
        {
            if (currentNode is Tag)
            {
                Tag possibleEndTag = (Tag) currentNode;
                if (IsXmlEndTag(tag))
                {
                    endTag = possibleEndTag;
                    endTagFound = true;
                }
            }
        }

        private void DoForceCorrectionCheckOn(Tag possibleEndTagCauser)
        {
            if (IsEndTagMissing(possibleEndTagCauser))
            {
                CreateCorrectionEndTagBefore(possibleEndTagCauser);

                endTagFound = true;
            }
        }

        private bool IsEndTagMissing(Tag possibleEndTag)
        {
            return scanner.IsTagToBeEndedFor(possibleEndTag) || IsSelfChildTagRecievedIncorrectly(possibleEndTag);
        }

        private bool IsSelfChildTagRecievedIncorrectly(Tag possibleEndTag)
        {
            return (!(possibleEndTag is EndTag) && !scanner.AllowSelfChildren &&
                    possibleEndTag.TagName.Equals(tag.TagName));
        }

        public virtual bool IsXmlEndTag(Tag tag)
        {
            string tagText = tag.Text;
            int lastSlash = tagText.LastIndexOf("/");
            return (lastSlash == tagText.Length - 1 || tag.EmptyXmlTag) && tag.Text.IndexOf("://") == - 1;
        }
    }
}
