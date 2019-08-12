// HTMLParser Library v1_3_20030125 - A java-based parser for HTML
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
using BodyScanner = org.htmlparser.scanners.BodyScanner;
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.tags
{
    [TestFixture]
    public class BodyTagTest : ParserTestCase
    {
        private BodyTag bodyTag;

        [SetUp]
        public void SetUp()
        {
            CreateParser("<html><head><title>body tag test</title></head><body>Yahoo!</body></html>");
            parser.RegisterScanners();
            parser.AddScanner(new BodyScanner("-b"));
            ParseAndAssertNodeCount(6);
            Assert.IsTrue(node[4] is BodyTag);
            bodyTag = (BodyTag) node[4];
        }

        [Test]
        public void ToPlainTextString()
        {
            // check the label node
            Assert.AreEqual("Yahoo!", bodyTag.ToPlainTextString(), "Body");
        }

        [Test]
        public void TestToHTML()
        {
            AssertStringEquals("Raw String", "<BODY>Yahoo!</BODY>", bodyTag.ToHtml());
        }

        [Test]
        public void TestToString()
        {
            Assert.AreEqual("BODY: Yahoo!", bodyTag.ToString(), "Body");
        }

        [Test]
        public void Attributes()
        {
            System.Collections.Hashtable attributes;

            CreateParser("<body style=\"margin-top:4px; margin-left:20px;\" title=\"body\">");
            parser.AddScanner(new BodyScanner("-b"));
            foreach (Node theNode in parser)
            {
                if (theNode is BodyTag)
                {
                    attributes = ((BodyTag) theNode).Attributes;
                    Assert.IsTrue(attributes.ContainsKey("STYLE"), "no style attribute");
                    Assert.IsTrue(attributes.ContainsKey("TITLE"), "no title attribute");
                }
                else
                    Assert.Fail("not a body tag");
            }
        }
    }
}
