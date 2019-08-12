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

namespace org.htmlparser.util
{
    /// <summary> Processor class for links, is present basically as a utility class.
    /// </summary>
    public class LinkProcessor
    {
        /// <summary> Returns the baseUrl.
        /// </summary>
        /// <returns> String
        ///
        /// </returns>
        /// <summary> Sets the baseUrl.
        /// </summary>
        /// <param name="baseUrl">The baseUrl to set
        ///
        /// </param>
        public string BaseUrl
        {
            get { return baseUrl; }

            set { this.baseUrl = value; }
        }

        /// <summary> Overriding base URL.
        /// If set, this is used instead of a provided base URL in extract().
        /// </summary>
        private string baseUrl;

        /// <summary> Create an LinkProcessor.
        /// </summary>
        public LinkProcessor()
        {
            baseUrl = null;
        }

        /// <summary> Create an absolute URL from a possibly relative link and a base URL.
        /// </summary>
        /// <param name="link">The relative portion of a URL.
        /// </param>
        /// <param name="baseURL">The base URL unless overridden by the current baseURL property.
        /// </param>
        /// <returns> The fully qualified URL or the original link if a failure occured.
        ///
        /// </returns>
        public virtual string Extract(string link, string baseURL)
        {
            string ret;

            try
            {
                if (null == link)
                    link = "";
                if (null != BaseUrl)
                    baseURL = BaseUrl;
                if ((null == baseURL) || ("".Equals(link)))
                    ret = link;
                else
                {
                    System.Uri url = ConstructUrl(link, baseURL);
                    ret = url.ToString();
                }
            }
            catch (System.UriFormatException)
            {
                ret = link;
            }

            return (Translate.Decode(ret));
        }

        public virtual System.Uri ConstructUrl(string link, string baseURL)
        {
            string path;
            bool modified;
            bool absolute;
            int index;
            System.Uri url; // constructed URL combining relative link and base
            url = new System.Uri(SupportClass.CreateUri(baseURL), link);
            path = url.AbsolutePath;
            modified = false;
            absolute = link.StartsWith("/");
            if (!absolute)
            {
                // we prefer to fix incorrect relative links
                // this doesn't fix them all, just the ones at the start
                while (path.StartsWith("/."))
                {
                    if (path.StartsWith("/../"))
                    {
                        path = path.Substring(3);
                        modified = true;
                    }
                    else if (path.StartsWith("/./") || path.StartsWith("/."))
                    {
                        path = path.Substring(2);
                        modified = true;
                    }
                    else
                        break;
                }
            }
            // fix backslashes
            while (- 1 != (index = path.IndexOf("/\\")))
            {
                path = path.Substring(0, (index + 1) - (0)) + path.Substring(index + 2);
                modified = true;
            }
            if (modified)
            {
                url = SupportClass.CreateUri(url, path);
            }
            return url;
        }

        /// <summary> Turn spaces into %20.
        /// </summary>
        /// <param name="url">The url containing spaces.
        /// </param>
        /// <returns> The URL with spaces as %20 sequences.
        ///
        /// </returns>
        public static string FixSpaces(string url)
        {
            int index;
            int length;
            char ch;
            System.Text.StringBuilder returnURL;

            index = url.IndexOf((System.Char) ' ');
            if (- 1 != index)
            {
                length = url.Length;
                returnURL = new System.Text.StringBuilder(length*3);
                returnURL.Append(url.Substring(0, (index) - (0)));
                for (int i = index; i < length; i++)
                {
                    ch = url[i];
                    if (ch == ' ')
                        returnURL.Append("%20");
                    else
                        returnURL.Append(ch);
                }
                url = returnURL.ToString();
            }

            return (url);
        }

        /// <summary> Check if a resource is a valid URL.
        /// </summary>
        /// <param name="resourceLocn">The resource to test.
        /// </param>
        /// <returns> <code>true</code> if the resource is a valid URL.
        ///
        /// </returns>
        public static bool IsURL(string resourceLocn)
        {
            System.Uri url;
            bool ret;

            try
            {
                url = SupportClass.CreateUri(resourceLocn);
                ret = true;
            }
            catch (System.UriFormatException)
            {
                ret = false;
            }

            return (ret);
        }

        public static string RemoveLastSlash(string baseUrl)
        {
            if (baseUrl[baseUrl.Length - 1] == '/')
            {
                return baseUrl.Substring(0, (baseUrl.Length - 1) - (0));
            }
            else
            {
                return baseUrl;
            }
        }
    }
}
