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
using System.Xml;
using NUnit.Framework;

namespace Industriallogic.FactoryMethod
{
    [TestFixture]
    public class DOMBuilderTest : AbstractBuilderTest
    {
        protected override OutputBuilder createOutputBuilder(String rootName)
        {
            return new DOMBuilder(rootName);
        }

        [Test]
        public void Document()
        {
            const string expectedRootName = "orders";
            DOMBuilder domBuilder = new DOMBuilder(expectedRootName);
            domBuilder.AddBelow("order");
            domBuilder.AddAttribute("num", "123");
            domBuilder.AddBelow("item");
            domBuilder.AddValue("bird feeder");

            XmlDocument doc = domBuilder.Document;

            System.Xml.XmlNode root = doc.ChildNodes[0];

            String rootName = root.Name;
            Assert.AreEqual(expectedRootName, rootName, "root name");

            String firstChildName = root.ChildNodes[0].Name;
            Assert.AreEqual("order", firstChildName, "first child");
        }
    }
}
