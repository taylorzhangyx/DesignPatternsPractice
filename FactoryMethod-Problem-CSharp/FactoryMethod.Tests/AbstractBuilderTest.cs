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
using NUnit.Framework;

namespace Industriallogic.FactoryMethod
{
    [TestFixture]
    public abstract class AbstractBuilderTest
    {
        protected PrettyPrinter prettyPrint;
        protected OutputBuilder builder;
        protected abstract OutputBuilder createOutputBuilder(String rootName);

        [SetUp]
        public void CreatePrinter()
        {
            prettyPrint = new PrettyPrinter();
        }

        [Test]
        [ExpectedException(typeof (SystemException))]
        public void AddAboveRoot()
        {
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            builder.AddAbove("customer");
        }

        [Test]
        public void AddBelow()
        {
            const String expected =
                "<orders>" +
                "<order>" +
                "</order>" +
                "</orders>";
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            assertXMLEquals(expected, builder.ToString(), "built xml");
        }

        [Test]
        public void AddBelowWithAttribute()
        {
            const String expected =
                "<orders>" +
                "<order number='123'>" +
                "</order>" +
                "</orders>";
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            builder.AddAttribute("number", "123");
            assertXMLEquals(expected, builder.ToString(), "built xml");
        }

        [Test]
        public void AddBesideAndAbove()
        {
            const String expected =
                "<orders>" +
                "<order>" +
                "<item/>" +
                "<item/>" +
                "<item/>" +
                "</order>" +
                "<order/>" +
                "</orders>";
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            builder.AddBelow("item");
            builder.AddBeside("item");
            builder.AddBeside("item");
            builder.AddAbove("order");
            assertXMLEquals(expected, builder.ToString(), "beside & above");
        }

        [Test]
        [ExpectedException(typeof (SystemException))]
        public void AddBesideRoot()
        {
            builder = createOutputBuilder("orders");
            builder.AddBeside("customer");
        }

        [Test]
        public void AddValue()
        {
            const String expected =
                "<orders>" +
                "<order>" +
                "123" +
                "</order>" +
                "</orders>";
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            builder.AddValue("123");
            assertXMLEquals(expected, builder.ToString(), "built xml");
        }

        [Test]
        public void OneElementTree()
        {
            const String expected =
                "<orders>" +
                "</orders>";
            builder = createOutputBuilder("orders");
            assertXMLEquals(expected, builder.ToString(), "one element tree");
        }

        [Test]
        public void StartNewBuild()
        {
            const String expected =
                "<fruits>" +
                "<apple>" +
                "</apple>" +
                "</fruits>";
            builder = createOutputBuilder("orders");
            builder.AddBelow("order");
            builder.StartNewBuild("fruits");
            builder.AddBelow("apple");
            assertXMLEquals(expected, builder.ToString(), "start new build");
        }

        [Test]
        public void ValueAndChild()
        {
            const String expected =
                "<flavor>" +
                "this flavor is really good" +
                "<grape>" +
                "</grape>" +
                "</flavor>";
            builder = createOutputBuilder("flavor");
            builder.AddValue("this flavor is really good");
            builder.AddBelow("grape");
            assertXMLEquals(expected, builder.ToString(), "value and child");
        }

        public void assertXMLEquals(string expected, string result, string testName)
        {
            Assert.AreEqual(prettyPrint.Format(expected), prettyPrint.Format(result), testName);
        }
    }
}
