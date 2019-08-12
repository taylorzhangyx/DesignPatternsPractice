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
    public abstract class AbstractBuilder : OutputBuilder
    {
        protected static readonly String CANNOT_ADD_ABOVE_ROOT = "Cannot Add node above the root node.";
        protected static readonly String CANNOT_ADD_BESIDE_ROOT = "Cannot Add node beside the root node.";

        protected Stack<XmlNode> history = new Stack<XmlNode>();

        protected XmlNode root;
        protected XmlNode parent;
        protected XmlNode current;

        public abstract XmlNode CreateNode(string value);

        protected abstract void Init(String rootName);


        public virtual void AddAbove(String uncle)
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

        public virtual void AddAttribute(String name, String value)
        {
            current.AddAttribute(name, value);
        }

        public virtual void AddBelow(String child)
        {
            XmlNode childNode = CreateNode(child);
            current.Add(childNode);
            parent = current;
            current = childNode;
            history.Push(current);
        }

        public virtual void AddBeside(String sibling)
        {
            if (current == root)
                throw new SystemException(CANNOT_ADD_BESIDE_ROOT);
            XmlNode siblingNode = CreateNode(sibling);
            parent.Add(siblingNode);
            current = siblingNode;
            history.Pop();
            history.Push(current);
        }

        public virtual void AddValue(String value)
        {
            current.AddValue(value);
        }

        public virtual void StartNewBuild(String rootName)
        {
            Init(rootName);
        }
    }
}
