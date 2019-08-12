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
using Parser = org.htmlparser.Parser;
using StringNode = org.htmlparser.StringNode;
using ParserTestCase = org.htmlparser.ParserTestCase;
using NodeIterator = org.htmlparser.util.NodeIterator;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.tags
{
    [TestFixture]
    public class TagTest : ParserTestCase
    {
        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// </summary>
        [Test]
        public void BodyTagBug1()
        {
            CreateParser(
                "<BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000\nvLink=#551a8b>");
            ParseAndAssertNodeCount(1);
            // The node should be an Tag
            Assert.IsTrue(node[0] is Tag, "Node should be a Tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual(
                "BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000\r\nvLink=#551a8b",
                tag.Text, "Contents of the tag");
        }

        /// <summary> The following should be identified as a tag : <BR>
        /// &lt;MYTAG abcd\n"+
        /// "efgh\n"+
        /// "ijkl\n"+
        /// "mnop&gt;
        /// Creation date: (6/17/2001 5:27:42 PM)
        /// </summary>
        [Test]
        public void LargeTagBug()
        {
            CreateParser("<MYTAG abcd\n" + "efgh\n" + "ijkl\n" + "mnop>");
            ParseAndAssertNodeCount(1);
            // The node should be an Tag
            Assert.IsTrue(node[0] is Tag, "Node should be a Tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("MYTAG abcd\r\nefgh\r\nijkl\r\nmnop", tag.Text, "Contents of the tag");
        }

        /// <summary> Bug reported by Gordon Deudney 2002-03-15
        /// Nested JSP Tags were not working
        /// </summary>
        [Test]
        public void NestedTags()
        {
            string s = "input type=\"text\" value=\"<%=\"test\"%>\" name=\"text\"";
            string line = "<" + s + ">";
            CreateParser(line);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "The node found should have been an Tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual(s, tag.Text, "Tag Contents");
        }

        [Test]
        public void ParseParameter3()
        {
            Tag tag;
            string lin1 = "<DIV class=\"userData\" id=\"oLayout\" name=\"oLayout\"></DIV>";
            CreateParser(lin1);
            NodeIterator en = parser.GetEnumerator();
            System.Collections.Hashtable h;

            try
            {
                Assert.IsTrue(en.MoveNext());
                tag = (Tag) en.Current;
                h = tag.Attributes;
                string classValue = (string) h["CLASS"];
                Assert.AreEqual("userData", classValue, "The class value should be ");
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        [Test]
        public void ParseParameterA()
        {
            Tag tag;
            EndTag etag;
            StringNode snode;
            Node node = null;
            string lin1 =
                "<A href=\"http://www.iki.fi/kaila\" myParameter yourParameter=\"Kaarle Kaaila\">Kaarle's homepage</A><p>Paragraph</p>";
            CreateParser(lin1);
            NodeIterator en = parser.GetEnumerator();
            System.Collections.Hashtable h;
            string a, href, myValue, nice;

            try
            {
                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                tag = (Tag) node;
                h = tag.Attributes;
                a = (string) h[Tag.TAGNAME];
                href = (string) h["HREF"];
                myValue = (string) h["MYPARAMETER"];
                nice = (string) h["YOURPARAMETER"];
                Assert.AreEqual("A", a, "Link tag (A)");
                Assert.AreEqual("http://www.iki.fi/kaila", href, "href value");
                Assert.AreEqual("", myValue, "myparameter value");
                Assert.AreEqual("Kaarle Kaaila", nice, "yourparameter value");

                if (!(node is LinkTag))
                {
                    // linkscanner has eaten up this piece
                    Assert.IsTrue(en.MoveNext());
                    node = (Node) en.Current;
                    snode = (StringNode) node;
                    Assert.AreEqual(snode.Text, "Kaarle's homepage", "Value of element");

                    Assert.IsTrue(en.MoveNext());
                    node = (Node) en.Current;
                    etag = (EndTag) node;
                    Assert.AreEqual(etag.Text, "A", "endtag of link");
                }

                // testing rest
                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                tag = (Tag) node;
                Assert.AreEqual(tag.Text, "p", "following paragraph begins");

                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                snode = (StringNode) node;
                Assert.AreEqual(snode.Text, "Paragraph", "paragraph contents");

                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                etag = (EndTag) node;
                Assert.AreEqual(etag.Text, "p", "paragraph endtag");
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        /// <summary> Test parseParameter method
        /// Created by Kaarle Kaila (august 2001)
        /// the tag name is here G
        /// </summary>
        [Test]
        public void ParseParameterG()
        {
            Tag tag;
            EndTag etag;
            StringNode snode;
            Node node = null;
            string lin1 =
                "<G href=\"http://www.iki.fi/kaila\" myParameter yourParameter=\"Kaila\">Kaarle's homepage</G><p>Paragraph</p>";
            CreateParser(lin1);
            NodeIterator en = parser.GetEnumerator();
            System.Collections.Hashtable h;
            string a, href, myValue, nice;

            try
            {
                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                tag = (Tag) node;
                h = tag.Attributes;
                a = (string) h[Tag.TAGNAME];
                href = (string) h["HREF"];
                myValue = (string) h["MYPARAMETER"];
                nice = (string) h["YOURPARAMETER"];
                Assert.AreEqual(a, "G", "The tagname should be G");
                Assert.AreEqual(href, "http://www.iki.fi/kaila", "Check the http address");
                Assert.AreEqual(myValue, "", "myValue is empty");
                Assert.AreEqual(nice, "Kaila", "The second parameter value");

                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                snode = (StringNode) node;
                Assert.AreEqual(snode.Text, "Kaarle's homepage", snode.Text, "Kaarle's homepage");


                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                etag = (EndTag) node;
                Assert.AreEqual(etag.Text, "G", "Endtag is G");

                // testing rest
                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                tag = (Tag) node;
                Assert.AreEqual(tag.Text, "p", "Follow up by p-tag");

                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                snode = (StringNode) node;
                Assert.AreEqual(snode.Text, "Paragraph", "Verify the paragraph text");

                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                etag = (EndTag) node;
                Assert.AreEqual(etag.Text, "p", "Still patragraph endtag");
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        /// <summary> Test parseParameter method
        /// Created by Kaarle Kaila (august 2002)
        /// the tag name is here A (and should be eaten up by linkScanner)
        /// Tests elements where = sign is surrounded by spaces
        /// </summary>
        [Test]
        public void ParseParameterSpace()
        {
            Tag tag;
            EndTag etag;
            StringNode snode;
            Node node = null;
            string lin1 = "<A yourParameter = \"Kaarle\">Kaarle's homepage</A>";
            CreateParser(lin1);
            NodeIterator en = parser.GetEnumerator();
            System.Collections.Hashtable h;
            string a, nice;

            try
            {
                Assert.IsTrue(en.MoveNext());
                node = (Node) en.Current;
                tag = (Tag) node;
                h = tag.Attributes;
                a = (string) h[Tag.TAGNAME];
                nice = (string) h["YOURPARAMETER"];
                Assert.AreEqual(a, "A", "Link tag (A)");
                Assert.AreEqual("Kaarle", nice, "yourParameter value");

                if (!(node is LinkTag))
                {
                    // linkscanner has eaten up this piece
                    Assert.IsTrue(en.MoveNext());
                    node = (Node) en.Current;
                    snode = (StringNode) node;
                    Assert.AreEqual(snode.Text, "Kaarle's homepage", "Value of element");

                    Assert.IsTrue(en.MoveNext());
                    node = (Node) en.Current;
                    etag = (EndTag) node;
                    Assert.AreEqual(etag.Text, "A", "Still patragraph endtag");
                }
                // testing rest
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        /// <summary> Reproduction of a bug reported by Annette Doyle
        /// This is actually a pretty good example of dirty html - we are in a fix
        /// here, bcos the font tag (the first one) has an erroneous inverted comma. In Tag,
        /// we ignore anything in inverted commas, and dont if its outside. This kind of messes
        /// up our parsing almost completely.
        /// </summary>
        [Test]
        public void StrictParsing()
        {
            string testHTML = "<div align=\"center\">" +
                              "<font face=\"Arial,\"helvetica,\" sans-serif=\"sans-serif\" size=\"2\" color=\"#FFFFFF\">" +
                              "<a href=\"/index.html\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Home</font></a>\n" +
                              "<a href=\"/cia/notices.html\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Notices</font></a>\n" +
                              "<a href=\"/cia/notices.html#priv\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Privacy</font></a>\n" +
                              "<a href=\"/cia/notices.html#sec\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Security</font></a>\n" +
                              "<a href=\"/cia/contact.htm\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Contact Us</font></a>\n" +
                              "<a href=\"/cia/sitemap.html\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Site Map</font></a>\n" +
                              "<a href=\"/cia/siteindex.html\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Index</font></a>\n" +
                              "<a href=\"/search\" link=\"#000000\" vlink=\"#000000\"><font color=\"#FFFFFF\">Search</font></a>\n" +
                              "</font>" + "</div>";

            CreateParser(testHTML, "http://www.cia.gov");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            // Check the tags
            AssertType("node", typeof (Div), node[0]);
            Div div = (Div) node[0];
            Tag fontTag = (Tag) div[0];
            Assert.AreEqual("font face=\"Arial,helvetica,\" sans-serif=\"sans-serif\" size=\"2\" color=\"#FFFFFF\"",
                            fontTag.Text, "Second tag should be corrected");
            // Try to parse the parameters from this tag.
            System.Collections.Hashtable table = fontTag.Attributes;
            Assert.IsNotNull(table, "Parameters table");
            Assert.AreEqual("sans-serif", table["SANS-SERIF"], "font sans-serif parameter");
            Assert.AreEqual("Arial,helvetica,", table["FACE"], "font face parameter");
        }

        [Test]
        public void ToHTML()
        {
            string testHTML = "<MYTAG abcd\n" + "efgh\n" + "ijkl\n" + "mnop>\n" + "<TITLE>Hello</TITLE>\n" +
                              "<A HREF=\"Hello.html\">Hey</A>";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(7);
            // The node should be a Tag
            Assert.IsTrue(node[0] is Tag, "1st Node should be a Tag");
            Tag tag = (Tag) node[0];
            Assert.IsTrue(node[1] is Tag, "2nd Node should be a Tag");
            Assert.IsTrue(node[4] is Tag, "5th Node should be a Tag");
            tag = (Tag) node[1];
            Assert.AreEqual("<TITLE>", tag.ToHtml(), "Raw String of the tag");
            tag = (Tag) node[4];
        }

        [Test]
        public void WithoutParseParameter()
        {
            string testHTML =
                "<A href=\"http://www.iki.fi/kaila\" myParameter yourParameter=\"Kaarle\">Kaarle's homepage</A><p>Paragraph</p>";
            CreateParser(testHTML);
            string result = "";
            try
            {
                foreach (Node node in parser)
                {
                    result += node.ToHtml();
                }
                string expected =
                    "<A HREF=\"http://www.iki.fi/kaila\" MYPARAMETER=\"\" YOURPARAMETER=\"Kaarle\">Kaarle's homepage</A><P>Paragraph</P>";
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        [Test]
        public void EmptyTagParseParameter()
        {
            string testHTML = "<INPUT name=\"foo\" value=\"foobar\" type=\"text\" />";

            CreateParser(testHTML);
            string result = "";
            try
            {
                foreach (Node node in parser)
                {
                    result += node.ToHtml();
                }
                string expected = "<INPUT TYPE=\"text\" VALUE=\"foobar\" NAME=\"foo\"/>";
                AssertStringEquals("Check collected contents to original", expected, result);
            }
            catch (System.InvalidCastException)
            {
                Assert.Fail("Bad class element = " + node.GetType().FullName);
            }
        }

        [Test]
        public void StyleSheetTag()
        {
            string testHTML1 = new string("<link rel src=\"af.css\"/>".ToCharArray());
            CreateParser(testHTML1, "http://www.google.com/test/index.html");
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("af.css", tag["src"], "StyleSheet Source");
        }

        /// <summary> Bug report by Cedric Rosa, causing null pointer exceptions when encountering a broken tag,
        /// and if this has no further lines to parse
        /// </summary>
        [Test]
        public void BrokenTag()
        {
            string testHTML1 = "<br";
            CreateParser(testHTML1);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("br", tag.Text, "Node contents");
        }

        [Test]
        public void TagInsideTag()
        {
            string testHTML = "<META name=\"Hello\" value=\"World </I>\">";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("META name=\"Hello\" value=\"World </I>\"", tag.Text, "Node contents");
            Assert.AreEqual("World </I>", tag["value"], "Meta Content");
        }

        [Test]
        public void IncorrectInvertedCommas()
        {
            string testHTML =
                "<META NAME=\"Author\" CONTENT = \"DORIER-APPRILL E., GERVAIS-LAMBONY P., MORICONI-EBRARD F., NAVEZ-BOUCHANINE F.\"\">";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            AssertStringEquals("Node contents",
                               "META NAME=\"Author\" CONTENT=\"DORIER-APPRILL E., GERVAIS-LAMBONY P., MORICONI-EBRARD F., NAVEZ-BOUCHANINE F.\"",
                               tag.Text);
            System.Collections.Hashtable table = tag.Attributes;
            Assert.AreEqual("DORIER-APPRILL E., GERVAIS-LAMBONY P., MORICONI-EBRARD F., NAVEZ-BOUCHANINE F.",
                            tag["CONTENT"], "Meta Content");
        }

        [Test]
        public void IncorrectInvertedCommas2()
        {
            string testHTML =
                "<META NAME=\"Keywords\" CONTENT=Moscou, modernisation, politique urbaine, sp�cificit�s culturelles, municipalit�, Moscou, modernisation, urban politics, cultural specificities, municipality\">";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            AssertStringEquals("Node contents",
                               "META NAME=\"Keywords\" CONTENT=\"Moscou, modernisation, politique urbaine, sp�cificit�s culturelles, municipalit�, Moscou, modernisation, urban politics, cultural specificities, municipality\"",
                               tag.Text);
        }

        [Test]
        public void IncorrectInvertedCommas3()
        {
            string testHTML =
                "<meta name=\"description\" content=\"Une base de donn�es sur les th�ses de g\"ographie soutenues en France \">";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is Tag, "Node should be a tag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual(
                "meta name=\"description\" content=\"Une base de donn�es sur les th�ses de gographie soutenues en France\"",
                tag.Text, "Node contents");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag()
        {
            string testHTML = "<html><body><>text</body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            Assert.AreEqual("<>text", stringNode.Text, "Third node has incorrect text");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag2()
        {
            string testHTML = "<html><body>text<></body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            Assert.AreEqual("text<>", stringNode.Text, "Third node has incorrect text");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag3()
        {
            string testHTML = "<html><body>text<>text</body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            Assert.AreEqual("text<>text", stringNode.Text, "Third node has incorrect text");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag4()
        {
            string testHTML = "<html><body>text\n<>text</body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            Parser.LineSeparator = "\r\n"; // actually a static method
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            string actual = stringNode.Text;
            Assert.AreEqual("text\r\n<>text", actual, "Third node has incorrect text");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag5()
        {
            string testHTML = "<html><body>text<\n>text</body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            Parser.LineSeparator = "\r\n"; // actually a static method
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            string actual = stringNode.Text;
            Assert.AreEqual("text<\r\n>text", actual, "Third node has incorrect text");
        }

        /// <summary> Ignore empty tags.
        /// </summary>
        [Test]
        public void EmptyTag6()
        {
            string testHTML = "<html><body>text<>\ntext</body></html>";
            CreateParser(testHTML);
            parser.RegisterScanners();
            Parser.LineSeparator = "\r\n"; // actually a static method
            ParseAndAssertNodeCount(5);
            Assert.IsTrue(node[2] is StringNode, "Third node should be a string node");
            StringNode stringNode = (StringNode) node[2];
            string actual = stringNode.Text;
            Assert.AreEqual("text<>\r\ntext", actual, "Third node has incorrect text");
        }

        [Test]
        public void AttributesReconstruction()
        {
            string testHTML = "<TEXTAREA name=\"JohnDoe\" ></TEXTAREA>";
            CreateParser(testHTML);
            ParseAndAssertNodeCount(2);
            Assert.IsTrue(node[0] is Tag, "First node should be an HTMLtag");
            Tag htmlTag = (Tag) node[0];
            string expectedHTML = "<TEXTAREA NAME=\"JohnDoe\">";
            AssertStringEquals("Expected HTML", expectedHTML, htmlTag.ToHtml());
        }

        [Test]
        public void IgnoreState()
        {
            string testHTML = "<A \n" +
                              "HREF=\"/a?b=c>d&e=f&g=h&i=http://localhost/Testing/Report1.html\">20020702 Report 1</A>";
            CreateParser(testHTML);
            Node node = Tag.Find(parser.Reader, testHTML, 0);
            Assert.IsTrue(node is Tag, "Node should be a tag");
            Tag tag = (Tag) node;
            string href = tag["HREF"];
            AssertStringEquals("Resolved Link", "/a?b=c>d&e=f&g=h&i=http://localhost/Testing/Report1.html", href);
        }

        [Test]
        public void ExtractWord()
        {
            string line = "Abc DEF GHHI";
            string word = Tag.ExtractWord(line);
            Assert.AreEqual("ABC", Tag.ExtractWord(line), "ABC", Tag.ExtractWord(line));
            string line2 = "%\n ";
            Assert.AreEqual("%", Tag.ExtractWord(line2), "Word expected for line 2");
            string line3 = "%\n%>";
            Assert.AreEqual("%", Tag.ExtractWord(line3), "Word expected for line 3");
            string line4 = "%=abc%>";
            Assert.AreEqual("%", Tag.ExtractWord(line4), "Word expected for line 4");
            string line5 = "OPTION";
            Assert.AreEqual("OPTION", Tag.ExtractWord(line5), "Word expected for line 5");
        }

        /// <summary> From oyoaha
        /// </summary>
        [Test]
        public void TabText()
        {
            string testHTML = "<a\thref=\"http://cbc.ca\">";
            CreateParser(testHTML);
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is LinkTag, "Node should be a LinkTag");
            LinkTag tag = (LinkTag) node[0];
            string href = tag["HREF"];
            AssertStringEquals("Resolved Link", "http://cbc.ca", href);
        }

        /// <summary> See bug #741026 RegisterScanners() mangles output HTML badly.
        /// </summary>
        [Test]
        public void HTMLOutputOfDifficultLinksWithRegisterScanners()
        {
            // straight out of a real world example
            CreateParser("<a href=http://www.google.com/webhp?hl=en>");
            // register standard scanners (Very Important)
            parser.RegisterScanners();
            string temp = null;
            foreach (Node newNode in parser)
            {
                temp += newNode.ToHtml();
            }
            Assert.IsNotNull(temp, "No nodes");
        }

        /// <summary> See bug #740411 setParsed() has no effect on output.
        /// </summary>
        [Test]
        public void ParameterChange()
        {
            CreateParser("<TABLE BORDER=0>");
            ParseAndAssertNodeCount(1);
            // the node should be an HTMLTag
            Assert.IsTrue(node[0] is Tag, "Node should be a HTMLTag");
            Tag tag = (Tag) node[0];
            Assert.AreEqual("TABLE BORDER=0", tag.Text, "Initial text should be");

            System.Collections.Hashtable tempHash = tag.Attributes;
            tempHash["BORDER"] = "1";
            tag.Attributes = tempHash;

            string s = tag.ToHtml();
        }
    }
}
