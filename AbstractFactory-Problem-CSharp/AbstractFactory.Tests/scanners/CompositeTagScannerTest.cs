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

using System;
using Node = org.htmlparser.Node;
using StringNode = org.htmlparser.StringNode;
using CompositeTag = org.htmlparser.tags.CompositeTag;
using EndTag = org.htmlparser.tags.EndTag;
using Tag = org.htmlparser.tags.Tag;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using ParserTestCase = org.htmlparser.ParserTestCase;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.scanners
{
    [TestFixture]
    public class CompositeTagScannerTest : ParserTestCase
    {
        private class AnonymousClassCompositeTagScanner : CompositeTagScanner
        {
            private CompositeTagScannerTest enclosingInstance;

            public override string[] ID
            {
                get { return null; }
            }

            public CompositeTagScannerTest Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            internal AnonymousClassCompositeTagScanner(CompositeTagScannerTest enclosingInstance, string[] Param1)
                : base(Param1)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
            {
                return null;
            }
        }

        private class AnonymousClassCustomScanner : CustomScanner
        {
            internal AnonymousClassCustomScanner(CompositeTagScannerTest enclosingInstance) : base(enclosingInstance)
            {
            }

            public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
            {
                Enclosing_Instance.url = tagData.UrlBeingParsed;
                return base.CreateTag(tagData, compositeTagData);
            }
        }

        private CompositeTagScanner scanner;
        private string url;

        [SetUp]
        public void SetUp()
        {
            string[] arr = new string[] {"SOMETHING"};
            scanner = new AnonymousClassCompositeTagScanner(this, arr);
        }

        private CustomTag ParseCustomTag(int expectedNodeCount)
        {
            parser.AddScanner(new CustomScanner(this));
            ParseAndAssertNodeCount(expectedNodeCount);
            AssertType("node", typeof (CustomTag), node[0]);
            CustomTag customTag = (CustomTag) node[0];
            return customTag;
        }

        [Test]
        public void EmptyCompositeTag()
        {
            CreateParser("<Custom/>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(0, customTag.ChildCount, "child count");
            Assert.IsTrue(customTag.EmptyXmlTag, "custom tag should be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(8, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("html", "<CUSTOM/>", customTag.ToHtml());
        }

        [Test]
        public void EmptyCompositeTagAnotherStyle()
        {
            CreateParser("<Custom></Custom>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(0, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            Assert.AreEqual("<CUSTOM></CUSTOM>", customTag.ToHtml(), "html");
        }

        [Test]
        public void CompositeTagWithOneTextChild()
        {
            CreateParser("<Custom>" + "Hello" + "</Custom>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");

            Node child = customTag[0];
            AssertType("child", typeof (StringNode), child);
            StringNode text = (StringNode) child;
            AssertStringEquals("child text", "Hello", child.ToPlainTextString());
        }

        [Test]
        public void CompositeTagWithTagChild()
        {
            CreateParser("<Custom>" + "<Hello>" + "</Custom>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(0, customTag.ElementBegin, "custom tag starting loc");
            Assert.AreEqual(23, customTag.ElementEnd, "custom tag ending loc");

            Node child = customTag[0];
            AssertType("child", typeof (Tag), child);
            Tag tag = (Tag) child;
            AssertStringEquals("child html", "<HELLO>", child.ToHtml());
        }

        [Test]
        public void CompositeTagWithAnotherTagChild()
        {
            CreateParser("<Custom>" + "<Another/>" + "</Custom>");
            parser.AddScanner(new AnotherScanner());
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(0, customTag.ElementBegin, "custom tag starting loc");
            Assert.AreEqual(26, customTag.ElementEnd, "custom tag ending loc");

            Node child = customTag[0];
            AssertType("child", typeof (AnotherTag), child);
            AnotherTag tag = (AnotherTag) child;
            Assert.AreEqual(8, tag.ElementBegin, "another tag start pos");
            Assert.AreEqual(17, tag.ElementEnd, "another tag ending pos");

            Assert.AreEqual(18, customTag.EndTag.ElementBegin, "custom end tag start pos");
            AssertStringEquals("child html", "<ANOTHER/>", child.ToHtml());
        }

        [Test]
        public void ParseTwoCompositeTags()
        {
            CreateParser("<Custom>" + "</Custom>" + "<Custom/>");
            parser.AddScanner(new CustomScanner(this));
            ParseAndAssertNodeCount(2);
            AssertType("tag 1", typeof (CustomTag), node[0]);
            AssertType("tag 2", typeof (CustomTag), node[1]);
        }

        [Test]
        public void XmlTypeCompositeTags()
        {
            CreateParser("<Custom>" + "<Another name=\"subtag\"/>" + "<Custom />" + "</Custom>" + "<Custom/>");
            parser.AddScanner(new CustomScanner(this));
            parser.AddScanner(new AnotherScanner());
            ParseAndAssertNodeCount(2);
            AssertType("first node", typeof (CustomTag), this.node[0]);
            AssertType("second node", typeof (CustomTag), this.node[1]);
            CustomTag customTag = (CustomTag) this.node[0];
            Node node = customTag[0];
            AssertType("first child", typeof (AnotherTag), node);
            node = customTag[1];
            AssertType("second child", typeof (CustomTag), node);
        }

        [Test]
        public void CompositeTagWithNestedTag()
        {
            CreateParser("<Custom>" + "<Another>" + "Hello" + "</Another>" + "<Custom/>" + "</Custom>" + "<Custom/>");
            parser.AddScanner(new CustomScanner(this));
            parser.AddScanner(new AnotherScanner());
            ParseAndAssertNodeCount(2);
            AssertType("first node", typeof (CustomTag), this.node[0]);
            AssertType("second node", typeof (CustomTag), this.node[1]);
            CustomTag customTag = (CustomTag) this.node[0];
            Node node = customTag[0];
            AssertType("first child", typeof (AnotherTag), node);
            AnotherTag anotherTag = (AnotherTag) node;
            Assert.AreEqual(1, anotherTag.ChildCount, "another tag children count");
            node = anotherTag[0];
            AssertType("nested child", typeof (StringNode), node);
            StringNode text = (StringNode) node;
            Assert.AreEqual("Hello", text.ToPlainTextString(), "text");
        }

        [Test]
        public void CompositeTagWithTwoNestedTags()
        {
            CreateParser("<Custom>" + "<Another>" + "Hello" + "</Another>" + "<unknown>" + "World" + "</unknown>" +
                         "<Custom/>" + "</Custom>" + "<Custom/>");
            parser.AddScanner(new CustomScanner(this));
            parser.AddScanner(new AnotherScanner());
            ParseAndAssertNodeCount(2);
            AssertType("first node", typeof (CustomTag), this.node[0]);
            AssertType("second node", typeof (CustomTag), this.node[1]);
            CustomTag customTag = (CustomTag) this.node[0];
            Assert.AreEqual(5, customTag.ChildCount, "first custom tag children count");
            Node node = customTag[0];
            AssertType("first child", typeof (AnotherTag), node);
            AnotherTag anotherTag = (AnotherTag) node;
            Assert.AreEqual(1, anotherTag.ChildCount, "another tag children count");
            node = anotherTag[0];
            AssertType("nested child", typeof (StringNode), node);
            StringNode text = (StringNode) node;
            Assert.AreEqual("Hello", text.ToPlainTextString(), "text");
        }

        [Test]
        public void ErroneousCompositeTag()
        {
            CreateParser("<custom>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(0, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("html", "<CUSTOM></CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void ErroneousCompositeTagWithChildren()
        {
            CreateParser("<custom>" + "<firstChild>" + "<secondChild>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(2, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("html", "<CUSTOM><FIRSTCHILD><SECONDCHILD></CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void ErroneousCompositeTagWithChildrenAndLineBreak()
        {
            CreateParser("<custom>" + "<firstChild>\n" + "<secondChild>");
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(2, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(2, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("html", "<CUSTOM><FIRSTCHILD>\r\n" + "<SECONDCHILD>" + "</CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void TwoConsecutiveErroneousCompositeTags()
        {
            CreateParser("<custom>something" + "<custom></endtag>");
            parser.AddScanner(new CustomScanner(this, false));
            ParseAndAssertNodeCount(2);
            CustomTag customTag = (CustomTag) node[0];
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(25, customTag.ElementEnd, "ending loc of custom tag");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("first custom tag", "<CUSTOM>something</CUSTOM>", customTag.ToHtml());
            customTag = (CustomTag) node[1];
            AssertStringEquals("second custom tag", "<CUSTOM></ENDTAG></CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void CompositeTagWithErroneousAnotherTagAndLineBreak()
        {
            CreateParser("<another>" + "<custom>\n" + "</custom>");
            parser.AddScanner(new AnotherScanner());
            parser.AddScanner(new CustomScanner(this));
            ParseAndAssertNodeCount(2);
            AnotherTag anotherTag = (AnotherTag) node[0];
            Assert.AreEqual(0, anotherTag.ChildCount, "another tag child count");

            CustomTag customTag = (CustomTag) node[1];
            int x = customTag.ChildCount;
            Assert.AreEqual(0, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(9, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(16, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(2, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("another tag html", "<ANOTHER></ANOTHER>", anotherTag.ToHtml());
            AssertStringEquals("custom tag html", "<CUSTOM>\r\n</CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void CompositeTagWithErroneousAnotherTag()
        {
            CreateParser("<custom>" + "<another>" + "</custom>");
            parser.AddScanner(new AnotherScanner(true));
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            AnotherTag anotherTag = (AnotherTag) customTag[0];
            Assert.AreEqual(26, anotherTag.ElementEnd, "another tag ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AssertStringEquals("html", "<CUSTOM><ANOTHER></ANOTHER></CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void CompositeTagWithDeadlock()
        {
            CreateParser("<custom>" + "<another>something" + "</custom>" + "<custom>" + "<another>else</another>" +
                         "</custom>");
            parser.AddScanner(new AnotherScanner(true));
            CustomTag customTag = ParseCustomTag(2);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            Assert.AreEqual(1, customTag.tagData.StartLine, "starting line position");
            Assert.AreEqual(1, customTag.tagData.EndLine, "ending line position");
            AnotherTag anotherTag = (AnotherTag) customTag[0];
            Assert.AreEqual(1, anotherTag.ChildCount, "anotherTag child count");
            StringNode stringNode = (StringNode) anotherTag[0];
            AssertStringEquals("anotherTag child text", "something", stringNode.ToPlainTextString());
            AssertStringEquals("first custom tag html", "<CUSTOM><ANOTHER>something</ANOTHER></CUSTOM>",
                               customTag.ToHtml());
            customTag = (CustomTag) node[1];
            AssertStringEquals("second custom tag html", "<CUSTOM><ANOTHER>else</ANOTHER></CUSTOM>", customTag.ToHtml());
        }

        [Test]
        public void CompositeTagCorrectionWithSplitLines()
        {
            CreateParser("<custom>" + "<another><abcdefg>\n" + "</custom>");
            parser.AddScanner(new AnotherScanner(true));
            CustomTag customTag = ParseCustomTag(1);
            int x = customTag.ChildCount;
            Assert.AreEqual(1, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");
            Assert.AreEqual(0, customTag.StartTag.ElementBegin, "starting loc");
            Assert.AreEqual(7, customTag.StartTag.ElementEnd, "ending loc");
            AnotherTag anotherTag = (AnotherTag) customTag[0];
            Assert.AreEqual(1, anotherTag.ChildCount, "anotherTag child count");
            Assert.AreEqual(9, anotherTag.ElementEnd, "anotherTag end loc");
            Assert.AreEqual(10, customTag.EndTag.ElementBegin, "custom end tag begin loc");
            Assert.AreEqual(8, customTag.EndTag.ElementEnd, "custom end tag end loc");
        }

        [Test]
        public void CompositeTagWithSelfChildren()
        {
            CreateParser("<custom>" + "<custom>something</custom>" + "</custom>");
            parser.AddScanner(new CustomScanner(this, false));
            parser.AddScanner(new AnotherScanner());
            ParseAndAssertNodeCount(3);

            CustomTag customTag = (CustomTag) node[0];
            int x = customTag.ChildCount;
            Assert.AreEqual(0, customTag.ChildCount, "child count");
            Assert.IsFalse(customTag.EmptyXmlTag, "custom tag should not be xml end tag");

            AssertStringEquals("first custom tag html", "<CUSTOM></CUSTOM>", customTag.ToHtml());
            customTag = (CustomTag) node[1];
            AssertStringEquals("first custom tag html", "<CUSTOM>something</CUSTOM>", customTag.ToHtml());
            EndTag endTag = (EndTag) node[2];
            AssertStringEquals("first custom tag html", "</CUSTOM>", endTag.ToHtml());
        }

        [Test]
        public void ParentConnections()
        {
            CreateParser("<custom>" + "<custom>something</custom>" + "</custom>");
            parser.AddScanner(new CustomScanner(this, false));
            parser.AddScanner(new AnotherScanner());
            ParseAndAssertNodeCount(3);

            CustomTag customTag = (CustomTag) node[0];

            AssertStringEquals("first custom tag html", "<CUSTOM></CUSTOM>", customTag.ToHtml());
            Assert.IsNull(customTag.Parent, "first custom tag should have no parent");

            customTag = (CustomTag) node[1];
            AssertStringEquals("first custom tag html", "<CUSTOM>something</CUSTOM>", customTag.ToHtml());
            Assert.IsNull(customTag.Parent, "second custom tag should have no parent");

            Node firstChild = customTag[0];
            AssertType("firstChild", typeof (StringNode), firstChild);
            CompositeTag parent = firstChild.Parent;
            Assert.IsNotNull(parent, "first child parent should not be null");
            Assert.AreSame(customTag, parent, "parent and custom tag should be the same");

            EndTag endTag = (EndTag) node[2];
            AssertStringEquals("first custom tag html", "</CUSTOM>", endTag.ToHtml());
            Assert.IsNull(endTag.Parent, "end tag should have no parent");
        }

        [Test]
        public void UrlBeingProvidedToCreateTag()
        {
            CreateParser("<Custom/>", "http://www.yahoo.com");

            parser.AddScanner(new AnonymousClassCustomScanner(this));
            ParseAndAssertNodeCount(1);
            AssertStringEquals("url", "http://www.yahoo.com", url);
        }

        [Test]
        public void ComplexNesting()
        {
            CreateParser("<custom>" + "<custom>" + "<another>" + "</custom>" + "<custom>" + "<another>" + "</custom>" +
                         "</custom>");
            parser.AddScanner(new CustomScanner(this));
            parser.AddScanner(new AnotherScanner(false));
            ParseAndAssertNodeCount(1);
            AssertType("root node", typeof (CustomTag), node[0]);
            CustomTag root = (CustomTag) node[0];
            AssertNodeCount("child count", 2, root.ChildrenAsNodeArray);
            Node child = root[0];
            AssertType("child", typeof (CustomTag), child);
            CustomTag customChild = (CustomTag) child;
            AssertNodeCount("grand child count", 1, customChild.ChildrenAsNodeArray);
            Node grandchild = customChild[0];
            AssertType("grandchild", typeof (AnotherTag), grandchild);
        }

        [Test]
        public void DisallowedChildren()
        {
            CreateParser("<custom>\n" + "Hello" + "<custom>\n" + "World" + "<custom>\n" + "Hey\n" + "</custom>");
            parser.AddScanner(new CustomScanner(this, false));
            ParseAndAssertNodeCount(3);
            for (int i = 0; i < nodeCount; i++)
            {
                AssertType("node " + i, typeof (CustomTag), node[i]);
            }
        }

        public class CustomScanner : CompositeTagScanner
        {
            private CompositeTagScannerTest enclosingInstance;

            public CompositeTagScannerTest Enclosing_Instance
            {
                get { return enclosingInstance; }
            }

            public override string[] ID
            {
                get { return MATCH_NAME; }
            }

            private static readonly string[] MATCH_NAME = new string[] {"CUSTOM"};

            public CustomScanner(CompositeTagScannerTest enclosingInstance) : this(enclosingInstance, true)
            {
            }

            public CustomScanner(CompositeTagScannerTest enclosingInstance, bool selfChildrenAllowed)
                : base("", MATCH_NAME, new string[] {}, selfChildrenAllowed)
            {
                this.enclosingInstance = enclosingInstance;
            }

            public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
            {
                return new CustomTag(tagData, compositeTagData);
            }
        }

        public class AnotherScanner : CompositeTagScanner
        {
            public override string[] ID
            {
                get { return MATCH_NAME; }
            }

            protected virtual bool BrokenTag
            {
                get { return false; }
            }

            private static readonly string[] MATCH_NAME = new string[] {"ANOTHER"};

            public AnotherScanner() : base("", MATCH_NAME, new string[] {"CUSTOM"})
            {
            }

            public AnotherScanner(bool acceptCustomTagsButDontAcceptCustomEndTags)
                : base("", MATCH_NAME, new string[] {}, new string[] {"CUSTOM"}, true)
            {
            }

            public override Tag CreateTag(TagData tagData, CompositeTagData compositeTagData)
            {
                return new AnotherTag(tagData, compositeTagData);
            }
        }

        public class CustomTag : CompositeTag
        {
            public TagData tagData;

            public CustomTag(TagData tagData, CompositeTagData compositeTagData) : base(tagData, compositeTagData)
            {
                this.tagData = tagData;
            }
        }

        public class AnotherTag : CompositeTag
        {
            public AnotherTag(TagData tagData, CompositeTagData compositeTagData) : base(tagData, compositeTagData)
            {
            }
        }
    }
}
