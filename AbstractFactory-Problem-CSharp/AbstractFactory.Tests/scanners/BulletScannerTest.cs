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
using org.htmlparser;
using org.htmlparser.tags;
using org.htmlparser.util;
using NUnit.Framework;

namespace org.htmlparser.scanners
{
    [TestFixture]
    public class BulletScannerTest : ParserTestCase
    {
        [Test]
        public void BulletFound()
        {
            CreateParser("<LI><A HREF=\"collapseHierarchy.html\">Collapse Hierarchy</A>\n" + "</LI>");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(1);
            AssertType("should be a bullet", typeof (Bullet), node[0]);
        }

        [Test]
        public void NonEndedBullets()
        {
            CreateParser("<li>forest practices legislation penalties for non-compliance\n" +
                         " (Kwan)  <A HREF=\"/hansard/37th3rd/h21107a.htm#4384\">4384-5</A>\n" +
                         "<li>passenger rail service\n" +
                         " (MacPhail)  <A HREF=\"/hansard/37th3rd/h21021p.htm#3904\">3904</A>\n" +
                         "<li>referendum on principles for treaty negotiations\n" +
                         " (MacPhail)  <A HREF=\"/hansard/37th3rd/h20313p.htm#1894\">1894</A>\n" +
                         "<li>transportation infrastructure projects\n" +
                         " (MacPhail)  <A HREF=\"/hansard/37th3rd/h21022a.htm#3945\">3945-7</A>\n" +
                         "<li>tuition fee freeze");
            parser.RegisterScanners();
            ParseAndAssertNodeCount(5);
            for (int i = 0; i < nodeCount; i++)
            {
                AssertType("node " + i, typeof (Bullet), node[i]);
            }
        }
    }
}
