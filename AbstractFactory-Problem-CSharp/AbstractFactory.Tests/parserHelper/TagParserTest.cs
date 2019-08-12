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
using org.htmlparser;
using org.htmlparser.tags;
using org.htmlparser.util;
using NUnit.Framework;

namespace org.htmlparser.parserHelper
{
    [TestFixture]
    public class TagParserTest : ParserTestCase
    {
        private const string TEST_HTML =
            "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.0 Transitional//EN\">" + "<!-- Server: sf-web2 -->" +
            "<html lang=\"en\">" +
            "  <head><link rel=\"stylesheet\" type=\"text/css\" href=\"http://sourceforge.net/cssdef.php\">" +
            "	<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">" +
            "    <TITLE>SourceForge.net: Modify: 711073 - HTMLTagParser not threadsafe as a static variable in Tag</TITLE>" +
            "	<SCRIPT language=\"JavaScript\" type=\"text/javascript\">" + "	<!--" + "	function help_window(helpurl) {" +
            "		HelpWin = window.open( 'http://sourceforge.net' + helpurl,'HelpWindow','scrollbars=yes,resizable=yes,toolbar=no,height=400,width=400');" +
            "	}" + "	// -->" + "	</SCRIPT>" + "		<link rel=\"SHORTCUT ICON\" href=\"/images/favicon.ico\">" +
            "<!-- This is temp javascript for the jump button. If we could actually have a jump script on the server side that would be ideal -->" +
            "<script language=\"JavaScript\" type=\"text/javascript\">" + "<!--" +
            "	function jump(targ,selObj,restore){ //v3.0" + "	if (selObj.options[selObj.selectedIndex].value) " +
            "		eval(targ+\".location='\"+selObj.options[selObj.selectedIndex].value+\"'\");" +
            "	if (restore) selObj.selectedIndex=0;" + "	}" + "	//-->" + "</script>" +
            "<a href=\"http://normallink.com/sometext.html\">" + "<style type=\"text/css\">" + "<!--" +
            "A:link { text-decoration:none }" + "A:visited { text-decoration:none }" +
            "A:active { text-decoration:none }" + "A:hover { text-decoration:underline; color:#0066FF; }" + "-->" +
            "</style>" + "</head>" +
            "<body bgcolor=\"#FFFFFF\" text=\"#000000\" leftmargin=\"0\" topmargin=\"0\" marginwidth=\"0\" marginheight=\"0\" link=\"#003399\" vlink=\"#003399\" alink=\"#003399\">";

        [Test]
        public void TagWithQuotes()
        {
            string testHtml =
                "<img src=\"http://g-images.amazon.com/images/G/01/merchants/logos/marshall-fields-logo-20.gif\" width=87 height=20 border=0 alt=\"Marshall Field's\">";

            CreateParser(testHtml);
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            AssertStringEquals("alt", "Marshall Field's", tag["ALT"]);
        }

        [Test]
        public void EmptyTag()
        {
            CreateParser("<custom/>");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            AssertStringEquals("tag name", "CUSTOM", tag.TagName);
            Assert.IsTrue(tag.EmptyXmlTag, "empty tag");
            AssertStringEquals("html", "<CUSTOM/>", tag.ToHtml());
        }

        [Test]
        public void TagWithCloseTagSymbolInAttribute()
        {
            CreateParser("<tag att=\"a>b\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            AssertStringEquals("attribute", "a>b", tag["att"]);
        }

        [Test]
        public void TagWithOpenTagSymbolInAttribute()
        {
            CreateParser("<tag att=\"a<b\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            AssertStringEquals("attribute", "a<b", tag["att"]);
        }

        [Test]
        public void TagWithSingleQuote()
        {
            CreateParser("<tag att=\'a<b\'>");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            AssertStringEquals("html", "<TAG ATT=\"a<b\">", tag.ToHtml());
            AssertStringEquals("attribute", "a<b", tag["att"]);
        }

        /// <summary> The following multi line test cases are from
        /// bug #725749 Parser does not handle < and > in multi-line attributes
        /// submitted by Joe Robins (zorblak)
        /// </summary>
        [Test]
        public void MultiLine1()
        {
            CreateParser("<meta name=\"foo\" content=\"foo<bar>\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            string html = tag.ToHtml();
            string attribute1 = tag["NAME"];
            AssertStringEquals("attribute 1", "foo", attribute1);
            string attribute2 = tag["CONTENT"];
            AssertStringEquals("attribute 2", "foo<bar>", attribute2);
        }

        [Test]
        public void MultiLine2()
        {
            CreateParser("<meta name=\"foo\" content=\"foo<bar\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            string html = tag.ToHtml();
            string attribute1 = tag["NAME"];
            AssertStringEquals("attribute 1", "foo", attribute1);
            string attribute2 = tag["CONTENT"];
            AssertStringEquals("attribute 2", "foo<bar", attribute2);
        }

        [Test]
        public void MultiLine3()
        {
            CreateParser("<meta name=\"foo\" content=\"foobar>\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            string html = tag.ToHtml();
            string attribute1 = tag["NAME"];
            AssertStringEquals("attribute 1", "foo", attribute1);
            string attribute2 = tag["CONTENT"];
            AssertStringEquals("attribute 2", "foobar>", attribute2);
        }

        [Test]
        public void MultiLine4()
        {
            CreateParser("<meta name=\"foo\" content=\"foo\nbar>\">");
            ParseAndAssertNodeCount(1);
            AssertType("should be Tag", typeof (Tag), node[0]);
            Tag tag = (Tag) node[0];
            string html = tag.ToHtml();
            string attribute1 = tag["NAME"];
            AssertStringEquals("attribute 1", "foo", attribute1);
            string attribute2 = tag["CONTENT"];
            AssertStringEquals("attribute 2", "foo\r\nbar>", attribute2);
        }
    }
}
