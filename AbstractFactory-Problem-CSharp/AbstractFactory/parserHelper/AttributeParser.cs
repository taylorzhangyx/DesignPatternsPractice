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


using Tag = org.htmlparser.tags.Tag;

namespace org.htmlparser.parserHelper
{
    /// <author>  Somik Raha, Kaarle Kaila
    /// </author>
    /// <version>  7 AUG 2001
    ///
    /// </version>
    public class AttributeParser
    {
        private const string delima = " \t\r\n\f=\"'>";
        private const string delimb = " \t\r\n\f\"'>";
        private const char doubleQuote = '\"';
        private const char singleQuote = '\'';
        private string delim;

        /// <summary> Method to break the tag into pieces.
        /// </summary>
        /// <param name="returns">a Hastable with elements containing the
        /// pieces of the tag. The tag-name has the value field set to
        /// the constant Tag.TAGNAME. In addition the tag-name is
        /// stored into the Hashtable with the name Tag.TAGNAME
        /// where the value is the name of the tag.
        /// Tag parameters without value
        /// has the value "". Parameters with value are represented
        /// in the Hastable by a name/value pair.
        /// As html is case insensitive but Hastable is not are all
        /// names converted into UPPERCASE to the Hashtable
        /// E.g extract the href values from A-tag's and print them
        /// <pre>
        /// *
        /// Tag tag;
        /// Hashtable h;
        /// String tmp;
        /// NodeReader in = new NodeReader(new FileReader(path),2048);
        /// Parser p = new Parser(in);
        /// foreach (Tag tag in p) {
        /// h = tag.parseParameters();
        /// tmp = (String)h.get(tag.TAGNAME);
        /// if (tmp != null && tmp.equalsIgnoreCase("A")) {;
        /// System.Console.Out.WriteLine("URL is :" + h["HREF"]);
        /// }
        /// </pre>
        /// *
        ///
        /// </param>
        public virtual System.Collections.Hashtable ParseAttributes(Tag tag)
        {
            System.Collections.Hashtable h = new System.Collections.Hashtable();
            string element, name, value, nextPart = null;
            name = null;
            value = null;
            element = null;
            bool waitingForEqual = false;
            delim = delima;
            StringTokenizer tokenizer = new StringTokenizer(tag.Text, delim, true);
            while (true)
            {
                nextPart = GetNextPart(tokenizer, delim);
                delim = delima;
                if (element == null && nextPart != null && !nextPart.Equals("="))
                {
                    element = nextPart;
                    PutDataIntoTable(h, element, null, true);
                }
                else
                {
                    if (nextPart != null && (0 < nextPart.Length))
                    {
                        if (name == null)
                        {
                            if (!nextPart.Substring(0, (1) - (0)).Equals(" "))
                            {
                                name = nextPart;
                                waitingForEqual = true;
                            }
                        }
                        else
                        {
                            if (waitingForEqual)
                            {
                                if (nextPart.Equals("="))
                                {
                                    waitingForEqual = false;
                                    delim = delimb;
                                }
                                else
                                {
                                    PutDataIntoTable(h, name, "", false);
                                    name = nextPart;
                                    value = null;
                                }
                            }
                            if (!waitingForEqual && !nextPart.Equals("="))
                            {
                                value = nextPart;
                                PutDataIntoTable(h, name, value, false);
                                name = null;
                                value = null;
                            }
                        }
                    }
                    else
                    {
                        if (name != null)
                        {
                            if (name.Equals("/"))
                            {
                                PutDataIntoTable(h, Tag.EMPTYTAG, "", false);
                            }
                            else
                            {
                                PutDataIntoTable(h, name, "", false);
                            }
                            name = null;
                            value = null;
                        }
                        break;
                    }
                }
            }
            if (null == element)
                // handle no tag contents
                PutDataIntoTable(h, "", null, true);
            return h;
        }

        private string GetNextPart(StringTokenizer tokenizer, string deli)
        {
            string tokenAccumulator = null;
            bool isDoubleQuote = false;
            bool isSingleQuote = false;
            bool isDataReady = false;
            string currentToken;
            while (isDataReady == false && tokenizer.HasMoreTokens())
            {
                currentToken = tokenizer.NextToken(deli);
                //
                // First let's combine tokens that are inside "" or ''
                //
                if (isDoubleQuote || isSingleQuote)
                {
                    if (isDoubleQuote && currentToken[0] == doubleQuote)
                    {
                        isDoubleQuote = false;
                        isDataReady = true;
                    }
                    else if (isSingleQuote && currentToken[0] == singleQuote)
                    {
                        isSingleQuote = false;
                        isDataReady = true;
                    }
                    else
                    {
                        tokenAccumulator += currentToken;
                        continue;
                    }
                }
                else if (currentToken[0] == doubleQuote)
                {
                    isDoubleQuote = true;
                    tokenAccumulator = "";
                    continue;
                }
                else if (currentToken[0] == singleQuote)
                {
                    isSingleQuote = true;
                    tokenAccumulator = "";
                    continue;
                }
                else
                    tokenAccumulator = currentToken;

                if (tokenAccumulator.Equals(currentToken))
                {
                    if (delim.IndexOf(tokenAccumulator) >= 0)
                    {
                        if (tokenAccumulator.Equals("="))
                        {
                            isDataReady = true;
                        }
                    }
                    else
                    {
                        isDataReady = true;
                    }
                }
                else
                    isDataReady = true;
            }
            return tokenAccumulator;
        }

        private void PutDataIntoTable(System.Collections.Hashtable h, string name, string value, bool isName)
        {
            if (isName && value == null)
                value = Tag.TAGNAME;
            else if (value == null)
                value = "";
            // Hashtable does not accept nulls
            if (isName)
            {
                // store tagname as tag.TAGNAME,tag
                h[value] = name.ToUpper();
            }
            else
            {
                // store tag parameters as NAME, value
                h[name.ToUpper()] = value;
            }
        }
    }
}
