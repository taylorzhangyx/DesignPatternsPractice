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

// contributed by Joshua Kerievsky
using System;
using StringNode = org.htmlparser.StringNode;

namespace org.htmlparser.visitors
{
    public class StringFindingVisitor : NodeVisitor
    {
        private bool stringFound = false;
        private string stringToFind;
        private int foundCount;
        private bool multipleSearchesWithinStrings;

        public StringFindingVisitor(string stringToFind)
        {
            this.stringToFind = stringToFind.ToUpper();
            foundCount = 0;
            multipleSearchesWithinStrings = false;
        }

        public virtual void DoMultipleSearchesWithinStrings()
        {
            multipleSearchesWithinStrings = true;
        }

        public override void VisitStringNode(StringNode stringNode)
        {
            string stringToBeSearched = stringNode.Text.ToUpper();
            if (!multipleSearchesWithinStrings && stringToBeSearched.IndexOf(stringToFind) != - 1)
            {
                stringFound = true;
                foundCount++;
            }
            else if (multipleSearchesWithinStrings)
            {
                int index = - 1;
                do
                {
                    index = stringToBeSearched.IndexOf(stringToFind, index + 1);
                    if (index != - 1)
                        foundCount++;
                } while (index != - 1);
            }
        }

        public virtual bool StringWasFound()
        {
            return stringFound;
        }

        public virtual int StringFoundCount()
        {
            return foundCount;
        }
    }
}
