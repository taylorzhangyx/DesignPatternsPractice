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
using org.htmlparser.tags.data;
using NUnit.Framework;

namespace org.htmlparser.parserHelper
{
    [TestFixture]
    public class AttributeParserTest : ParserTestCase
    {
        private new AttributeParser parser;
        private Tag tag;
        private System.Collections.Hashtable table;

        [SetUp]
        public void SetUp()
        {
            parser = new AttributeParser();
        }

        public void GetParameterTableFor(string tagContents)
        {
            tag = new Tag(new TagData(0, 0, tagContents, ""));
            table = parser.ParseAttributes(tag);
        }

        [Test]
        public void ParseParameters()
        {
            GetParameterTableFor("a b = \"c\"");
            Assert.AreEqual("c", table["B"], "Value");
        }

        [Test]
        public void ParseTokenValues()
        {
            GetParameterTableFor("a b = \"'\"");
            Assert.AreEqual("'", table["B"], "Value");
        }

        [Test]
        public void ParseEmptyValues()
        {
            GetParameterTableFor("a b = \"\"");
            Assert.AreEqual("", table["B"], "Value");
        }

        [Test]
        public void ParseMissingEqual()
        {
            GetParameterTableFor("a b\"c\"");
            Assert.AreEqual("", table["B"], "ValueB");
        }

        [Test]
        public void TwoParams()
        {
            GetParameterTableFor("PARAM NAME=\"Param1\" VALUE=\"Somik\">\n");
            Assert.AreEqual("Param1", table["NAME"], "Param1");
            Assert.AreEqual("Somik", table["VALUE"], "Somik");
        }

        [Test]
        public void PlainParams()
        {
            GetParameterTableFor("PARAM NAME=Param1 VALUE=Somik");
            Assert.AreEqual("Param1", table["NAME"], "Param1");
            Assert.AreEqual("Somik", table["VALUE"], "Somik");
        }

        [Test]
        public void ValueMissing()
        {
            GetParameterTableFor("INPUT type=\"checkbox\" name=\"Authorize\" value=\"Y\" checked");
            Assert.AreEqual("INPUT", table[Tag.TAGNAME], "Name of Tag");
            Assert.AreEqual("checkbox", table["TYPE"], "Type");
            Assert.AreEqual("Authorize", table["NAME"], "Name");
            Assert.AreEqual("Y", table["VALUE"], "Value");
            Assert.AreEqual("", table["CHECKED"], "Checked");
        }

        /// <summary> This is a simulation of a bug reported by Dhaval Udani - wherein
        /// a space before the end of the tag causes a problem - there is a key
        /// in the table with just a space in it and an empty value
        /// </summary>
        [Test]
        public void IncorrectSpaceKeyBug()
        {
            GetParameterTableFor("TEXTAREA name=\"Remarks\" ");
            // There should only be two keys..
            Assert.AreEqual(2, table.Count, "There should only be two keys");
            // The first key is name
            string key1 = "NAME";
            string value1 = (string) table[key1];
            Assert.AreEqual("Remarks", value1, "Expected value 1");
            string key2 = Tag.TAGNAME;
            Assert.AreEqual("TEXTAREA", table[key2], "Expected Value 2");
        }

        [Test]
        public void NullTag()
        {
            GetParameterTableFor("INPUT type=");
            Assert.AreEqual("INPUT", table[Tag.TAGNAME], "Name of Tag");
            Assert.AreEqual("", table["TYPE"], "Type");
        }

        [Test]
        public void AttributeWithSpuriousEqualTo()
        {
            GetParameterTableFor("a class=rlbA href=/news/866201.asp?0sl=-32");
            AssertStringEquals("href", "/news/866201.asp?0sl=-32", (string) table["HREF"]);
        }

        [Test]
        public void QuestionMarksInAttributes()
        {
            GetParameterTableFor("a href=\"mailto:sam@neurogrid.com?subject=Site Comments\"");
            AssertStringEquals("href", "mailto:sam@neurogrid.com?subject=Site Comments", (string) table["HREF"]);
            AssertStringEquals("tag name", "A", (string) table[Tag.TAGNAME]);
        }

        /// <summary> Believe it or not Moi (vincent_aumont) wants htmlparser to parse a text file
        /// containing something that looks nearly like a tag:
        /// <pre>
        /// "                        basic_string&lt;char, string_char_traits&lt;char&gt;, &lt;&gt;&gt;::basic_string()"
        /// </pre>
        /// This was throwing a null pointer exception when the empty &lt;&gt; was encountered.
        /// Bug #725420 NPE in StringBean.visitTag
        /// *
        /// </summary>
        [Test]
        public void EmptyTag()
        {
            GetParameterTableFor("");
            Assert.IsNotNull(table[Tag.TAGNAME], "No Tag.TAGNAME");
        }
    }
}
