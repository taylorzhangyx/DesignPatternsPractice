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
using Node = org.htmlparser.Node;
using NodeReader = org.htmlparser.NodeReader;
using Parser = org.htmlparser.Parser;
using Tag = org.htmlparser.tags.Tag;
using ParserTestCase = org.htmlparser.ParserTestCase;
using NodeIterator = org.htmlparser.util.NodeIterator;
using ParserException = org.htmlparser.util.ParserException;
using ParserUtils = org.htmlparser.util.ParserUtils;
using NUnit.Framework;

namespace org.htmlparser.scanners
{
    [TestFixture]
    public class TagScannerTest : ParserTestCase
    {
        private class AnonymousClassTagScanner : TagScanner
        {
            public AnonymousClassTagScanner(TagScannerTest enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            private void InitBlock(TagScannerTest enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            private TagScannerTest enclosingInstance;

            public override string[] ID
            {
                get { return null; }
            }

            public TagScannerTest Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            public override Tag Scan(Tag tag, string url, NodeReader reader, string currLine)
            {
                return null;
            }

            public override bool Evaluate(string s, TagScanner previousOpenScanner)
            {
                return false;
            }
        }

        private class AnonymousClassTagScanner1 : TagScanner
        {
            public AnonymousClassTagScanner1(TagScannerTest enclosingInstance)
            {
                InitBlock(enclosingInstance);
            }

            private void InitBlock(TagScannerTest enclosingInstance)
            {
                this.enclosingInstance = enclosingInstance;
            }

            private TagScannerTest enclosingInstance;

            public override string[] ID
            {
                get { return null; }
            }

            public TagScannerTest Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            public override Tag Scan(Tag tag, string url, NodeReader reader, string currLine)
            {
                return null;
            }

            public override bool Evaluate(string s, TagScanner previousOpenScanner)
            {
                return false;
            }
        }

        [Test]
        public void AbsorbLeadingBlanks()
        {
            string test = "   This is a test";
            string result = TagScanner.AbsorbLeadingBlanks(test);
            Assert.AreEqual("This is a test", result, "Absorb test");
        }

        [Test]
        public void ExtractXMLData()
        {
            CreateParser("<MESSAGE>\n" + "Abhi\n" + "Sri\n" + "</MESSAGE>");
            Parser.LineSeparator = "\r\n";

            foreach (Node node in parser)
            {
                string result = TagScanner.ExtractXMLData(node, "MESSAGE", parser.Reader);
                Assert.AreEqual("Abhi\r\nSri\r\n", result, "Result");
            }
        }

        [Test]
        public void ExtractXMLDataSingle()
        {
            CreateParser("<MESSAGE>Test</MESSAGE>");
            foreach (Node node in parser)
            {
                string result = TagScanner.ExtractXMLData(node, "MESSAGE", parser.Reader);
                Assert.AreEqual("Test", result, "Result");
            }
        }

        [Test]
        public void TagExtraction()
        {
            string testHTML = "<AREA \n coords=0,0,52,52 href=\"http://www.yahoo.com/r/c1\" shape=RECT>";
            CreateParser(testHTML);
            Tag tag = Tag.Find(parser.Reader, testHTML, 0);
            Assert.IsNotNull(tag);
        }

        /// <summary> Captures bug reported by Raghavender Srimantula
        /// Problem is in isXMLTag - when it uses equals() to
        /// find a match
        /// </summary>
        [Test]
        public void IsXMLTag()
        {
            CreateParser("<OPTION value=\"#\">Select a destination</OPTION>");
            NodeIterator iterator = parser.GetEnumerator();
            iterator.MoveNext();
            Node node = (Node) iterator.Current;
            Assert.IsTrue(TagScanner.IsXMLTagFound(node, "OPTION"), "OPTION tag could not be identified");
        }

        [Test]
        public void RemoveChars()
        {
            string test = "hello\nworld\n\tqsdsds";
            TagScanner scanner = new AnonymousClassTagScanner(this);
            string result = ParserUtils.RemoveChars(test, '\n');
            Assert.AreEqual("helloworld\tqsdsds", result, "Removing Chars");
        }

        [Test]
        public void RemoveChars2()
        {
            string test = "hello\r\nworld\r\n\tqsdsds";
            TagScanner scanner = new AnonymousClassTagScanner1(this);
            string result = scanner.RemoveChars(test, "\r\n");
            Assert.AreEqual("helloworld\tqsdsds", result, "Removing Chars");
        }

        /// <summary> Bug report by Cedric Rosa
        /// in absorbLeadingBlanks - crashes if the tag
        /// is empty
        /// </summary>
        [Test]
        public void AbsorbLeadingBlanksBlankTag()
        {
            string testData = "";
            string result = TagScanner.AbsorbLeadingBlanks(testData);
            Assert.AreEqual("", result);
        }
    }
}
