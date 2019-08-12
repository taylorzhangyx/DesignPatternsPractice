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


//$CopyrightHeader()$

using System;
using org.htmlparser.tags;
using org.htmlparser.util;
using org.htmlparser.visitors;

namespace org.htmlparser.decorators
{
    public class DecodingNode : Node
    {
        private Node delegateNode;

        public DecodingNode(Node node)
        {
            delegateNode = node;
        }

        public String ToPlainTextString()
        {
            return Translate.Decode(delegateNode.ToPlainTextString());
        }

        public void Accept(NodeVisitor visitor)
        {
            delegateNode.Accept(visitor);
        }

        public void CollectInto(NodeList collectionList, Type nodeType)
        {
            delegateNode.CollectInto(collectionList, nodeType);
        }

        public void CollectInto(NodeList collectionList, string filter)
        {
            delegateNode.CollectInto(collectionList, filter);
        }

        public int ElementBegin
        {
            get { return delegateNode.ElementBegin; }
        }

        public int ElementEnd
        {
            get { return delegateNode.ElementEnd; }
        }

        public CompositeTag Parent
        {
            get { return delegateNode.Parent; }
            set { delegateNode.Parent = value; }
        }

        public String Text
        {
            get { return delegateNode.Text; }
            set { delegateNode.Text = value; }
        }

        public String ToHtml()
        {
            return delegateNode.ToHtml();
        }
    }
}
