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
namespace org.htmlparser.util
{
    using System;

    public class NoFeedback : ParserFeedback
    {
        public virtual void Info(string message)
        {
        }

        public virtual void Warning(string message)
        {
        }

        public virtual void Error(string message, ParserException e)
        {
        }
    }
}
