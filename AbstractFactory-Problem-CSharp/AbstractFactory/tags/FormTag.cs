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
using CompositeTagData = org.htmlparser.tags.data.CompositeTagData;
using TagData = org.htmlparser.tags.data.TagData;
using NodeList = org.htmlparser.util.NodeList;

namespace org.htmlparser.tags
{
    /// <author>  ili
    /// *
    /// </author>
    /// <summary> Represents a FORM tag.
    /// </summary>
    public class FormTag : CompositeTag
    {
        /// <returns> NodeList of Input elements in the form
        ///
        /// </returns>
        public NodeList FormInputs
        {
            get { return formInputList; }
        }

        /// <returns> String The url of the form
        ///
        /// </returns>
        /// <summary> Set the form location. Modification of this element will cause the HTML rendering
        /// to change as well (in a call to ToHTML()).
        /// </summary>
        /// <param name="formURL">The new FORM location
        ///
        /// </param>
        public string FormLocation
        {
            get { return formURL; }
            set
            {
                attributes["ACTION"] = value;
                this.formURL = value;
            }
        }

        /// <summary> Returns the method of the form
        /// </summary>
        /// <returns> String The method of the form (GET if nothing is specified)
        ///
        /// </returns>
        public string FormMethod
        {
            get
            {
                if (formMethod == null)
                {
                    formMethod = "GET";
                }
                return formMethod;
            }
        }

        /// <returns> String The name of the form
        ///
        /// </returns>
        public string FormName
        {
            get { return formName; }
        }

        public const string POST = "POST";
        public const string GET = "GET";
        protected string formURL;
        protected string formName;
        protected string formMethod;
        protected NodeList formInputList;

        /// <summary> Constructor takes in tagData, compositeTagData
        /// </summary>
        /// <param name="">tagData
        /// </param>
        /// <param name="">compositeTagData
        ///
        /// </param>
        public FormTag(TagData tagData, CompositeTagData compositeTagData) : base(tagData, compositeTagData)
        {
            this.formURL = compositeTagData.StartTag["ACTION"];
            this.formName = compositeTagData.StartTag["NAME"];
            this.formMethod = compositeTagData.StartTag["METHOD"];
            this.formInputList = compositeTagData.Children.SearchFor(typeof (InputTag));
        }

        /// <summary> Get the input tag in the form corresponding to the given name
        /// </summary>
        /// <param name="name">The name of the input tag to be retrieved
        /// </param>
        /// <returns> Tag The input tag corresponding to the name provided
        ///
        /// </returns>
        public virtual InputTag GetInputTag(string name)
        {
            foreach (InputTag inputTag in formInputList)
            {
                string inputTagName = inputTag["NAME"];
                if (inputTagName != null && inputTagName.ToUpper().Equals(name.ToUpper()))
                    return inputTag;
            }

            return null;
        }

        /// <returns> String The contents of the FormTag
        ///
        /// </returns>
        public override string ToString()
        {
            return "FORM TAG : Form at " + formURL + "; begins at : " + ElementBegin + "; ends at : " + ElementEnd;
        }
    }
}
