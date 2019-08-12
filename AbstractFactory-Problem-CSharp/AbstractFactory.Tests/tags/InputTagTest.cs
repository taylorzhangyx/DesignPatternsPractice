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

// Author of this class : Dhaval Udani
// dhaval.h.udani@orbitech.co.in
using System;

using NUnit.Framework;
using org.htmlparser;
using org.htmlparser.scanners;
using org.htmlparser.tags;

namespace HtmlParser.tags
{
    [TestFixture]
    public class InputTagTest : ParserTestCase
    {
        private const string testHTML = "<INPUT type=\"text\" name=\"Google\">";

        [SetUp]
        public void SetUp()
        {
            CreateParser(testHTML, "http://www.google.com/test/index.html");
            parser.AddScanner(new InputTagScanner("-i"));
        }

        [Test]
        public void ToHTML()
        {
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is InputTag, "Node 1 should be INPUT Tag");
            InputTag inputTag = (InputTag)node[0];
            Assert.AreEqual("<INPUT TYPE=\"text\" NAME=\"Google\">", inputTag.ToHtml(), "HTML String");
        }

        [Test]
        public void TestToString()
        {
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is InputTag, "Node 1 should be INPUT Tag");
            InputTag inputTag = (InputTag)node[0];
            Assert.AreEqual("INPUT TAG\n--------\nTYPE : text\nNAME : Google\n", inputTag.ToString(), "HTML Raw String");
        }

        /// <summary> Reproduction of bug report 663038
        /// </summary>
        [Test]
        public void ToHTML2()
        {
            string testHTML = "<INPUT type=\"checkbox\" " + "name=\"cbCheck\" checked>";
            CreateParser(testHTML);
            parser.AddScanner(new InputTagScanner("-i"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is InputTag, "Node 1 should be INPUT Tag");
            InputTag inputTag = (InputTag)node[0];

            AssertHelper.FoundExpectedTagAndAttributes(inputTag.ToHtml(),
                "INPUT",
                "TYPE=\"checkbox\"",
                "CHECKED=\"\"",
                "NAME=\"cbCheck\"");
        }
    }
}
