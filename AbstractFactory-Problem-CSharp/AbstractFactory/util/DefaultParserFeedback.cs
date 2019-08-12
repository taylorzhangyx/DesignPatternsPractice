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

namespace org.htmlparser.util
{
    /// <summary> Default implementation of the ParserFeedback interface.
    /// This implementation prints output to the console but users
    /// can implement their own classes to support alternate behavior.
    /// *
    /// </summary>
    /// <author>  Claude Duguay
    /// </author>
    /// <seealso cref="">ParserFeedback
    /// </seealso>
    /// <seealso cref="">FeedbackManager
    /// *
    /// </seealso>
    public class DefaultParserFeedback : ParserFeedback
    {
        /// <summary> Constructor argument for a quiet feedback.
        /// </summary>
        public const int QUIET = 0;

        /// <summary> Constructor argument for a normal feedback.
        /// </summary>
        public const int NORMAL = 1;

        /// <summary> Constructor argument for a debugging feedback.
        /// </summary>
        public const int DEBUG = 2;

        /// <summary> Verbosity level.
        /// Corresponds to constructor arguments:
        /// <pre>
        /// DEBUG = 2;
        /// NORMAL = 1;
        /// QUIET = 0;
        /// </pre>
        /// </summary>
        protected int mode;

        /// <summary> Construct a feedback object of the given type.
        /// </summary>
        /// <param name="mode">The type of feedback:
        /// <pre>
        /// DEBUG - verbose debugging with stack traces
        /// NORMAL - normal messages
        /// QUIET - no messages
        /// </pre>
        /// 
        /// </param>
        public DefaultParserFeedback(int mode)
        {
            if (mode < QUIET || mode > DEBUG)
                throw new System.ArgumentException("illegal mode (" + mode + "), must be one of: QUIET, NORMAL, DEBUG");
            this.mode = mode;
        }

        /// <summary> Construct a NORMAL feedback object.
        /// </summary>
        public DefaultParserFeedback() : this(NORMAL)
        {
        }

        /// <summary> Print an info message.
        /// </summary>
        /// <param name="message">The message to print.
        /// 
        /// </param>
        public virtual void Info(string message)
        {
            if (mode != QUIET)
                System.Console.Out.WriteLine("INFO: " + message);
        }

        /// <summary> Print an warning message.
        /// </summary>
        /// <param name="message">The message to print.
        /// 
        /// </param>
        public virtual void Warning(string message)
        {
            if (mode != QUIET)
                System.Console.Out.WriteLine("WARNING: " + message);
        }

        /// <summary> Print an error message.
        /// </summary>
        /// <param name="message">The message to print.
        /// </param>
        /// <param name="exception">The exception for stack tracing.
        /// 
        /// </param>
        public virtual void Error(string message, ParserException exception)
        {
            if (mode != QUIET)
            {
                System.Console.Out.WriteLine("ERROR: " + message);
                if (mode == DEBUG && (exception != null))
                    SupportClass.WriteStackTrace(exception, Console.Error);
            }
        }
    }
}
