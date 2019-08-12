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

using NUnit.Framework;

namespace org.htmlparser
{
    [TestFixture]
    public class AssertXmlEqualsTest : ParserTestCase
    {
        [Test]
        public void NestedTagWithText()
        {
            AssertXmlEquals("<hello>   <hi>My name is Nothing</hi></hello>",
                            "<hello><hi>My name is Nothing</hi>  </hello>",
                            "nested with text");
        }

        [Test]
        public void ThreeTagsDifferent()
        {
            AssertXmlEquals("<someTag></someTag><someOtherTag>", "<someTag/><someOtherTag>",
                            "two tags different");
        }

        [Test]
        public void OneTag()
        {
            AssertXmlEquals("<someTag>", "<someTag>",
                            "one tag");
        }

        [Test]
        public void TwoTags()
        {
            AssertXmlEquals("<someTag></someTag>", "<someTag></someTag>",
                            "two tags");
        }

        [Test]
        public void TwoTagsDifferent()
        {
            AssertXmlEquals("<someTag></someTag>", "<someTag/>",
                            "two tags different");
        }

        [Test]
        public void TwoTagsDifferent2()
        {
            AssertXmlEquals("<someTag/>", "<someTag></someTag>",
                            "two tags different");
        }

        [Test]
        public void TwoTagsWithSameAttributes()
        {
            AssertXmlEquals("<tag name=\"John\" age=\"22\" sex=\"M\"/>", "<tag sex=\"M\" name=\"John\" age=\"22\"/>",
                            "attributes");
        }

        [Test]
        public void TagWithText()
        {
            AssertXmlEquals("<hello>   My name is Nothing</hello>", "<hello>My name is Nothing  </hello>",
                            "text");
        }

        [Test]
        public void StringWithLineBreaks()
        {
            AssertXmlEquals("testing & refactoring", "testing &\nrefactoring",
                            "string with line breaks");
        }
    }
}
