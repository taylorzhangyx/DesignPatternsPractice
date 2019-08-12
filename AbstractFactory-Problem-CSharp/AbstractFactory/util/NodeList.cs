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
using AbstractNode = org.htmlparser.AbstractNode;
using Node = org.htmlparser.Node;

namespace org.htmlparser.util
{
    public class NodeList
    {
        public int NumberOfAdjustments
        {
            get { return numberOfAdjustments; }
        }

        private const int INITIAL_CAPACITY = 10;
        private Node[] nodeData;
        private int size;
        private int capacity;
        private int capacityIncrement;
        private int numberOfAdjustments;

        public NodeList()
        {
            size = 0;
            capacity = INITIAL_CAPACITY;
            nodeData = new AbstractNode[capacity];
            capacityIncrement = capacity*2;
            numberOfAdjustments = 0;
        }

        public virtual void Add(Node node)
        {
            if (size == capacity)
                AdjustVectorCapacity();
            nodeData[size++] = node;
        }

        /// <summary> Insert the given node at the head of the list.
        /// </summary>
        /// <param name="node">The new first element.
        ///
        /// </param>
        public virtual void Prepend(Node node)
        {
            if (size == capacity)
                AdjustVectorCapacity();
            Array.Copy(nodeData, 0, nodeData, 1, size);
            size++;
            nodeData[0] = node;
        }

        private void AdjustVectorCapacity()
        {
            capacity += capacityIncrement;
            capacityIncrement *= 2;
            Node[] oldData = nodeData;
            nodeData = new AbstractNode[capacity];
            Array.Copy(oldData, 0, nodeData, 0, size);
            numberOfAdjustments++;
        }

        public int Size
        {
            get { return size; }
        }

        public Node this[int i]
        {
            get { return nodeData[i]; }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return new SimpleNodeIterator(this);
        }

        public virtual Node[] ToNodeArray()
        {
            Node[] nodeArray = new AbstractNode[size];
            Array.Copy(nodeData, 0, nodeArray, 0, size);
            return nodeArray;
        }

        public virtual string AsString()
        {
            System.Text.StringBuilder buff = new System.Text.StringBuilder();
            for (int i = 0; i < size; i++)
                buff.Append(nodeData[i].ToPlainTextString());
            return buff.ToString();
        }

        public virtual string AsHtml()
        {
            System.Text.StringBuilder buff = new System.Text.StringBuilder();
            for (int i = 0; i < size; i++)
                buff.Append(nodeData[i].ToHtml());
            return buff.ToString();
        }

        public virtual void Remove(int index)
        {
            Array.Copy(nodeData, index + 1, nodeData, index, size - index - 1);
            size--;
        }

        public virtual void RemoveAll()
        {
            size = 0;
            capacity = INITIAL_CAPACITY;
            nodeData = new AbstractNode[capacity];
            capacityIncrement = capacity*2;
            numberOfAdjustments = 0;
        }

        public override string ToString()
        {
            System.Text.StringBuilder text = new System.Text.StringBuilder();
            for (int i = 0; i < size; i++)
                text.Append(nodeData[i].ToPlainTextString());
            return text.ToString();
        }

        public virtual NodeList SearchFor(Type classType)
        {
            NodeList foundList = new NodeList();
            for (int i = 0; i < size; i++)
            {
                if (nodeData[i].GetType().FullName.Equals(classType.FullName))
                    foundList.Add(nodeData[i]);
            }
            return foundList;
        }
    }
}
