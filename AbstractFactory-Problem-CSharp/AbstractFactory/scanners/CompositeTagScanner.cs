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
using System.Collections.Specialized;
using Node = org.htmlparser.Node;
using NodeReader = org.htmlparser.NodeReader;
using CompositeTagScannerHelper = org.htmlparser.parserHelper.CompositeTagScannerHelper;
using EndTag = org.htmlparser.tags.EndTag;
using Tag = org.htmlparser.tags.Tag;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using ParserException = org.htmlparser.util.ParserException;

namespace org.htmlparser.scanners
{
    /// <summary> To create your own scanner that can hold children, create a subclass of this class.
    /// The composite tag scanner can be configured with:<br>
    /// <ul>
    /// <li>Tags which will trigger a match</li>
    /// <li>Tags which when encountered before a legal end tag, should force a correction</li>
    /// <li>Preventing more tags of its own type to appear as children
    /// </ul>
    /// Here are examples of each:<BR>
    /// <B>Tags which will trigger a match</B>
    /// If we wish to recognize &lt;mytag&gt;,
    /// <pre>
    /// MyScanner extends CompositeTagScanner {
    /// private static final String [] MATCH_IDS = { "MYTAG" };
    /// MyScanner() {
    /// super(MATCH_IDS);
    /// }
    /// ...
    /// }
    /// </pre>
    /// <B>Tags which force correction</B>
    /// If we wish to insert end tags if we get a </BODY> or </HTML> without recieving
    /// &lt;/mytag&gt;
    /// <pre>
    /// MyScanner extends CompositeTagScanner {
    /// private static readonly string[] MATCH_IDS = { "MYTAG" };
    /// private static readonly string[] ENDERS = {};
    /// private static readonly string[] END_TAG_ENDERS = { "BODY", "HTML" };
    /// MyScanner() : base(MATCH_IDS, ENDERS, END_TAG_ENDERS, true) {
    /// }
    /// ...
    /// }
    /// </pre>
    /// <B>Preventing children of same type</B>
    /// This is useful when you know that a certain tag can never hold children of its own type.
    /// e.g. &lt;FORM&gt; can never have more form tags within it. If it does, it is an error and should
    /// be corrected. The default behavior is to allow nesting.
    /// <pre>
    /// MyScanner extends CompositeTagScanner {
    /// private static readonly string[] MATCH_IDS = { "FORM" };
    /// private static readonly string[] ENDERS = {};
    /// private static readonly string[] END_TAG_ENDERS = { "BODY", "HTML" };
    /// MyScanner() : base(MATCH_IDS, ENDERS,END_TAG_ENDERS, false) {
    /// }
    /// ...
    /// }
    /// </pre>
    /// Inside the scanner, use CreateTag() to specify what tag needs to be created.
    /// </summary>
    public abstract class CompositeTagScanner : TagScanner
    {
        public bool AllowSelfChildren
        {
            get { return allowSelfChildren; }
        }

        protected string[] nameOfTagToMatch;
        private bool allowSelfChildren;
        protected IList tagEnderSet;
        private IList endTagEnderSet;
        private bool balance_quotes;

        public CompositeTagScanner(string[] nameOfTagToMatch) : this(nameOfTagToMatch, new string[] {})
        {
        }

        public CompositeTagScanner(string[] nameOfTagToMatch, string[] tagEnders)
            : this("", nameOfTagToMatch, tagEnders)
        {
        }

        public CompositeTagScanner(string[] nameOfTagToMatch, string[] tagEnders, bool allowSelfChildren)
            : this("", nameOfTagToMatch, tagEnders, allowSelfChildren)
        {
        }

        public CompositeTagScanner(string filter, string[] nameOfTagToMatch)
            : this(filter, nameOfTagToMatch, new string[] {}, true)
        {
        }

        public CompositeTagScanner(string filter, string[] nameOfTagToMatch, string[] tagEnders)
            : this(filter, nameOfTagToMatch, tagEnders, true)
        {
        }

        public CompositeTagScanner(string filter, string[] nameOfTagToMatch, string[] tagEnders, bool allowSelfChildren)
            : this(filter, nameOfTagToMatch, tagEnders, new string[] {}, allowSelfChildren)
        {
        }

        public CompositeTagScanner(string filter, string[] nameOfTagToMatch, string[] tagEnders, string[] endTagEnders,
                                   bool allowSelfChildren)
            : this(filter, nameOfTagToMatch, tagEnders, endTagEnders, allowSelfChildren, false)
        {
        }

        /// <summary> Constructor specifying all member fields.
        /// </summary>
        /// <param name="filter">A string that is used to match which tags are to be allowed
        /// to pass through. This can be useful when one wishes to dynamically filter
        /// out all tags except one type which may be programmed later than the parser.
        /// </param>
        /// <param name="nameOfTagToMatch">The tag names recognized by this scanner.
        /// </param>
        /// <param name="tagEnders">The non-endtag tag names which signal that no closing
        /// end tag was found. For example, encountering &lt;FORM&gt; while
        /// scanning a &lt;A&gt; link tag would mean that no &lt;/A&gt; was found
        /// and needs to be corrected.
        /// </param>
        /// <param name="endTagEnders">The endtag names which signal that no closing end
        /// tag was found. For example, encountering &lt;/HTML&gt; while
        /// scanning a &lt;BODY&gt; tag would mean that no &lt;/BODY&gt; was found
        /// and needs to be corrected. These items are not prefixed by a '/'.
        /// </param>
        /// <param name="allowSelfChildren">If <code>true</code> a tag of the same name is
        /// allowed within this tag. Used to determine when an endtag is missing.
        /// </param>
        /// <param name="balance_quotes"><code>true</code> if scanning string nodes needs to
        /// honour quotes. For example, ScriptScanner defines this <code>true</code>
        /// so that text within &lt;SCRIPT&gt;&lt;/SCRIPT&gt; ignores tag-like text
        /// within quotes.
        ///
        /// </param>
        public CompositeTagScanner(string filter, string[] nameOfTagToMatch, string[] tagEnders, string[] endTagEnders,
                                   bool allowSelfChildren, bool balance_quotes) : base(filter)
        {
            this.nameOfTagToMatch = nameOfTagToMatch;
            this.allowSelfChildren = allowSelfChildren;
            this.balance_quotes = balance_quotes;
            this.tagEnderSet = new StringCollection();
            for (int i = 0; i < tagEnders.Length; i++)
                tagEnderSet.Add(tagEnders[i]);
            this.endTagEnderSet = new StringCollection();
            for (int i = 0; i < endTagEnders.Length; i++)
                endTagEnderSet.Add(endTagEnders[i]);
        }

        public override Tag Scan(Tag tag, string url, NodeReader reader, string currLine)
        {
            CompositeTagScannerHelper helper = new CompositeTagScannerHelper(this, tag, url, reader, currLine,
                                                                             balance_quotes);
            return helper.Scan();
        }

        /// <summary> Override this method if you wish to create any data structures or do anything
        /// before the start of the scan. This is just after a tag has triggered the scanner
        /// but before the scanner begins its processing.
        /// </summary>
        public virtual void BeforeScanningStarts()
        {
        }

        /// <summary> This method is called everytime a child to the composite is found. It is useful when we
        /// need to store special children seperately. Though, all children are collected anyway into a node list.
        /// </summary>
        public virtual void ChildNodeEncountered(Node node)
        {
        }

        /// <summary> You must override this method to create the tag of your choice upon successful parsing. Data required
        /// for construction of your tag can be found within tagData and compositeTagData
        /// </summary>
        public abstract Tag CreateTag(TagData tagData, CompositeTagData compositeTagData);

        public bool IsTagToBeEndedFor(Tag tag)
        {
            bool isEndTag = tag is EndTag;
            string tagName = tag.TagName;
            if ((isEndTag && endTagEnderSet.Contains(tagName)) || (!isEndTag && tagEnderSet.Contains(tagName)))
                return true;
            else
                return false;
        }

        /// <summary> Override this method to implement scanner logic that determines if the current scanner is
        /// to be allowed. This is useful when there are rules which dont allow recursive tags of the same
        /// </summary>
        /// <seealso cref="">BulletScanner
        /// </seealso>
        /// <returns> boolean true/false
        ///
        /// </returns>
        public virtual bool ShouldCreateEndTagAndExit()
        {
            return false;
        }
    }
}
