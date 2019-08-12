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
using Parser = org.htmlparser.Parser;
using LinkScanner = org.htmlparser.scanners.LinkScanner;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using LinkData = org.htmlparser.tags.data.LinkData;
using TagData = org.htmlparser.tags.data.TagData;
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.tags
{
    [TestFixture]
    public class LinkTagTest : ParserTestCase
    {
        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkNodeBug()
        {
            CreateParser("<A HREF=\"../test.html\">abcd</A>", "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            Assert.AreEqual("http://www.google.com/test.html", linkNode.Link, "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkNodeBug2()
        {
            CreateParser("<A HREF=\"../../test.html\">abcd</A>", "http://www.google.com/test/test/index.html");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            Assert.AreEqual("http://www.google.com/test.html", linkNode.Link, "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// When a url ends with a slash, and the link begins with a slash,the parser puts two slashes
        /// This bug was submitted by Roget Kjensrud
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkNodeBug3()
        {
            CreateParser("<A HREF=\"/mylink.html\">abcd</A>", "http://www.cj.com/");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            Assert.AreEqual("http://www.cj.com/mylink.html", linkNode.Link, "Link incorrect");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// Simple url without index.html, doesent get appended to link
        /// This bug was submitted by Roget Kjensrud
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkNodeBug4()
        {
            CreateParser("<A HREF=\"/mylink.html\">abcd</A>", "http://www.cj.com");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            Assert.AreEqual("http://www.cj.com/mylink.html", linkNode.Link, "Link incorrect!!");
        }

        [Test]
        public void LinkNodeBug5()
        {
            CreateParser(
                "<a href=http://note.kimo.com.tw/>���O</a>&nbsp; <a \n" +
                "href=http://photo.kimo.com.tw/>��ï</a>&nbsp; <a\n" +
                "href=http://address.kimo.com.tw/>�q�T��</a>&nbsp;&nbsp;", "http://www.cj.com");
            Parser.LineSeparator = "\r\n";
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(6);
            // The node should be an LinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a LinkTag");
            LinkTag linkNode = (LinkTag) node[2];
            AssertStringEquals("Link incorrect!!", "http://photo.kimo.com.tw/", linkNode.Link);
            Assert.AreEqual(48, linkNode.ElementBegin, "Link beginning");
            Assert.AreEqual(38, linkNode.ElementEnd, "Link ending");

            LinkTag linkNode2 = (LinkTag) node[4];
            AssertStringEquals("Link incorrect!!", "http://address.kimo.com.tw/", linkNode2.Link);
            Assert.AreEqual(46, linkNode2.ElementBegin, "Link beginning");
            Assert.AreEqual(42, linkNode2.ElementEnd, "Link ending");
        }

        /// <summary> This bug occurs when there is a null pointer exception thrown while scanning a tag using LinkScanner.
        /// Creation date: (7/1/2001 2:42:13 PM)
        /// </summary>
        [Test]
        public void LinkNodeBugNullPointerException()
        {
            CreateParser(
                "<FORM action=http://search.yahoo.com/bin/search name=f><MAP name=m><AREA\n" +
                "coords=0,0,52,52 href=\"http://www.yahoo.com/r/c1\" shape=RECT><AREA" +
                "coords=53,0,121,52 href=\"http://www.yahoo.com/r/p1\" shape=RECT><AREA" +
                "coords=122,0,191,52 href=\"http://www.yahoo.com/r/m1\" shape=RECT><AREA" +
                "coords=441,0,510,52 href=\"http://www.yahoo.com/r/wn\" shape=RECT>", "http://www.cj.com/");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));
            ParseAndAssertNodeCount(6);
        }

        /// <summary> This bug occurs when there is a null pointer exception thrown while scanning a tag using LinkScanner.
        /// Creation date: (7/1/2001 2:42:13 PM)
        /// </summary>
        [Test]
        public void LinkNodeMailtoBug()
        {
            CreateParser("<A HREF='mailto:somik@yahoo.com'>hello</A>", "http://www.cj.com/");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            AssertStringEquals("Link incorrect", "somik@yahoo.com", linkNode.Link);
            Assert.AreEqual(true, linkNode.MailLink, "Link Type");
        }

        /// <summary> This bug occurs when there is a null pointer exception thrown while scanning a tag using LinkScanner.
        /// Creation date: (7/1/2001 2:42:13 PM)
        /// </summary>
        [Test]
        public void LinkNodeSingleQuoteBug()
        {
            CreateParser("<A HREF='abcd.html'>hello</A>", "http://www.cj.com/");

            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkNode = (LinkTag) node[0];
            Assert.AreEqual("http://www.cj.com/abcd.html", linkNode.Link, "Link incorrect");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkTag()
        {
            CreateParser("<A HREF=\"test.html\">abcd</A>", "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.google.com/test/test.html", linkTag.Link, "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void LinkTagBug()
        {
            CreateParser("<A HREF=\"../test.html\">abcd</A>", "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("http://www.google.com/test.html", linkTag.Link, "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;A HREF=&gt;Something&lt;A&gt;<BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void NullTagBug()
        {
            CreateParser("<A HREF=>Something</A>", "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("", linkTag.Link, "The link location");
            Assert.AreEqual("Something", linkTag.LinkText, "The link text");
        }

        [Test]
        public void ToPlainTextString()
        {
            CreateParser("<A HREF='mailto:somik@yahoo.com'>hello</A>", "http://www.cj.com/");
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.AreEqual("hello", linkTag.ToPlainTextString(), "Link Plain Text");
        }

        [Test]
        public void ToHTML()
        {
            CreateParser(
                "<A HREF='mailto:somik@yahoo.com'>hello</A>\n" +
                "<LI><font color=\"FF0000\" size=-1><b>Tech Samachar:</b></font><a \n" +
                "href=\"http://ads.samachar.com/bin/redirect/tech.txt?http://www.samachar.com/tech\n" +
                "nical.html\"> Journalism 3.0</a> by Rajesh Jain", "http://www.cj.com/");
            Parser.LineSeparator = "\r\n";
            // Register the image scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(9);
            Assert.IsTrue(node[0] is LinkTag, "First Node should be a HTMLLinkTag");
            LinkTag linkTag = (LinkTag) node[0];
            Assert.IsTrue(node[7] is LinkTag, "Eighth Node should be a HTMLLinkTag");
            linkTag = (LinkTag) node[7];
        }

        [Test]
        public void TypeHttps()
        {
            LinkTag linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                          new LinkData("https://www.someurl.com", "", "", false, false));
            Assert.IsTrue(linkTag.HTTPSLink, "This is a https link");
        }

        [Test]
        public void TypeFtp()
        {
            LinkTag linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                          new LinkData("ftp://www.someurl.com", "", "", false, false));
            Assert.IsTrue(linkTag.FTPLink, "This is an ftp link");
        }

        [Test]
        public void TypeJavaScript()
        {
            LinkTag linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                          new LinkData("javascript://www.someurl.com", "", "", false, true));
            Assert.IsTrue(linkTag.JavascriptLink, "This is a javascript link");
        }

        [Test]
        public void TypeHttpLink()
        {
            LinkTag linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                          new LinkData("http://www.someurl.com", "", "", false, false));
            Assert.IsTrue(linkTag.HTTPLink, "This is a http link : " + linkTag.Link);
            linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                  new LinkData("somePage.html", "", "", false, false));
            Assert.IsTrue(linkTag.HTTPLink, "This relative link is alsp a http link : " + linkTag.Link);
            linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                  new LinkData("ftp://somePage.html", "", "", false, false));
            Assert.IsFalse(linkTag.HTTPLink, "This is not a http link : " + linkTag.Link);
        }

        [Test]
        public void TypeHttpLikeLink()
        {
            LinkTag linkTag = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                          new LinkData("http://", "", "", false, false));
            Assert.IsTrue(linkTag.HTTPLikeLink, "This is a http link");
            LinkTag linkTag2 = new LinkTag(new TagData(0, 0, "", ""), new CompositeTagData(null, null, null),
                                           new LinkData("https://www.someurl.com", "", "", false, false));
            Assert.IsTrue(linkTag2.HTTPLikeLink, "This is a https link");
        }

        /// <summary> Bug #738504 MailLink != HTTPLink
        /// </summary>
        [Test]
        public void MailToIsNotAHTTPLink()
        {
            LinkTag link;

            CreateParser("<A HREF='mailto:derrickoswald@users.sourceforge.net'>Derrick</A>", "http://sourceforge.net");
            // Register the link scanner
            parser.AddScanner(new LinkScanner("-l"));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a HTMLLinkTag");
            link = (LinkTag) node[0];
            Assert.IsFalse(link.HTTPLink, "bug #738504 MailLink != HTTPLink");
            Assert.IsFalse(link.HTTPSLink, "bug #738504 MailLink != HTTPSLink");
        }
    }
}
