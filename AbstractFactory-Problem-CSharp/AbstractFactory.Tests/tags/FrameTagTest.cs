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

using NUnit.Framework;
using org.htmlparser;
using org.htmlparser.scanners;
using org.htmlparser.tags;

namespace HtmlParser.tags
{
    [TestFixture]
    public class FrameTagTest : ParserTestCase
    {
        [Test]
        public void ToHTML()
        {
            CreateParser("<frameset rows=\"115,*\" frameborder=\"NO\" border=\"0\" framespacing=\"0\">\n" +
                         "<frame name=\"topFrame\" noresize src=\"demo_bc_top.html\" scrolling=\"NO\" frameborder=\"NO\">\n" +
                         "<frame name=\"mainFrame\" src=\"http://www.kizna.com/web_e/\" scrolling=\"AUTO\">\n" +
                         "</frameset>");
            parser.AddScanner(new FrameScanner(""));

            ParseAndAssertNodeCount(4);
            Assert.IsTrue(node[1] is FrameTag, "Node 1 should be Frame Tag");
            Assert.IsTrue(node[2] is FrameTag, "Node 2 should be Frame Tag");

            FrameTag frameTag1 = (FrameTag)node[1];
            FrameTag frameTag2 = (FrameTag)node[2];

            AssertHelper.FoundExpectedTagAndAttributes(frameTag1.ToHtml(),
                "FRAME",
                "NORESIZE=\"\"",
                "FRAMEBORDER=\"NO\"",
                "SRC=\"demo_bc_top.html\"",
                "NAME=\"topFrame\"",
                "SCROLLING=\"NO\"");

            AssertHelper.FoundExpectedTagAndAttributes(frameTag2.ToHtml(),
                "FRAME",
                "SRC=\"http://www.kizna.com/web_e/\"",
                "NAME=\"mainFrame\"",
                "SCROLLING=\"AUTO\"");
        }
    }
}
