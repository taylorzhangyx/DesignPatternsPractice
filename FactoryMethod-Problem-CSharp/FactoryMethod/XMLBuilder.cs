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



        public override XmlNode CreateNode(string value)
        {
            return new TagNode(value);
        }


        protected override void Init(String rootName)
        {
            root = CreateNode(rootName);
            current = root;
            parent = root;
            history = new Stack<XmlNode>();
            history.Push(current);
        }


        public override String ToString()
        {
            return root.ToString();
        }
    }
}
