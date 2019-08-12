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
using System;
using System.Collections;
using Node = org.htmlparser.Node;
using NodeReader = org.htmlparser.NodeReader;
using TagScanner = org.htmlparser.scanners.TagScanner;
using Tag = org.htmlparser.tags.Tag;

namespace org.htmlparser.util
{
    public class ParserUtils
    {
        public static bool EvaluateTag(TagScanner pTagScanner, string pTagString, string pTagName)
        {
            pTagString = TagScanner.AbsorbLeadingBlanks(pTagString);
            if (pTagString.ToUpper().IndexOf(pTagName) == 0)
                return true;
            else
                return false;
        }

        public static string ToString(Tag tag)
        {
            string tagName = tag[Tag.TAGNAME];
            System.Collections.Hashtable attrs = tag.Attributes;

            System.Text.StringBuilder lString = new System.Text.StringBuilder(tagName);
            lString.Append(" TAG\n");
            lString.Append("--------\n");

            foreach (string key in attrs.Keys)
            {
                string value = (string) attrs[key];
                if (!key.ToUpper().Equals(Tag.TAGNAME.ToUpper()) && value.Length > 0)
                    lString.Append(key).Append(" : ").Append(value).Append("\n");
            }

            return lString.ToString();
        }

        public static IDictionary AdjustScanners(NodeReader reader)
        {
            IDictionary tempScanners = new Hashtable();
            tempScanners = reader.Parser.Scanners;
            // Remove all existing scanners
            reader.Parser.FlushScanners();
            return tempScanners;
        }

        public static void RestoreScanners(NodeReader reader, IDictionary tempScanners)
        {
            // Flush the scanners
            reader.Parser.Scanners = tempScanners;
        }

        public static string RemoveChars(string s, char occur)
        {
            System.Text.StringBuilder newString = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < s.Length; i++)
            {
                ch = s[i];
                if (ch != occur)
                    newString.Append(ch);
            }
            return newString.ToString();
        }

        public static string RemoveEscapeCharacters(string inputString)
        {
            inputString = ParserUtils.RemoveChars(inputString, '\r');
            inputString = ParserUtils.RemoveChars(inputString, '\n');
            inputString = ParserUtils.RemoveChars(inputString, '\t');
            return inputString;
        }

        public static string RemoveLeadingBlanks(string plainText)
        {
            while (plainText.IndexOf((System.Char) ' ') == 0)
                plainText = plainText.Substring(1);
            return plainText;
        }

        public static string RemoveTrailingBlanks(string text)
        {
            char ch = ' ';
            while (ch == ' ')
            {
                ch = text[text.Length - 1];
                if (ch == ' ')
                    text = text.Substring(0, (text.Length - 1) - (0));
            }
            return text;
        }

        /// <summary> Search given node and pick up any objects of given type, return
        /// Node array.
        /// </summary>
        /// <param name="">node
        /// </param>
        /// <param name="">type
        /// </param>
        /// <returns> Node[]
        ///
        /// </returns>
        public static Node[] FindTypeInNode(Node node, Type type)
        {
            NodeList nodeList = new NodeList();
            node.CollectInto(nodeList, type);
            Node[] spans = nodeList.ToNodeArray();
            return spans;
        }
    }
}
