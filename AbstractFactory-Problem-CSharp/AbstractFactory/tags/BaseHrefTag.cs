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
using TagData = org.htmlparser.tags.data.TagData;

namespace org.htmlparser.tags
{
    /// <author>  Somik Raha
    /// *
    /// </author>
    public class BaseHrefTag : Tag
    {
        public string BaseUrl
        {
            get { return baseUrl; }

            set { this.baseUrl = value; }
        }

        private string baseUrl;

        public BaseHrefTag(TagData tagData, string baseUrl) : base(tagData)
        {
            this.baseUrl = baseUrl;
        }

        public override string ToString()
        {
            return "BASE TAG\n" + "--------\n" + "Name : " + baseUrl;
        }
    }
}
