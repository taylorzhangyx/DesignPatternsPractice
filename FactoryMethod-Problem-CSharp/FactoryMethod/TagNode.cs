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
using System.Collections.Generic;
using System.Text;

namespace Industriallogic.FactoryMethod
{
    public class TagNode : XmlNode
    {
        private readonly String name = "";
        private readonly StringBuilder attributes;
        private List<XmlNode> children;
        private String value = "";

        public TagNode(String name)
        {
            this.name = name;
            attributes = new StringBuilder("");
        }

        public List<XmlNode> Children
        {
            get
            {
                if (children == null)
                    children = new List<XmlNode>();
                return children;
            }
        }

        public void Add(XmlNode childNode)
        {
            Children.Add(childNode);
        }

        public void AddAttribute(String attribute, String v)
        {
            attributes.Append(" ");
            attributes.Append(attribute);
            attributes.Append("='");
            attributes.Append(v);
            attributes.Append("'");
        }

        public void AddValue(String v)
        {
            value = v;
        }

        public override String ToString()
        {
            return ToStringHelper(new StringBuilder(""));
        }

        protected String ToStringHelper(StringBuilder result)
        {
            WriteOpenTagTo(result);
            WriteValueTo(result);
            WriteChildrenTo(result);
            WriteEndTagTo(result);
            return result.ToString();
        }

        protected void WriteChildrenTo(StringBuilder result)
        {
            foreach (XmlNode node in Children)
            {
                TagNode tagNode = (TagNode) node;
                tagNode.ToStringHelper(result);
            }
        }

        private void WriteValueTo(StringBuilder result)
        {
            if (!value.Equals(""))
                result.Append(value);
        }

        protected void WriteEndTagTo(StringBuilder result)
        {
            result.Append("</");
            result.Append(name);
            result.Append(">");
        }

        protected void WriteOpenTagTo(StringBuilder result)
        {
            result.Append("<");
            result.Append(name);
            result.Append(attributes.ToString());
            result.Append(">");
        }
    }
}
