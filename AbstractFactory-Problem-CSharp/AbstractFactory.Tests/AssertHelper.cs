/// ***************************************************************************
/// Copyright (c) 2009, Industrial Logic, Inc., All Rights Reserved.
///
/// This code is the exclusive property of Industrial Logic, Inc. It may ONLY be
/// used by students during Industrial Logic's workshops or by individuals
/// who are being coached by Industrial Logic on a project.
///
/// This code may NOT be copied or used for any other purpose without the prior
/// written consent of Industrial Logic, Inc.
/// ****s************************************************************************

using NUnit.Framework;

namespace HtmlParser
{
    static class AssertHelper
    {
        public static void FoundExpectedTagAndAttributes(string tagText, string tagName, params string[] attributes)
        {
            StringAssert.StartsWith("<" + tagName, tagText);

            Assert.AreEqual(attributes.Length + 1, tagText.Split('=').Length, "wrong # attributes");

            foreach (string attribute in attributes)
                StringAssert.Contains(attribute, tagText);

            StringAssert.EndsWith(">", tagText);
        }
    }
}
