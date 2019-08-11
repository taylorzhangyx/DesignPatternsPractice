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

        public abstract void AddAbove(string uncle);
        public abstract void AddAttribute(string name, string value);
        public abstract void AddBelow(string child);
        public abstract void AddBeside(string sibling);
        public abstract void AddValue(string value);
        public abstract void StartNewBuild(string rootName);
    }
}
