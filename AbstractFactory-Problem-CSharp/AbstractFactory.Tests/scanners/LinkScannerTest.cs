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
using AbstractNode = org.htmlparser.AbstractNode;
using Node = org.htmlparser.Node;
using Parser = org.htmlparser.Parser;
using StringNode = org.htmlparser.StringNode;
using EndTag = org.htmlparser.tags.EndTag;
using ImageTag = org.htmlparser.tags.ImageTag;
using LinkTag = org.htmlparser.tags.LinkTag;
using Tag = org.htmlparser.tags.Tag;
using TagData = org.htmlparser.tags.data.TagData;
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.scanners
{
    [TestFixture]
    public class LinkScannerTest : ParserTestCase
    {
        [Test]
        public void AccessKey()
        {
            CreateParser(
                "<a href=\"http://www.kizna.com/servlets/SomeServlet?name=Sam Joseph\" accessKey=1>Click Here</A>");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "The node should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.kizna.com/servlets/SomeServlet?name=Sam Joseph", linkTag.Link,
                            "Link URL of link tag");
            Assert.AreEqual("Click Here", linkTag.LinkText, "Link Text of link tag");
            Assert.AreEqual("1", linkTag.AccessKey, "Access key");
        }

        [Test]
        public void ErroneousLinkBug()
        {
            CreateParser("<p>Site Comments?<br>" + "<a href=\"mailto:sam@neurogrid.com?subject=Site Comments\">" +
                         "Mail Us" + "<a>" + "</p>");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(6);
            // The first node should be a Tag
            Assert.IsTrue(node[0] is Tag, "First node should be a Tag");
            // The second node should be a HTMLStringNode
            Assert.IsTrue(node[1] is StringNode, "Second node should be a HTMLStringNode");
            StringNode stringNode = (StringNode) node[1];
            Assert.AreEqual("Site Comments?", stringNode.Text, "Text of the StringNode");
            Assert.IsTrue(node[2] is Tag, "Third node should be a tag");
        }

        /// <summary> Test case based on a report by Raghavender Srimantula, of the parser giving out of memory exceptions. Found to occur
        /// on the following piece of html
        /// <pre>
        /// <a href=s/8741><img src="http://us.i1.yimg.com/us.yimg.com/i/i16/mov_popc.gif" height=16 width=16 border=0></img></td><td nowrap> &nbsp;
        /// <a href=s/7509>
        /// </pre>
        /// </summary>
        [Test]
        public void ErroneousLinkBugFromYahoo2()
        {
            CreateParser(
                "<td>" + "<a href=s/8741>" +
                "<img src=\"http://us.i1.yimg.com/us.yimg.com/i/i16/mov_popc.gif\" height=16 width=16 border=0>" +
                "</td>" + "<td nowrap> &nbsp;\n" + "<a href=s/7509><b>Yahoo! Movies</b></a>" + "</td>",
                "http://www.yahoo.com");
            parser.RegisterScanners();
            Node[] linkNodes = parser.ExtractAllNodesThatAre(typeof (LinkTag));

            Assert.AreEqual(2, linkNodes.Length, "number of links");
            LinkTag linkTag = (LinkTag) linkNodes[0];
            AssertStringEquals("Link", "http://www.yahoo.com/s/8741", linkTag.Link);
            // Verify the link data
            AssertStringEquals("Link Text", "", linkTag.LinkText);
        }

        /// <summary> Test case based on a report by Raghavender Srimantula, of the parser giving out of memory exceptions. Found to occur
        /// on the following piece of html
        /// <pre>
        /// <a href=s/8741><img src="http://us.i1.yimg.com/us.yimg.com/i/i16/mov_popc.gif" height=16 width=16 border=0></img>This is test
        /// <a href=s/7509>
        /// </pre>
        /// </summary>
        [Test]
        public void ErroneousLinkBugFromYahoo()
        {
            CreateParser(
                "<a href=s/8741>" + "<img src=\"http://us.i1.yimg.com/us.yimg.com/i/i16/mov_popc.gif\" " + "height=16 " +
                "width=16 " + "border=0>" + "This is a test\n" + "<a href=s/7509>" + "<b>Yahoo! Movies</b>" + "</a>",
                "http://www.yahoo.com");

            parser.RegisterScanners();
            ParseAndAssertNodeCount(2);
            // The first node should be a Tag
            Assert.IsTrue(node[0] is LinkTag, "First node should be a HTMLLinkTag");
            // The second node should be a HTMLStringNode
            Assert.IsTrue(node[1] is LinkTag, "Second node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.yahoo.com/s/8741", linkTag.Link, "Link");
            // Verify the link data
            Assert.AreEqual("This is a test\r\n", linkTag.LinkText, "Link Text");
        }

        [Test]
        public void Evaluate()
        {
            LinkScanner scanner = new LinkScanner("-l");
            bool retVal = scanner.Evaluate("   a href ", null);
            Assert.IsTrue(retVal, "Evaluation of the Link tag");
        }

        /// <summary> This is the reproduction of a bug which causes a null pointer exception
        /// </summary>
        [Test]
        public void ExtractLinkInvertedCommasBug()
        {
            string tagContents = "a href=r/anorth/top.html";
            Tag tag = new Tag(new TagData(0, 0, tagContents, ""));
            string url = "c:\\cvs\\html\\binaries\\yahoo.htm";
            LinkScanner scanner = new LinkScanner("-l");
            Assert.AreEqual("r/anorth/top.html", scanner.ExtractLink(tag, url), "Extracted Link");
        }

        /// <summary> This is the reproduction of a bug which produces multiple text copies.
        /// </summary>
        [Test]
        public void ExtractLinkInvertedCommasBug2()
        {
            CreateParser(
                "<a href=\"http://cbc.ca/artsCanada/stories/greatnorth271202\" class=\"lgblacku\">Vancouver schools plan 'Great Northern Way'</a>");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "The node should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            AssertStringEquals("Extracted Text", "Vancouver schools plan 'Great Northern Way'", linkTag.LinkText);
        }

        /// <summary> Bug pointed out by Sam Joseph (sam@neurogrid.net)
        /// Links with spaces in them will get their spaces absorbed
        /// </summary>
        [Test]
        public void LinkSpacesBug()
        {
            CreateParser("<a href=\"http://www.kizna.com/servlets/SomeServlet?name=Sam Joseph\">Click Here</A>");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "The node should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.kizna.com/servlets/SomeServlet?name=Sam Joseph", linkTag.Link,
                            "Link URL of link tag");
            Assert.AreEqual("Click Here", linkTag.LinkText, "Link Text of link tag");
        }

        /// <summary> Bug reported by Raj Sharma,5-Apr-2002, upon parsing
        /// http://www.samachar.com, the entire page could not be picked up.
        /// The problem was occurring after parsing a particular link
        /// after which the parsing would not proceed. This link was spread over three lines.
        /// The bug has been reproduced and fixed.
        /// </summary>
        [Test]
        public void MultipleLineBug()
        {
            CreateParser("<LI><font color=\"FF0000\" size=-1><b>Tech Samachar:</b></font><a \n" +
                         "href=\"http://ads.samachar.com/bin/redirect/tech.txt?http://www.samachar.com/tech\n" +
                         "nical.html\"> Journalism 3.0</a> by Rajesh Jain");
            Parser.LineSeparator = "\r\n";
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(8);
            Assert.IsTrue(node[6] is LinkTag, "Seventh node should be a link tag");
            LinkTag linkTag = (LinkTag) node[6];
            string exp = "http://ads.samachar.com/bin/redirect/tech.txt?http://www.samachar.com/technical.html";
            AssertStringEquals("Link URL of link tag", exp, linkTag.Link);
            Assert.AreEqual(" Journalism 3.0", linkTag.LinkText, "Link Text of link tag");
            Assert.IsTrue(node[7] is StringNode, "Eight node should be a string node");
            StringNode stringNode = (StringNode) node[7];
            Assert.AreEqual(" by Rajesh Jain", stringNode.Text, "String node contents");
        }

        [Test]
        public void RelativeLinkScan()
        {
            CreateParser("<A HREF=\"mytest.html\"> Hello World</A>", "http://www.yahoo.com");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node identified should be HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.yahoo.com/mytest.html", linkTag.Link, "Expected Link");
        }

        [Test]
        public void RelativeLinkScan2()
        {
            CreateParser("<A HREF=\"abc/def/mytest.html\"> Hello World</A>", "http://www.yahoo.com");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node identified should be HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            AssertStringEquals("Expected Link", "http://www.yahoo.com/abc/def/mytest.html", linkTag.Link);
        }

        [Test]
        public void RelativeLinkScan3()
        {
            CreateParser("<A HREF=\"../abc/def/mytest.html\"> Hello World</A>", "http://www.yahoo.com/ghi");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node identified should be HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            AssertStringEquals("Expected Link", "http://www.yahoo.com/abc/def/mytest.html", linkTag.Link);
        }

        /// <summary> Test scan with data which is of diff nodes type
        /// </summary>
        [Test]
        public void Scan()
        {
            CreateParser("<A HREF=\"mytest.html\"><IMG SRC=\"abcd.jpg\">Hello World</A>", "http://www.yahoo.com");
            // Register the image scanner
            LinkScanner linkScanner = new LinkScanner("-l");
            parser.AddScanner(linkScanner);
            parser.AddScanner(linkScanner.CreateImageScanner("-i"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a link node");

            LinkTag linkTag = (LinkTag) node[0];
            // Get the link data and cross-check
            Node[] dataNode = new AbstractNode[10];
            int i = 0;
            foreach (Node nestedNode in linkTag)
            {
                dataNode[i++] = nestedNode;
            }
            Assert.AreEqual(2, i, "Number of data nodes");
            Assert.IsTrue(dataNode[0] is ImageTag, "First data node should be an Image Node");
            Assert.IsTrue(dataNode[1] is StringNode, "Second data node shouls be a String Node");

            // Check the contents of each data node
            ImageTag imageTag = (ImageTag) dataNode[0];
            Assert.AreEqual("http://www.yahoo.com/abcd.jpg", imageTag.ImageURL, "Image URL");
            StringNode stringNode = (StringNode) dataNode[1];
            Assert.AreEqual("Hello World", stringNode.Text, "String Contents");
        }

        [Test]
        public void ReplaceFaultyTagWithEndTag()
        {
            string currentLine =
                "<p>Site Comments?<br><a href=\"mailto:sam@neurogrid.com?subject=Site Comments\">Mail Us<a></p>";
            Tag tag = new Tag(new TagData(85, 87, "a", currentLine));
            LinkScanner linkScanner = new LinkScanner();
            string newLine = linkScanner.ReplaceFaultyTagWithEndTag(tag, currentLine);
            Assert.AreEqual(
                "<p>Site Comments?<br><a href=\"mailto:sam@neurogrid.com?subject=Site Comments\">Mail Us</A></p>",
                newLine, "Expected replacement");
        }

        [Test]
        public void InsertEndTagBeforeTag()
        {
            string currentLine = "<a href=s/7509><b>Yahoo! Movies</b></a>";
            Tag tag = new Tag(new TagData(0, 14, "a href=s/7509", currentLine));
            LinkScanner linkScanner = new LinkScanner();
            string newLine = linkScanner.InsertEndTagBeforeNode(tag, currentLine);
            Assert.AreEqual("</A><a href=s/7509><b>Yahoo! Movies</b></a>", newLine, "Expected insertion");
        }

        /// <summary> A bug in the freshmeat page - really bad html
        /// tag - &lt;A&gt;Revision&lt;\a&gt;
        /// Reported by Mazlan Mat
        /// </summary>
        [Test]
        public void FreshMeatBug()
        {
            CreateParser("<a>Revision</a>", "http://www.yahoo.com");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(3);
            Assert.IsTrue(node[0] is Tag, "Node 0 should be a tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("a", tag.Text, "Tag Contents");
            Assert.IsTrue(node[1] is StringNode, "Node 1 should be a string node");
            StringNode stringNode = (StringNode) node[1];
            Assert.AreEqual("Revision", stringNode.Text, "StringNode Contents");
            Assert.IsTrue(node[2] is EndTag, "Node 2 should be a string node");
            EndTag endTag = (EndTag) node[2];
            Assert.AreEqual("a", endTag.Text, "End Tag Contents");
        }

        /// <summary> Test suggested by Cedric Rosa
        /// A really bad link tag sends parser into infinite loop
        /// </summary>
        [Test]
        public void BrokenLink()
        {
            CreateParser(
                "<a href=\"faq.html\">" + "<br>\n" + "<img src=\"images/46revues.gif\" " + "width=\"100\" " +
                "height=\"46\" " + "border=\"0\" " + "alt=\"Rejoignez revues.org!\" " + "align=\"middle\">",
                "http://www.yahoo.com");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node 0 should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.IsNotNull(linkTag.ToString());
        }

        [Test]
        public void LinkDataContents()
        {
            CreateParser(
                "<a href=\"http://transfer.go.com/cgi/atransfer.pl?goto=http://www.signs.movies.com&name=114332&srvc=nws&context=283&guid=4AD5723D-C802-4310-A388-0B24E1A79689\" target=\"_new\"><img src=\"http://ad.abcnews.com/ad/sponsors/buena_vista_pictures/bvpi-ban0003.gif\" width=468 height=60 border=\"0\" alt=\"See Signs in Theaters 8-2 - Starring Mel Gibson\" align=><font face=\"verdana,arial,helvetica\" SIZE=\"1\"><b></b></font></a>",
                "http://transfer.go.com");
            // Register the image scanner
            LinkScanner linkScanner = new LinkScanner("-l");
            parser.AddScanner(linkScanner);
            parser.AddScanner(linkScanner.CreateImageScanner("-i"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node 0 should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual(
                "http://transfer.go.com/cgi/atransfer.pl?goto=http://www.signs.movies.com&name=114332&srvc=nws&context=283&guid=4AD5723D-C802-4310-A388-0B24E1A79689",
                linkTag.Link, "Link URL");
            Assert.AreEqual("", linkTag.LinkText, "Link Text");
            Node[] containedNodes = new AbstractNode[10];
            int i = 0;
            foreach (Node nestedNode in linkTag)
            {
                containedNodes[i++] = nestedNode;
            }
            Assert.AreEqual(5, i, "There should be 5 contained nodes in the link tag");
            Assert.IsTrue(containedNodes[0] is ImageTag, "First contained node should be an image tag");
            ImageTag imageTag = (ImageTag) containedNodes[0];
            Assert.AreEqual("http://ad.abcnews.com/ad/sponsors/buena_vista_pictures/bvpi-ban0003.gif", imageTag.ImageURL,
                            "Image Location");
            Assert.AreEqual("60", imageTag["HEIGHT"], "Image Height");
            Assert.AreEqual("468", imageTag["WIDTH"], "Image Width");
            Assert.AreEqual("0", imageTag["BORDER"], "Image Border");
            Assert.AreEqual("See Signs in Theaters 8-2 - Starring Mel Gibson", imageTag["ALT"], "Image Alt");
            Assert.IsTrue(containedNodes[1] is Tag, "Second contained node should be Tag");
            Tag tag1 = (Tag) containedNodes[1];
            Assert.AreEqual("font face=\"verdana,arial,helvetica\" SIZE=\"1\"", tag1.Text, "Tag Contents");
            Assert.IsTrue(containedNodes[2] is Tag, "Third contained node should be Tag");
            Tag tag2 = (Tag) containedNodes[2];
            Assert.AreEqual("b", tag2.Text, "Tag Contents");
            Assert.IsTrue(containedNodes[3] is EndTag, "Fourth contained node should be HTMLEndTag");
            EndTag endTag1 = (EndTag) containedNodes[3];
            Assert.AreEqual("b", endTag1.Text, "Fourth Tag contents");
            Assert.IsTrue(containedNodes[4] is EndTag, "Fifth contained node should be HTMLEndTag");
            EndTag endTag2 = (EndTag) containedNodes[4];
            Assert.AreEqual("font", endTag2.Text, "Fifth Tag contents");
        }

        /// <summary> This is a reproduction of bug 617228, reported by
        /// Stephen J. Harrington. When faced with a link like :
        /// &lt;A
        /// HREF="/cgi-bin/view_search?query_text=postdate&gt;20020701&txt_clr=White&bg_clr=Red&url=http://loc
        /// al
        /// host/Testing/Report
        /// 1.html"&gt;20020702 Report 1&lt;/A&gt;
        /// *
        /// parser is unable to handle the link correctly due to the greater than
        /// symbol being confused to be the end of the tag.
        /// </summary>
        [Test]
        public void QueryLink()
        {
            CreateParser(
                "<A \n" +
                "HREF=\"/cgi-bin/view_search?query_text=postdate>20020701&txt_clr=White&bg_clr=Red&url=http://localhost/Testing/Report1.html\">20020702 Report 1</A>",
                "http://transfer.go.com");
            // Register the image scanner
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node 1 should be a link tag");
            LinkTag linkTag = (LinkTag) node[0];
            AssertStringEquals("Resolved Link",
                               "http://transfer.go.com/cgi-bin/view_search?query_text=postdate>20020701&txt_clr=White&bg_clr=Red&url=http://localhost/Testing/Report1.html",
                               linkTag.Link);
            Assert.AreEqual("20020702 Report 1", linkTag.LinkText, "Resolved Link Text");
        }

        [Test]
        public void NotMailtoLink()
        {
            CreateParser("<A HREF=\"mailto.html\">not@for.real</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];

            Assert.AreEqual("not@for.real", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsFalse(linkTag.MailLink, "Link is not a mail link");
        }

        [Test]
        public void MailtoLink()
        {
            CreateParser("<A HREF=\"mailto:this@is.real\">this@is.real</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("this@is.real", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsTrue(linkTag.MailLink, "Link is a mail link");
        }

        [Test]
        public void JavascriptLink()
        {
            CreateParser("<A HREF=\"javascript:alert('hello');\">say hello</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];

            Assert.AreEqual("say hello", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsTrue(linkTag.JavascriptLink, "Link is a Javascript command");
        }

        [Test]
        public void NotJavascriptLink()
        {
            CreateParser("<A HREF=\"javascript_not.html\">say hello</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];

            Assert.AreEqual("say hello", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsFalse(linkTag.JavascriptLink, "Link is not a Javascript command");
        }

        [Test]
        public void FTPLink()
        {
            CreateParser("<A HREF=\"ftp://some.where.it\">my ftp</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];

            Assert.AreEqual("my ftp", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsTrue(linkTag.FTPLink, "Link is a FTP site");
        }

        [Test]
        public void NotFTPLink()
        {
            CreateParser("<A HREF=\"ftp.html\">my ftp</A>", "http://www.cj.com/");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];

            Assert.AreEqual("my ftp", linkTag.ToPlainTextString(), "Link Plain Text");
            Assert.IsFalse(linkTag.FTPLink, "Link is not a FTP site");
        }

        [Test]
        public void RelativeLinkNotHTMLBug()
        {
            CreateParser("<A HREF=\"newpage.html\">New Page</A>", "http://www.mysite.com/books/some.asp");
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.mysite.com/books/newpage.html", linkTag.Link, "Link");
        }

        [Test]
        public void BadImageInLinkBug()
        {
            CreateParser(
                "<a href=\"registration.asp?EventID=1272\"><img border=\"0\" src=\"\\images\\register.gif\"</a>",
                "http://www.fedpage.com/Event.asp?EventID=1272");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            // Get the image tag from the link

            Node[] insideNodes = new AbstractNode[10];
            int j = 0;
            foreach (Node nestedNode in linkTag)
            {
                insideNodes[j++] = nestedNode;
            }
            Assert.AreEqual(1, j, "Number of contained internal nodes");
            Assert.IsTrue(insideNodes[0] is ImageTag);
            ImageTag imageTag = (ImageTag) insideNodes[0];
            Assert.AreEqual("http://www.fedpage.com/images/register.gif", imageTag.ImageURL, "Image Tag Location");
        }

        /// <summary> This is an attempt to reproduce bug 677874
        /// reported by James Moliere. A link tag of the form
        /// <code>
        /// <a class=rlbA href=/news/866201.asp?0sl=-
        /// 32>Shoe bomber handed life sentence</a>
        /// </code>
        /// is not parsed correctly. The second '=' sign in the link causes
        /// the parser to treat it as a seperate attribute
        /// </summary>
        [Test]
        public void LinkContainsEqualTo()
        {
            CreateParser("<a class=rlbA href=/news/866201.asp?0sl=-" + "32>Shoe bomber handed life sentence</a>");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            AssertType("node type", typeof (LinkTag), node[0]);
            LinkTag linkTag = (LinkTag) node[0];
            AssertStringEquals("link text", "Shoe bomber handed life sentence", linkTag.LinkText);
            AssertStringEquals("link url", "/news/866201.asp?0sl=-32", linkTag.Link);
        }

        [Test]
        public void LinkScannerFilter()
        {
            LinkScanner linkScanner = new LinkScanner(LinkTag.LINK_TAG_FILTER);
            Assert.AreEqual(LinkTag.LINK_TAG_FILTER, linkScanner.Filter, "linkscanner filter");
        }

        [Test]
        public void TagSymbolsInLinkText()
        {
            CreateParser("<a href=\"/cataclysm/Langy-AnEmpireReborn-Ch2.shtml#story\"" +
                         "><< An Empire Reborn: Chapter 2 <<</a>");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            AssertType("node", typeof (LinkTag), node[0]);
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("<< An Empire Reborn: Chapter 2 <<", linkTag.LinkText, "link text");
        }
    }
}
