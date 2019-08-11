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

namespace Industriallogic.FactoryMethod
{
    public class XMLBuilder : AbstractBuilder
    {
        public XMLBuilder(String rootName)
        {
            Init(rootName);
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
            XmlNode childNode = new TagNode(child);
            current.Add(childNode);
            parent = current;
            current = childNode;
            history.Push(current);
        }

        public override void AddBeside(String sibling)
        {
            if (current == root)
                throw new SystemException(CANNOT_ADD_BESIDE_ROOT);
            XmlNode siblingNode = new TagNode(sibling);
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
            root = new TagNode(rootName);
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
            return root.ToString();
        }
    }
}
