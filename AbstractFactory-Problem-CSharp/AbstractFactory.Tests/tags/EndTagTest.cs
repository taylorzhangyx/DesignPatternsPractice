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
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.tags
{
    [TestFixture]
    public class EndTagTest : ParserTestCase
    {
        [Test]
        public void ToHTML()
        {
            CreateParser("<HTML></HTML>");
            // Register the image scanner
            parser.RegisterScanners();
            ParseAndAssertNodeCount(2);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[1] is EndTag, "Node should be a HTMLEndTag");
            EndTag endTag = (EndTag) node[1];
            Assert.AreEqual("</HTML>", endTag.ToHtml(), "Raw String");
        }

        [Test]
        public void EndTagFind()
        {
            string testHtml = "<SCRIPT>document.write(d+\".com\")</SCRIPT>";
            int pos = testHtml.IndexOf("</SCRIPT>");
            EndTag endTag = (EndTag) EndTag.Find(testHtml, pos);
            Assert.AreEqual(32, endTag.ElementBegin, "endtag element begin");
            Assert.AreEqual(40, endTag.ElementEnd, "endtag element end");
        }
    }
}
