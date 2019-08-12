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
using ImageScanner = org.htmlparser.scanners.ImageScanner;
using ParserTestCase = org.htmlparser.ParserTestCase;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;
using NUnit.Framework;

namespace org.htmlparser.tags
{
    [TestFixture]
    public class ImageTagTest : ParserTestCase
    {
        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void ImageTag()
        {
            CreateParser("<IMG alt=Google height=115 src=\"goo/title_homepage4.gif\" width=305>",
                         "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLImageTag
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            Assert.AreEqual("http://www.google.com/test/goo/title_homepage4.gif", imageTag.ImageURL,
                            "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void ImageTagBug()
        {
            CreateParser("<IMG alt=Google height=115 src=\"../goo/title_homepage4.gif\" width=305>",
                         "http://www.google.com/test/");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLImageTag
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            Assert.AreEqual("http://www.google.com/goo/title_homepage4.gif", imageTag.ImageURL, "The image location");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;BODY aLink=#ff0000 bgColor=#ffffff link=#0000cc onload=setfocus() text=#000000 <BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void ImageTageBug2()
        {
            CreateParser("<IMG alt=Google height=115 src=\"../../goo/title_homepage4.gif\" width=305>",
                         "http://www.google.com/test/test/index.html");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLImageTag
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            Assert.AreEqual("http://www.google.com/goo/title_homepage4.gif", imageTag.ImageURL, "The image location");
        }

        /// <summary> This bug occurs when there is a null pointer exception thrown while scanning a tag using LinkScanner.
        /// Creation date: (7/1/2001 2:42:13 PM)
        /// </summary>
        [Test]
        public void ImageTagSingleQuoteBug()
        {
            CreateParser("<IMG SRC='abcd.jpg'>", "http://www.cj.com/");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            Assert.AreEqual("http://www.cj.com/abcd.jpg", imageTag.ImageURL, "Image incorrect");
        }

        /// <summary> The bug being reproduced is this : <BR>
        /// &lt;A HREF=&gt;Something&lt;A&gt;<BR>
        /// vLink=#551a8b&gt;
        /// The above line is incorrectly parsed in that, the BODY tag is not identified.
        /// Creation date: (6/17/2001 4:01:06 PM)
        /// </summary>
        [Test]
        public void NullImageBug()
        {
            CreateParser("<IMG SRC=>", "http://www.google.com/test/index.html");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLLinkTag
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            AssertStringEquals("The image location", "", imageTag.ImageURL);
        }

        [Test]
        public void ToHTML()
        {
            CreateParser("<IMG alt=Google height=115 src=\"../../goo/title_homepage4.gif\" width=305>",
                         "http://www.google.com/test/test/index.html");
            // Register the image scanner
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));

            ParseAndAssertNodeCount(1);
            // The node should be an HTMLImageTag
            Assert.IsTrue(node[0] is ImageTag, "Node should be a HTMLImageTag");
            ImageTag imageTag = (ImageTag) node[0];
            Assert.AreEqual("Google", imageTag["alt"], "Alt");
            Assert.AreEqual("115", imageTag["height"], "Height");
            Assert.AreEqual("305", imageTag["width"], "Width");
        }
    }
}
