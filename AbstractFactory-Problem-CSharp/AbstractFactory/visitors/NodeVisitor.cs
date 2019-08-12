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
using RemarkNode = org.htmlparser.RemarkNode;
using StringNode = org.htmlparser.StringNode;
using EndTag = org.htmlparser.tags.EndTag;
using ImageTag = org.htmlparser.tags.ImageTag;
using LinkTag = org.htmlparser.tags.LinkTag;
using Tag = org.htmlparser.tags.Tag;
using TitleTag = org.htmlparser.tags.TitleTag;

namespace org.htmlparser.visitors
{
    public abstract class NodeVisitor
    {
        private bool recurseChildren;
        private bool recurseSelf;

        public NodeVisitor() : this(true)
        {
        }

        public NodeVisitor(bool recurseChildren)
        {
            this.recurseChildren = recurseChildren;
            this.recurseSelf = true;
        }

        public NodeVisitor(bool recurseChildren, bool recurseSelf)
        {
            this.recurseChildren = recurseChildren;
            this.recurseSelf = recurseSelf;
        }

        public virtual void VisitTag(Tag tag)
        {
        }

        public virtual void VisitStringNode(StringNode stringNode)
        {
        }

        public virtual void VisitLinkTag(LinkTag linkTag)
        {
        }

        public virtual void VisitImageTag(ImageTag imageTag)
        {
        }

        public virtual void VisitEndTag(EndTag endTag)
        {
        }

        public virtual void VisitTitleTag(TitleTag titleTag)
        {
        }

        public virtual void VisitRemarkNode(RemarkNode remarkNode)
        {
        }

        public virtual bool ShouldRecurseChildren()
        {
            return recurseChildren;
        }

        public virtual bool ShouldRecurseSelf()
        {
            return recurseSelf;
        }

        /// <summary> Override this method if you wish to do special
        /// processing upon completion of parsing
        /// </summary>
        public virtual void FinishedParsing()
        {
        }
    }
}
