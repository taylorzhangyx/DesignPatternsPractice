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
using Html = org.htmlparser.tags.Html;
using TitleTag = org.htmlparser.tags.TitleTag;
using ParserTestCase = org.htmlparser.ParserTestCase;
using NodeList = org.htmlparser.util.NodeList;
using NUnit.Framework;

namespace org.htmlparser.scanners
{
    [TestFixture]
    public class HtmlTest : ParserTestCase
    {
        [Test]
        public void Scan()
        {
            CreateParser("<html>" + "	<head>" + "		<title>Some Title</title>" + "	</head>" + "	<body>" + "		Some data" +
                         "	</body>" + "</html>");
            parser.AddScanner(new TitleScanner(""));
            parser.AddScanner(new HtmlScanner());
            ParseAndAssertNodeCount(1);
            AssertType("html tag", typeof (Html), this.node[0]);
            Html html = (Html) this.node[0];
            NodeList nodeList = new NodeList();
            html.CollectInto(nodeList, typeof (TitleTag));
            Assert.AreEqual(1, nodeList.Size, "nodelist size");
            Node node = nodeList[0];
            AssertType("expected title tag", typeof (TitleTag), node);
            TitleTag titleTag = (TitleTag) node;
            AssertStringEquals("title", "Some Title", titleTag.Title);
        }
    }
}
