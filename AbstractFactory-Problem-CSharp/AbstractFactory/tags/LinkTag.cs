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
using System.Text;
using Node = org.htmlparser.Node;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using LinkData = org.htmlparser.tags.data.LinkData;
using TagData = org.htmlparser.tags.data.TagData;
using SimpleNodeIterator = org.htmlparser.util.SimpleNodeIterator;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser.tags
{
    /// <summary> Identifies a link tag
    /// </summary>
    public class LinkTag : CompositeTag
    {
        /// <summary> Returns the accesskey element if any inside this link tag
        /// </summary>
        public string AccessKey
        {
            get { return accessKey; }
        }

        /// <summary> Returns the url as a string, to which this link points
        /// </summary>
        public string Link
        {
            get { return link; }
            set
            {
                this.link = value;
                attributes["HREF"] = value;
            }
        }

        /// <summary> Returns the text contained inside this link tag
        /// </summary>
        public string LinkText
        {
            get { return linkText; }
        }

        /// <summary> Return the text contained in this linkinode
        /// Kaarle Kaila 23.10.2001
        /// </summary>
        public override string Text
        {
            get { return ToHtml(); }
        }

        /// <summary> Is this a mail address
        /// </summary>
        /// <returns> boolean true/false
        ///
        /// </returns>
        /// <summary> Insert the method's description here.
        /// Creation date: (8/3/2001 1:49:31 AM)
        /// </summary>
        /// <param name="newMailLink">boolean
        ///
        /// </param>
        public bool MailLink
        {
            get { return mailLink; }
            set { mailLink = value; }
        }

        /// <summary> Tests if the link is javascript
        /// </summary>
        /// <returns> flag indicating if the link is a javascript code
        ///
        /// </returns>
        /// <summary> Set the link as a javascript link.
        /// *
        /// </summary>
        /// <param name="newJavascriptLink">flag indicating if the link is a javascript code
        ///
        /// </param>
        public bool JavascriptLink
        {
            get { return javascriptLink; }
            set { javascriptLink = value; }
        }

        /// <summary> Tests if the link is an FTP link.
        /// *
        /// </summary>
        /// <returns> flag indicating if this link is an FTP link
        ///
        /// </returns>
        public bool FTPLink
        {
            get { return link.IndexOf("ftp://") == 0; }
        }

        /// <summary> Tests if the link is an HTTP link.
        /// *
        /// </summary>
        /// <returns> flag indicating if this link is an HTTP link
        ///
        /// </returns>
        public bool HTTPLink
        {
            get { return (!FTPLink && !HTTPSLink && !JavascriptLink && !MailLink); }
        }

        /// <summary> Tests if the link is an HTTPS link.
        /// *
        /// </summary>
        /// <returns> flag indicating if this link is an HTTPS link
        ///
        /// </returns>
        public bool HTTPSLink
        {
            get { return link.IndexOf("https://") == 0; }
        }

        /// <summary> Tests if the link is an HTTP link or one of its variations (HTTPS, etc.).
        /// *
        /// </summary>
        /// <returns> flag indicating if this link is an HTTP link or one of its variations (HTTPS, etc.)
        ///
        /// </returns>
        public bool HTTPLikeLink
        {
            get { return HTTPLink || HTTPSLink; }
        }

        public const string LINK_TAG_FILTER = "-l";

        /// <summary> The URL where the link points to
        /// </summary>
        protected string link;

        /// <summary> The text of of the link element
        /// </summary>
        protected string linkText;

        /// <summary> The accesskey existing inside this link.
        /// </summary>
        protected string accessKey;

        private bool mailLink;
        private bool javascriptLink;

        /// <summary> Constructor creates a LinkTag object, which basically stores the location
        /// where the link points to, and the text it contains.
        /// <p>
        /// In order to get the contents of the link tag, use foreach (... this).
        /// <p>
        /// The following code will get all the images inside a link tag.
        /// <pre>
        /// ImageTag imageTag;
        /// foreach (Node node in linkTag.LinkData()) {
        /// if (node instanceof ImageTag) {
        /// imageTag = (ImageTag)node;
        /// // Process imageTag
        /// }
        /// }
        /// </pre>
        /// There is another mechanism available that allows for uniform extraction of images. You could do this to
        /// get all images from a web page :
        /// <pre>
        /// Vector imageCollectionVector = new Vector();
        /// foreach (Node node in parser) {
        ///     node.CollectInto(imageCollectionVector,ImageTag.IMAGE_FILTER);
        /// }
        /// </pre>
        /// The link tag processes all its contents in CollectInto().
        /// </summary>
        /// <param name="tagData">The data relating to the tag.
        /// </param>
        /// <param name="compositeTagData">The data regarding the composite structure of the tag.
        /// </param>
        /// <param name="linkData">The data specific to the link tag.
        /// </param>
        /// <seealso cref="">#linkData()
        ///
        /// </seealso>
        public LinkTag(TagData tagData, CompositeTagData compositeTagData, LinkData linkData)
            : base(tagData, compositeTagData)
        {
            this.link = linkData.Link;
            this.linkText = linkData.LinkText;
            this.accessKey = linkData.AccessKey;
            this.mailLink = linkData.MailLink;
            this.javascriptLink = linkData.JavascriptLink;
        }

        /// <summary> Print the contents of this Link Node
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Link to : " + link + "; titled : " + linkText + "; begins at : " + ElementBegin + "; ends at : " +
                      ElementEnd + ", AccessKey=");
            if (accessKey == null)
                sb.Append("null\n");
            else
                sb.Append(accessKey + "\n");

            sb.Append("  " + "LinkData\n");
            sb.Append("  " + "--------\n");
            int i = 0;
            foreach (Node node in this)
            {
                sb.Append("   " + (i++) + " ");
                sb.Append(node.ToString() + "\n");
            }
            sb.Append("  " + "*** END of LinkData ***\n");

            return sb.ToString();
        }

        public override void Accept(NodeVisitor visitor)
        {
            visitor.VisitLinkTag(this);
            base.Accept(visitor);
        }

        public virtual void RemoveChild(int i)
        {
            childTags.Remove(i);
        }
    }
}
