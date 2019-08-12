// ***************************************************************************
// Copyright (c) 2018, Industrial Logic, Inc., All Rights Reserved.
//
// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
// used by students during Industrial Logic's workshops or by individuals
// who are being coached by Industrial Logic on a project.
//
// This code may NOT be copied or used for any other purpose without the prior
// written consent of Industrial Logic, Inc.
// ****************************************************************************

using System;
using System.Text;
using FormTag = org.htmlparser.tags.FormTag;
using InputTag = org.htmlparser.tags.InputTag;
using Tag = org.htmlparser.tags.Tag;
using org.htmlparser.util;
using DefaultParserFeedback = org.htmlparser.util.DefaultParserFeedback;
using NodeIterator = org.htmlparser.util.NodeIterator;
using ParserException = org.htmlparser.util.ParserException;
using ParserUtils = org.htmlparser.util.ParserUtils;
using NUnit.Framework;

namespace org.htmlparser
{
    public class ParserTestCase
    {
        protected Parser parser;
        protected Node[] node;
        protected int nodeCount;
        protected NodeReader reader;

        protected void Parse(string response)
        {
            CreateParser(response, 10000);
            parser.RegisterScanners();
            ParseNodes();
        }

        protected void CreateParser(string inputHTML)
        {
            string testHTML = new string(inputHTML.ToCharArray());
            System.IO.StringReader sr = new System.IO.StringReader(testHTML);
            reader = new NodeReader(sr, 5000);
            parser = new Parser(reader, new NoFeedback());
            node = new AbstractNode[40];
        }

        protected void CreateParser(string inputHTML, int numNodes)
        {
            string testHTML = new string(inputHTML.ToCharArray());
            System.IO.StringReader sr = new System.IO.StringReader(testHTML);
            reader = new NodeReader(sr, 5000);
            parser = new Parser(reader, new DefaultParserFeedback());
            node = new AbstractNode[numNodes];
        }

        protected void CreateParser(string inputHTML, string url)
        {
            string testHTML = new string(inputHTML.ToCharArray());
            System.IO.StringReader sr = new System.IO.StringReader(testHTML);
            reader = new NodeReader(sr, url);
            parser = new Parser(reader, new NoFeedback());
            node = new AbstractNode[40];
        }

        protected void CreateParser(string inputHTML, string url, int numNodes)
        {
            string testHTML = new string(inputHTML.ToCharArray());
            System.IO.StringReader sr = new System.IO.StringReader(testHTML);
            reader = new NodeReader(sr, url);
            parser = new Parser(reader, new DefaultParserFeedback());
            node = new AbstractNode[numNodes];
        }

        public virtual void AssertStringEquals(string message, string expected, string actual)
        {
            string mismatchInfo = "";

            if (expected.Length < actual.Length)
            {
                mismatchInfo = "\n\nACTUAL result has " + (actual.Length - expected.Length) +
                               " extra characters at the end. They are :";

                for (int i = expected.Length; i < actual.Length; i++)
                {
                    mismatchInfo += ("\nPosition : " + i + " , Code = " + (int) actual[i]);
                }
            }
            else if (expected.Length > actual.Length)
            {
                mismatchInfo = "\n\nEXPECTED result has " + (expected.Length - actual.Length) +
                               " extra characters at the end. They are :";

                for (int i = actual.Length; i < expected.Length; i++)
                {
                    mismatchInfo += ("\nPosition : " + i + " , Code = " + (int) expected[i]);
                }
            }
            for (int i = 0; i < expected.Length; i++)
            {
                if ((expected.Length != actual.Length && (i >= (expected.Length - 1) || i >= (actual.Length - 1))) ||
                    (actual[i] != expected[i]))
                {
                    StringBuilder errorMsg = new StringBuilder();
                    errorMsg.Append(message + mismatchInfo + " \nMismatch of strings at char posn " + i +
                                    " \n\nString Expected upto mismatch = " + expected.Substring(0, (i) - (0)) +
                                    " \n\nString Actual upto mismatch = " + actual.Substring(0, (i) - (0)));
                    if (i < expected.Length)
                        errorMsg.Append(" \n\nString Expected MISMATCH CHARACTER = " + expected[i] + ", code = " +
                                        (int) expected[i]);

                    if (i < actual.Length)
                        errorMsg.Append(" \n\nString Actual MISMATCH CHARACTER = " + actual[i] + ", code = " +
                                        (int) actual[i]);

                    errorMsg.Append(" \n\n**** COMPLETE STRING EXPECTED ****\n" + expected +
                                    " \n\n**** COMPLETE STRING ACTUAL***\n" + actual);
                    Assert.Fail(errorMsg.ToString());
                }
            }
        }

        public void ParseNodes()
        {
            nodeCount = 0;
            foreach (Node aNode in parser)
            {
                node[nodeCount++] = aNode;
            }
        }

        public void ParseAndAssertNodeCount(int nodeCountExpected)
        {
            ParseNodes();
            AssertNodeCount(nodeCountExpected);
        }

        public void AssertSameType(string displayMessage, Node expected, Node actual)
        {
            string expectedNodeName = expected.GetType().FullName;
            string actualNodeName = actual.GetType().FullName;
            displayMessage = "The types did not match: Expected " + expectedNodeName + " \nbut was " + actualNodeName +
                             "\nEXPECTED XML:" + expected.ToHtml() + "\n" + "ACTUAL XML:" + actual.ToHtml() +
                             displayMessage;
            AssertStringEquals(displayMessage, expectedNodeName, actualNodeName);
        }

        public void AssertTagEquals(Node expected, Node actual, string displayMessage)
        {
            if (expected is Tag)
            {
                Tag expectedTag = (Tag) expected;
                Tag actualTag = (Tag) actual;
                AssertTagNameMatches(expectedTag, actualTag, displayMessage);
                AssertAttributesMatch(expectedTag, actualTag, displayMessage);
            }
        }

        private void AssertTagNameMatches(Tag nextExpectedTag, Tag nextActualTag, string displayMessage)
        {
            string expectedTagName = nextExpectedTag.TagName;
            string actualTagName = nextActualTag.TagName;
            displayMessage = "The tag names did not match: Expected " + expectedTagName + " \nbut was " + actualTagName +
                             displayMessage;
            AssertStringEquals(displayMessage, expectedTagName, actualTagName);
        }

        public void AssertXmlEquals(string expected, string actual, string displayMessage)
        {
            expected = RemoveEscapeCharacters(expected);
            actual = RemoveEscapeCharacters(actual);

            Parser expectedParser = Parser.CreateParser(expected);
            Parser resultParser = Parser.CreateParser(actual);

            NodeIterator expectedIterator = expectedParser.GetEnumerator();
            NodeIterator actualIterator = resultParser.GetEnumerator();
            displayMessage = CreateGenericFailureMessage(displayMessage, expected, actual);

            Node currentExpectedNode = null, currentActualNode = null;
            while (expectedIterator.MoveNext())
            {
                Assert.IsTrue(actualIterator.MoveNext());

                currentExpectedNode = GetCurrentNodeUsing(expectedIterator);
                currentActualNode = GetCurrentNodeUsing(actualIterator);

                AssertStringValueMatches(currentExpectedNode, currentActualNode, displayMessage);
                FixIfXmlEndTag(resultParser, currentActualNode);
                FixIfXmlEndTag(expectedParser, currentExpectedNode);
                AssertSameType(displayMessage, currentExpectedNode, currentActualNode);
                AssertTagEquals(currentExpectedNode, currentActualNode, displayMessage);
            }
            AssertActualXmlHasNoMoreNodes(actualIterator, displayMessage);
        }

        private Node GetCurrentNodeUsing(NodeIterator nodeIterator)
        {
            Node currentNode;
            string text = null;
            do
            {
                if (text != null)
                    nodeIterator.MoveNext();
                currentNode = (Node) nodeIterator.Current;
                if (currentNode is StringNode)
                    text = currentNode.ToPlainTextString().Trim();
                else
                    text = null;
            } while (text != null && text.Length == 0);
            return currentNode;
        }

        private void AssertStringValueMatches(Node expectedNode, Node actualNode, string displayMessage)
        {
            string expected = expectedNode.ToPlainTextString().Trim();
            string actual = actualNode.ToPlainTextString().Trim();
            expected = expected.Replace('\n', ' ');
            actual = actual.Replace('\n', ' ');
            displayMessage = "String value mismatch\nEXPECTED:" + expected + "\nACTUAL:" + actual + displayMessage;
            AssertStringEquals(displayMessage, expected, actual);
        }

        private void AssertActualXmlHasNoMoreNodes(NodeIterator actualIterator, string displayMessage)
        {
            if (actualIterator.MoveNext())
            {
                string extraTags = "\nExtra Tags\n**********\n";
                do
                {
                    extraTags += ((Node) actualIterator.Current).ToHtml();
                } while (actualIterator.MoveNext());

                displayMessage = "Actual had more data than expected\n" + extraTags + displayMessage;
                Assert.Fail(displayMessage);
            }
        }

        private string CreateGenericFailureMessage(string displayMessage, string expected, string actual)
        {
            return "\n\n" + displayMessage + "\n\nComplete Xml\n************\nEXPECTED:\n" + expected + "\nACTUAL:\n" +
                   actual;
        }

        private void FixIfXmlEndTag(Parser parser, Node node)
        {
            if (node is Tag)
            {
                Tag tag = (Tag) node;
                if (tag.EmptyXmlTag)
                {
                    // Add end tag
                    string currLine = parser.Reader.CurrentLine;
                    int pos = parser.Reader.LastReadPosition;
                    currLine = currLine.Substring(0, (pos + 1) - (0)) + "</" + tag.TagName + ">" +
                               currLine.Substring(pos + 1, (currLine.Length) - (pos + 1));
                    parser.Reader.ChangeLine(currLine);
                }
            }
        }

        private void AssertAttributesMatch(Tag expectedTag, Tag actualTag, string displayMessage)
        {
            AssertAllExpectedTagAttributesFoundInActualTag(expectedTag, actualTag, displayMessage);
            if (expectedTag.Attributes.Count != actualTag.Attributes.Count)
            {
                AssertActualTagHasNoExtraAttributes(expectedTag, actualTag, displayMessage);
            }
        }

        private void AssertActualTagHasNoExtraAttributes(Tag expectedTag, Tag actualTag, string displayMessage)
        {
            foreach (string key in actualTag.Attributes.Keys)
            {
                if (key.Equals("/"))
                    continue;
                string expectedValue = expectedTag[key];
                string actualValue = actualTag[key];
                if (key == Tag.TAGNAME)
                {
                    expectedValue = ParserUtils.RemoveChars(expectedValue, '/');
                    actualValue = ParserUtils.RemoveChars(actualValue, '/');
                    AssertStringEquals(displayMessage + "]ntag name", actualValue, expectedValue);
                    continue;
                }

                if (expectedValue == null)
                    Assert.Fail("\nActual tag had extra key: " + key + displayMessage);
            }
        }

        private void AssertAllExpectedTagAttributesFoundInActualTag(Tag expectedTag, Tag actualTag,
                                                                    string displayMessage)
        {
            foreach (string key in expectedTag.Attributes.Keys)
            {
                if (key.Equals("/"))
                    continue;
                string expectedValue = expectedTag[key];

                string actualValue = actualTag[key];
                if (key == Tag.TAGNAME)
                {
                    expectedValue = ParserUtils.RemoveChars(expectedValue, '/');
                    actualValue = ParserUtils.RemoveChars(actualValue, '/');
                    AssertStringEquals(displayMessage + "]ntag name", expectedValue, actualValue);
                    continue;
                }

                AssertStringEquals(
                    "\nvalue for key " + key + " in tag " + expectedTag.TagName + " expected=" + expectedValue +
                    " but was " + actualValue + "\n\nComplete Tag expected:\n" + expectedTag.ToHtml() +
                    "\n\nComplete Tag actual:\n" + actualTag.ToHtml() + displayMessage, expectedValue, actualValue);
            }
        }

        public static string RemoveEscapeCharacters(string inputString)
        {
            inputString = ParserUtils.RemoveChars(inputString, '\r');
            inputString = inputString.Replace('\n', ' ');
            inputString = ParserUtils.RemoveChars(inputString, '\t');
            return inputString;
        }

        public void AssertType(string message, Type expectedType, Node actual)
        {
            string expectedTypeName = expectedType.FullName;
            string actualTypeName = actual.GetType().FullName;
            if (!actualTypeName.Equals(expectedTypeName))
            {
                Assert.Fail(message + " should have been of type\n" + expectedTypeName + " but was of type \n" +
                            actualTypeName + "\n and is :" + actual.ToHtml());
            }
        }

        public void AssertNodeCount(int nodeCountExpected)
        {
            StringBuilder msg = new StringBuilder();
            for (int i = 0; i < nodeCount; i++)
            {
                msg.Append(node[i].GetType().Name);
                msg.Append("-->\n").Append(node[i].ToHtml()).Append("\n");
            }
            Assert.AreEqual(nodeCountExpected, nodeCount,
                            "Number of nodes parsed didn't match, nodes found were :\n" + msg.ToString());
        }

        protected void AssertNodeCount(string message, int expectedLength, Node[] nodes)
        {
            if (expectedLength != nodes.Length)
            {
                StringBuilder failMsg = new StringBuilder(message);
                failMsg.Append("\n");
                failMsg.Append("Number of nodes expected ").Append(expectedLength).Append(" \n");
                failMsg.Append("but was : ");
                failMsg.Append(nodes.Length).Append("\n");
                failMsg.Append("Nodes found are:\n");
                for (int i = 0; i < nodes.Length; i++)
                {
                    failMsg.Append("Node ").Append(i).Append(" : ");
                    failMsg.Append(nodes[i].GetType().Name).Append("\n");
                    failMsg.Append(nodes[i].ToString()).Append("\n\n");
                }
                Assert.Fail(failMsg.ToString());
            }
        }
    }
}
