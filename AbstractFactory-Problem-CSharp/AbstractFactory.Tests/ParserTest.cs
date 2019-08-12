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
using System.Collections;
using System.IO;
using org.htmlparser.scanners;
using org.htmlparser.tags;
using org.htmlparser.util;
using NUnit.Framework;

namespace org.htmlparser
{
    [TestFixture]
    public class ParserTest : ParserTestCase
    {
        [Test]
        public void NodeIteration()
        {
            System.Text.StringBuilder hugeData = new System.Text.StringBuilder();
            for (int i = 0; i < 5001; i++)
                hugeData.Append('a');
            CreateParser(hugeData.ToString());
            int i2 = 0;
            foreach (Node aNode in parser)
            {
                node[i2++] = aNode;
            }
            Assert.AreEqual(1, i2, "There should be 1 node identified");
        }

        /// <summary> Tests the 'from file' Parser constructor.
        /// </summary>
        [Test]
        public void File()
        {
            string path;
            FileInfo file;
            StreamWriter writer;
            Parser parser;
            Node[] nodes;
            int i;

            path = System.Environment.CurrentDirectory;
            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path += Path.DirectorySeparatorChar.ToString();
            file = new FileInfo(path + "delete_me.html");
            try
            {
                writer = new StreamWriter(file.FullName);
                writer.WriteLine("<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\">");
                writer.WriteLine("<html>");
                writer.WriteLine("<head>");
                writer.WriteLine("<title>test</title>");
                writer.WriteLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">");
                writer.WriteLine("</head>");
                writer.WriteLine("<body>");
                writer.WriteLine("This is a test page ");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
                writer.Close();
                parser = new Parser("file://" + file.FullName, new NoFeedback());
                nodes = new AbstractNode[30];
                i = 0;
                foreach (Node node in parser)
                {
                    nodes[i++] = node;
                }
                Assert.AreEqual(12, i, "Expected nodes");
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void ImageCollection()
        {
            CreateParser("<html>\n" + "<head>\n" +
                         "<meta name=\"generator\" content=\"Created Using Yahoo! PageBuilder 2.60.24\">\n" +
                         "</head>\n" + "<body bgcolor=\"#FFFFFF\" link=\"#0000FF\" vlink=\"#FF0000\" text=\"#000000\"\n" +
                         " onLoad=\"window.onresize=new Function('if (navigator.appVersion==\'Netscape\') history.go(0);');\">\n" +
                         "<div id=\"layer0\" style=\"position:absolute;left:218;top:40;width:240;height:26;\">\n" +
                         "<table width=240 height=26 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><b><font size=\"+2\"><span style=\"font-size:24\">NISHI-HONGWAN-JI</span></font></b></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer1\" style=\"position:absolute;left:75;top:88;width:542;height:83;\">\n" +
                         "<table width=542 height=83 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><span style=\"font-size:14\">The Nihi Hongwanj-ji temple is very traditional, very old, and very beautiful. This is the place that we stayed on our first night in Kyoto. We then attended the morning prayer ceremony, at 6:30 am. Staying here costed us 7,500 yen, which was inclusive of dinner and breakfast, and usage of the o-furo (public bath). Felt more like a luxury hotel than a temple.</span></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer2\" style=\"position:absolute;left:144;top:287;width:128;height:96;\">\n" +
                         "<table width=128 height=96 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><a href=\"nishi-hongwanji1.html\"><img height=96 width=128 src=\"nishi-hongwanji1-thumb.jpg\" border=0 ></a></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer3\" style=\"position:absolute;left:415;top:285;width:128;height:96;\">\n" +
                         "<table width=128 height=96 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><a href=\"nishi-hongwanji3.html\"><img height=96 width=128 src=\"nishi-hongwanji2-thumb.jpg\" border=0 ></a></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer4\" style=\"position:absolute;left:414;top:182;width:128;height:96;\">\n" +
                         "<table width=128 height=96 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><a href=\"higashi-hongwanji.html\"><img height=96 width=128 src=\"higashi-hongwanji-thumb.jpg\" border=0 ></a></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer5\" style=\"position:absolute;left:78;top:396;width:530;height:49;\">\n" +
                         "<table width=530 height=49 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><span style=\"font-size:14\">Click on the pictures to see the full-sized versions. The picture at the top right corner is taken in Higashi-Hongwanji. Nishi means west, and Higashi means east. These two temples are adjacent to each other and represent two different Buddhist sects.</span></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer6\" style=\"position:absolute;left:143;top:180;width:128;height:102;\">\n" +
                         "<table width=128 height=102 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><a href=\"nishi-hongwanji4.html\"><img height=102 width=128 src=\"nishi-hongwanji4-thumb.jpg\" border=0 ></a></td>\n" +
                         "</tr></table></div>\n" +
                         "<div id=\"layer7\" style=\"position:absolute;left:280;top:235;width:124;height:99;\">\n" +
                         "<table width=124 height=99 border=0 cellpadding=0 cellspacing=0><tr valign=\"top\">\n" +
                         "<td><a href=\"nishi-hongwanji-lodging.html\"><img height=99 width=124 src=\"nishi-hongwanji-lodging-thumb.jpg\" border=0 ></a></td>\n" +
                         "</tr></table></div>\n" + "</body>\n" + "</html>");
            parser.RegisterScanners();
            NodeList collectionList = new NodeList();

            foreach (Node node in parser)
            {
                node.CollectInto(collectionList, ImageTag.IMAGE_TAG_FILTER);
            }
            Assert.AreEqual(5, collectionList.Size, "Size of collection list should be 5");
            // All items in collection vector should be links
            foreach (Node node in collectionList)
            {
                Assert.IsTrue(node is ImageTag, "Only images should have been parsed");
            }
        }

        [Test]
        public void RemoveScanner()
        {
            CreateParser("");
            parser.RegisterScanners();
            parser.RemoveScanner(new FormScanner("", parser));
            IDictionary scanners = parser.Scanners;
            TagScanner scanner = (TagScanner) scanners["FORM"];
            Assert.IsNull(scanner, "shouldnt have found scanner");
        }

        [Test]
        public void OutOfMemory()
        {
            CreateParser("<html><head></head>\n" + "<body>\n" + "<table>\n" + "<tr>\n" +
                         "      <td><img src=\"foo.jpg\" alt=\"f's b\"><font\n" + " size=1>blah</font>\n" + "</td>\n" +
                         "</tr>\n" + "</table>\n" + "</body></html>\n");
            foreach (Node node in parser)
            {
                Node dummy = node;
            }
        }

        [Test]
        public void EmbeddedQuoteSplit()
        {
            CreateParser("<html><head></head>\n" + "<body>\n" + "<table>\n" +
                         "<tr><td><img src=\"x\" alt=\"f's b\"><font\n" + "size=1>blah</font></td></tr>\n" +
                         "</table>\n" + "</body></html>");
            int i = 0;
            foreach (Node node in parser)
            {
                if (7 == i)
                {
                    Assert.IsTrue(node is Tag, "not a tag");
                    Assert.IsTrue(((Tag) node)["ALT"].Equals("f's b"), "ALT attribute incorrect");
                }
                i++;
            }
            Assert.AreEqual(16, i, "Expected nodes");
        }
    }
}
