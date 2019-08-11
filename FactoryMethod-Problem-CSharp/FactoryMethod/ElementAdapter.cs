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
using System.Xml;

namespace Industriallogic.FactoryMethod
{
    public class ElementAdapter : XmlNode
    {
        private XmlElement element;

        public ElementAdapter(XmlElement element)
        {
            this.element = element;
        }

        public XmlElement Element
        {
            get { return element; }
            internal set { element = value; }
        }

        public void Add(XmlNode childNode)
        {
            element.AppendChild(((ElementAdapter) childNode).Element);
        }

        public void AddValue(String textNode)
        {
            XmlText node = element.OwnerDocument.CreateTextNode(textNode);
            element.AppendChild(node);
        }

        public void AddAttribute(String attribute, String value)
        {
            element.SetAttribute(attribute, value);
        }
    }
}
