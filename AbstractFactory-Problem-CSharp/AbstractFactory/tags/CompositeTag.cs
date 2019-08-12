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
using System.Text;
using org.htmlparser;
using AbstractNode = org.htmlparser.AbstractNode;
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using NodeList = org.htmlparser.util.NodeList;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser.tags
{
    public abstract class CompositeTag : Tag
    {
        public Node[] ChildrenAsNodeArray
        {
            get { return childTags.ToNodeArray(); }
        }

        public IEnumerator GetEnumerator()
        {
            return childTags.GetEnumerator();
        }

        public string ChildrenHTML
        {
            get
            {
                StringBuilder buff = new StringBuilder();
                foreach (AbstractNode node in this)
                {
                    buff.Append(node.ToHtml());
                }
                return buff.ToString();
            }
        }

        public int ChildCount
        {
            get { return childTags.Size; }
        }

        public Tag StartTag
        {
            get { return startTag; }
        }

        public Tag EndTag
        {
            get { return endTag; }
        }

        protected Tag startTag, endTag;
        protected NodeList childTags;

        public CompositeTag(TagData tagData, CompositeTagData compositeTagData) : base(tagData)
        {
            this.childTags = compositeTagData.Children;
            this.startTag = compositeTagData.StartTag;
            this.endTag = compositeTagData.EndTag;
        }

        public override string ToPlainTextString()
        {
            StringBuilder stringRepresentation = new StringBuilder();
            foreach (Node node in this)
            {
                stringRepresentation.Append(node.ToPlainTextString());
            }
            return stringRepresentation.ToString();
        }

        public virtual void PutStartTagInto(StringBuilder sb)
        {
            sb.Append(startTag.ToHtml());
        }

        protected virtual void PutChildrenInto(StringBuilder sb)
        {
            Node prevNode = startTag;
            foreach (Node node in this)
            {
                if (prevNode != null)
                {
                    if (prevNode.ElementEnd > node.ElementBegin)
                    {
                        // It's a new line
                        sb.Append(Parser.LineSeparator);
                    }
                }
                sb.Append(node.ToHtml());
                prevNode = node;
            }
            if (prevNode.ElementEnd > endTag.ElementBegin)
            {
                sb.Append(Parser.LineSeparator);
            }
        }

        protected virtual void PutEndTagInto(StringBuilder sb)
        {
            sb.Append(endTag.ToHtml());
        }

        public override string ToHtml()
        {
            StringBuilder sb = new StringBuilder();
            PutStartTagInto(sb);
            if (!startTag.EmptyXmlTag)
            {
                PutChildrenInto(sb);
                PutEndTagInto(sb);
            }
            return sb.ToString();
        }

        /// <summary> Searches all children who for a name attribute. Returns first match.
        /// </summary>
        /// <param name="name">Attribute to match in tag
        /// </param>
        /// <returns> Tag Tag matching the name attribute
        ///
        /// </returns>
        public virtual Tag SearchByName(string name)
        {
            foreach (Node node in this)
            {
                if (node is Tag)
                {
                    Tag tag = (Tag) node;
                    string nameAttribute = tag["NAME"];
                    if (nameAttribute != null && nameAttribute.Equals(name))
                        return tag;
                }
            }

            return null;
        }

        /// <summary> Searches for any node whose text representation contains the search
        /// string. Collects all such nodes in a NodeList.
        /// e.g. if you wish to find any textareas in a form tag containing "hello
        /// world", the code would be :
        /// <code>
        /// NodeList nodeList = formTag.SearchFor("Hello World");
        /// </code>
        /// </summary>
        /// <param name="searchString">search criterion
        /// </param>
        /// <param name="caseSensitive">specify whether this search should be case
        /// sensitive
        /// </param>
        /// <returns> NodeList Collection of nodes whose string contents or
        /// representation have the searchString in them
        ///
        /// </returns>
        public virtual NodeList SearchFor(string searchString, bool caseSensitive)
        {
            NodeList foundList = new NodeList();
            if (!caseSensitive)
                searchString = searchString.ToUpper();
            foreach (Node node in this)
            {
                string nodeTextString = node.ToPlainTextString();
                if (!caseSensitive)
                    nodeTextString = nodeTextString.ToUpper();
                if (nodeTextString.IndexOf(searchString) != - 1)
                    foundList.Add(node);
            }
            return foundList;
        }

        /// <summary> Collect all objects that are of a certain type
        /// Note that this will not check for parent types, and will not
        /// recurse through child tags
        /// </summary>
        /// <param name="classType">
        /// </param>
        /// <returns> NodeList
        ///
        /// </returns>
        public virtual NodeList SearchFor(Type classType)
        {
            return childTags.SearchFor(classType);
        }

        /// <summary> Searches for any node whose text representation contains the search
        /// string. Collects all such nodes in a NodeList.
        /// e.g. if you wish to find any textareas in a form tag containing "hello
        /// world", the code would be :
        /// <code>
        /// NodeList nodeList = formTag.SearchFor("Hello World");
        /// </code>
        /// This search is <b>case-insensitive</b>.
        /// </summary>
        /// <param name="searchString">search criterion
        /// </param>
        /// <returns> NodeList Collection of nodes whose string contents or
        /// representation have the searchString in them
        ///
        /// </returns>
        public virtual NodeList SearchFor(string searchString)
        {
            return SearchFor(searchString, false);
        }

        /// <summary> Returns the node number of the string node containing the
        /// given text. This can be useful to index into the composite tag
        /// and get other children.
        /// </summary>
        /// <param name="">text
        /// </param>
        /// <returns> int
        ///
        /// </returns>
        public virtual int FindPositionOf(string text)
        {
            int loc = 0;
            foreach (Node node in this)
            {
                if (node.ToPlainTextString().ToUpper().IndexOf(text.ToUpper()) != - 1)
                {
                    return loc;
                }
                loc++;
            }
            return -1;
        }

        /// <summary> Returns the node number of a child node given the node object.
        /// This would typically be used in conjuction with digUpStringNode,
        /// after which the string node's parent can be used to find the
        /// string node's position. Faster than calling findPositionOf(text)
        /// again. Note that the position is at a linear level alone - there
        /// is no recursion in this method.
        /// </summary>
        /// <param name="searchNode">
        /// </param>
        /// <returns> int
        ///
        /// </returns>
        public virtual int FindPositionOf(Node searchNode)
        {
            int loc = 0;
            foreach (Node node in this)
            {
                if (node == searchNode)
                {
                    return loc;
                }
                loc++;
            }
            return -1;
        }

        /// <summary> Get child at given index
        /// </summary>
        /// <param name="">index
        /// </param>
        /// <returns> Node
        ///
        /// </returns>
        public Node this[int index]
        {
            get { return childTags[index]; }
        }

        public override void CollectInto(NodeList collectionList, string filter)
        {
            base.CollectInto(collectionList, filter);
            foreach (Node node in this)
            {
                node.CollectInto(collectionList, filter);
            }
        }

        public override void CollectInto(NodeList collectionList, Type nodeType)
        {
            base.CollectInto(collectionList, nodeType);
            foreach (Node node in this)
            {
                node.CollectInto(collectionList, nodeType);
            }
        }

        public override void Accept(NodeVisitor visitor)
        {
            if (visitor.ShouldRecurseChildren())
            {
                startTag.Accept(visitor);
                foreach (Node child in this)
                {
                    child.Accept(visitor);
                }
                endTag.Accept(visitor);
            }
            if (visitor.ShouldRecurseSelf())
                visitor.VisitTag(this);
        }

        /// <summary> Finds a string node, however embedded it might be, and returns
        /// it. The string node will retain links to its parents, so
        /// further navigation is possible.
        /// </summary>
        /// <param name="">searchText
        /// </param>
        /// <returns> The list of string nodes (recursively) found.
        ///
        /// </returns>
        public virtual StringNode[] DigupStringNode(string searchText)
        {
            NodeList nodeList = SearchFor(searchText);
            NodeList stringNodes = new NodeList();

            for (int i = 0; i < nodeList.Size; i++)
            {
                Node node = nodeList[i];
                if (node is StringNode)
                {
                    stringNodes.Add(node);
                }
                else
                {
                    if (node is CompositeTag)
                    {
                        CompositeTag ctag = (CompositeTag) node;
                        StringNode[] nodes = ctag.DigupStringNode(searchText);
                        foreach (Node nestedNode in nodes)
                        {
                            stringNodes.Add(nestedNode);
                        }
                    }
                }
            }
            StringNode[] stringNode = new StringNode[stringNodes.Size];
            for (int i = 0; i < stringNode.Length; i++)
            {
                stringNode[i] = (StringNode) stringNodes[i];
            }
            return stringNode;
        }
    }
}
