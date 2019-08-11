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
using NUnit.Framework;

namespace Industriallogic.FactoryMethod
{
    [TestFixture]
    public class XMLBuilderTest : AbstractBuilderTest
    {
        protected override OutputBuilder createOutputBuilder(String rootName)
        {
            return new XMLBuilder(rootName);
        }
    }
}
