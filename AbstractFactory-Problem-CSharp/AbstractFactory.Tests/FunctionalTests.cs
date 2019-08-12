/// ***************************************************************************
/// Copyright (c) 2009, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****************************************************************************

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

using NUnit.Framework;

using org.htmlparser;
using org.htmlparser.scanners;
using org.htmlparser.tags;
using org.htmlparser.util;

namespace HtmlParser
{
    [TestFixture]
    public class FunctionalTests
    {
        private const string UriToExamine = "http://www.w3.org";

        /// <summary> Based on a suspected bug report by Annette Doyle,
        /// to check if the no of image tags are correctly
        /// identified by the parser
        /// </summary>
        [Test, Ignore]
        public void NumImageTagsInYahooWithoutRegisteringScanners()
        {
            try
            {
                Assert.AreEqual(
                    FindImageTagCount(),
                    CountImageTagsWithHTMLParser() + OvercountedImageTagsInComments(),
                    "Image Tag Count");
            }
            catch (ParserException e)
            {
                throw new ParserException("Error thrown in call to countImageTagsWithHTMLParser()", e);
            }
        }

        public int FindImageTagCount()
        {
            int imgTagCount = 0;
            try
            {
                System.Uri url = SupportClass.CreateUri(UriToExamine);
                System.IO.Stream responseStream = System.Net.WebRequest.Create(url).GetResponse().GetResponseStream();
                System.IO.StreamReader reader;
                reader = new System.IO.StreamReader(responseStream, System.Text.Encoding.UTF7);
                imgTagCount = CountImageTagsWithoutHTMLParser(reader);
                responseStream.Close();
            }
            catch (System.UriFormatException)
            {
                System.Console.Error.WriteLine("URL was malformed!");
            }
            catch (System.IO.IOException)
            {
                System.Console.Error.WriteLine("IO Exception occurred while trying to open stream");
            }
            return imgTagCount;
        }

        public int CountImageTagsWithHTMLParser()
        {
            Parser parser = new Parser(UriToExamine, new DefaultParserFeedback());
            parser.AddScanner(new ImageScanner("-i", new LinkProcessor()));
            int parserImgTagCount = 0;
            foreach (Node node in parser)
            {
                if (node is ImageTag)
                {
                    parserImgTagCount++;
                }
            }
            return parserImgTagCount;
        }

        // Caution: this method is very naive - it counts IMG tags that appear in <!-- HTML comments -->
        public int CountImageTagsWithoutHTMLParser(System.IO.StreamReader reader)
        {
            string line;
            int imgTagCount = 0;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    // Check the line for image tags
                    string newline = line.ToUpper();
                    int fromIndex = - 1;
                    do
                    {
                        fromIndex = newline.IndexOf("<IMG", fromIndex + 1);
                        if (fromIndex != - 1)
                        {
                            imgTagCount++;
                        }
                    } while (fromIndex != - 1);
                }
            } while (line != null);
            return imgTagCount;
        }

        // The overcount is determined by manual inspection of the UriToExamine page.
        private int OvercountedImageTagsInComments()
        {
            return 1;
        }
    }
}
