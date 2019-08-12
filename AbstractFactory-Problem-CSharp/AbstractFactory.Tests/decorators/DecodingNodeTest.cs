// ***************************************************************************
// Copyright (c) 2016, Industrial Logic, Inc., All Rights Reserved.
//
// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
// used by students during Industrial Logic's workshops or by individuals
// who are being coached by Industrial Logic on a project.
//
// This code may NOT be copied or used for any other purpose without the prior
// written consent of Industrial Logic, Inc.
// ****************************************************************************

// HTMLParser Library v1_4_20030713 - A java-based parser for HTML
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
// Industrial Logic Corporation
// 2583 Cedar Street, Berkeley,
// CA 94708, USA
// Website : http://www.industriallogic.com
//
// This test class produced by Joshua Kerievsky
using System;
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.decorators
{
    [TestFixture]
    public class DecodingNodeTest : ParserTestCase
    {
        private string ParseToObtainDecodedResult(string STRING_TO_DECODE)
        {
            System.Text.StringBuilder decodedContent = new System.Text.StringBuilder();
            CreateParser(STRING_TO_DECODE);
            parser.ShouldDecodeNodes = true; // tell parser to decode StringNodes

            foreach (Node node in parser)
            {
                decodedContent.Append(node.ToPlainTextString());
            }

            return decodedContent.ToString();
        }

        [Test]
        public void Ampersand()
        {
            string ENCODED = "This &amp; That";
            string DECODED = "This & That";

            Assert.AreEqual(DECODED, ParseToObtainDecodedResult(ENCODED), "ampersand in string");
        }

        [Test]
        public void NumericReference()
        {
            string ENCODED_DIVISION_SIGN = "&#247; is the division sign.";
            string DECODED_DIVISION_SIGN = "÷ is the division sign.";

            Assert.AreEqual(DECODED_DIVISION_SIGN, ParseToObtainDecodedResult(ENCODED_DIVISION_SIGN),
                            "numeric reference for division sign");
        }

        [Test]
        public void ReferencesInString()
        {
            string ENCODED_REFERENCE_IN_STRING = "Thus, the character entity reference &divide; is a more convenient" +
                                                 " form than &#247; for obtaining the division sign (÷)";
            string DECODED_REFERENCE_IN_STRING = "Thus, the character entity reference ÷ is a more convenient" +
                                                 " form than ÷ for obtaining the division sign (÷)";

            Assert.AreEqual(DECODED_REFERENCE_IN_STRING, ParseToObtainDecodedResult(ENCODED_REFERENCE_IN_STRING),
                            "character references within a string");
        }

        [Test]
        public void BogusCharacterEntityReference()
        {
            string ENCODED_BOGUS_CHARACTER_ENTITY = "The character entity reference &divode; is bogus";
            string DECODED_BOGUS_CHARACTER_ENTITY = "The character entity reference &divode; is bogus";

            Assert.AreEqual(DECODED_BOGUS_CHARACTER_ENTITY, ParseToObtainDecodedResult(ENCODED_BOGUS_CHARACTER_ENTITY),
                            "bogus character entity reference");
        }

        [Test]
        public void DecodingNonBreakingSpaceDoesNotOccur()
        {
            string ENCODED_WITH_NON_BREAKING_SPACE = "Here is string with \u00a0.";
            string DECODED_WITH_NON_BREAKING_SPACE = "Here is string with \u00a0.";

            Assert.AreEqual(DECODED_WITH_NON_BREAKING_SPACE, ParseToObtainDecodedResult(ENCODED_WITH_NON_BREAKING_SPACE),
                            "bogus character entity reference");
        }
    }
}
