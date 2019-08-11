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
using System.IO;
using System.Xml;

namespace Industriallogic.FactoryMethod
{
    public class DOMBuilder : AbstractBuilder
    {
        private XmlDocument doc;

        public DOMBuilder(String rootName)
        {
            Init(rootName);
        }

        public XmlDocument Document
        {
            get { return doc; }
        }

        public override void AddAbove(String uncle)
        {
            if (current == root)
                throw new SystemException(CANNOT_ADD_ABOVE_ROOT);
            history.Pop();
            bool atRootNode = (history.Count == 1);
            if (atRootNode)
                throw new SystemException(CANNOT_ADD_ABOVE_ROOT);
            history.Pop();
            current = history.Peek();
            AddBelow(uncle);
        }

        public override void AddAttribute(String name, String value)
        {
            current.AddAttribute(name, value);
        }

        public override void AddBelow(String child)
        {
            XmlNode childNode = new ElementAdapter(doc.CreateElement(child));
            current.Add(childNode);
            parent = current;
            current = childNode;
            history.Push(current);
        }

        public override void AddBeside(String sibling)
        {
            if (current == root)
                throw new SystemException(CANNOT_ADD_BESIDE_ROOT);
            XmlNode siblingNode = new ElementAdapter(doc.CreateElement(sibling));
            parent.Add(siblingNode);
            current = siblingNode;
            history.Pop();
            history.Push(current);
        }

        public override void AddValue(String value)
        {
            current.AddValue(value);
        }

        protected void Init(String rootName)
        {
            doc = new XmlDocument();
            XmlElement rootNode = doc.CreateElement(rootName);
            root = new ElementAdapter(rootNode);
            doc.AppendChild(rootNode);
            current = root;
            parent = root;
            history = new Stack<XmlNode>();
            history.Push(current);
        }

        public override void StartNewBuild(String rootName)
        {
            Init(rootName);
        }

        public override String ToString()
        {
            StringWriter stringOut = new StringWriter();
            XmlWriter xmlWriter = new XmlTextWriter(stringOut);
            doc.WriteContentTo(xmlWriter);
            stringOut.Close();
            return stringOut.ToString();
        }
    }
}
