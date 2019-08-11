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
        public DOMBuilder(String rootName)
        {
            Init(rootName);
        }

        public XmlDocument Document { get; private set; }

        public override XmlNode CreateNode(string value)
        {
            return new ElementAdapter(Document.CreateElement(value));
        }


        protected override void Init(String rootName)
        {
            Document = new XmlDocument();
            XmlElement rootNode = Document.CreateElement(rootName);
            root = new ElementAdapter(rootNode);
            Document.AppendChild(rootNode);
            current = root;
            parent = root;
            history = new Stack<XmlNode>();
            history.Push(current);
        }

        public override String ToString()
        {
            StringWriter stringOut = new StringWriter();
            XmlWriter xmlWriter = new XmlTextWriter(stringOut);
            Document.WriteContentTo(xmlWriter);
            stringOut.Close();
            return stringOut.ToString();
        }
    }
}
