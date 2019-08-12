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

using System;
using System.Net;
using LinkProcessor = org.htmlparser.util.LinkProcessor;
using ParserException = org.htmlparser.util.ParserException;
using ParserFeedback = org.htmlparser.util.ParserFeedback;

namespace org.htmlparser.parserHelper
{
    public class ParserHelper
    {
        /// <summary> Opens a connection using the given url.
        /// </summary>
        /// <param name="url">The url to open.
        /// </param>
        /// <param name="feedback">The object to use for messages or <code>null</code>.
        /// </param>
        /// <exception cref="">ParserException if an I/O exception occurs accessing the url.
        ///
        /// </exception>
        public static WebRequest OpenConnection(System.Uri url, ParserFeedback feedback)
        {
            WebRequest ret;

            try
            {
                ret = (WebRequest) WebRequest.Create(url);
            }
            catch (System.IO.IOException ioe)
            {
                string msg = "HTMLParser.OpenConnection() : Error in opening a connection to " + url.ToString();
                ParserException ex = new ParserException(msg, ioe);
                if (null != feedback)
                    feedback.Error(msg, ex);
                throw ex;
            }

            return (ret);
        }

        /// <summary> Opens a connection based on a given string.
        /// The string is either a file, in which case <code>file://localhost</code>
        /// is prepended to a canonical path derived from the string, or a url that
        /// begins with one of the known protocol strings, i.e. <code>http://</code>.
        /// Embedded spaces are silently converted to %20 sequences.
        /// </summary>
        /// <param name="resource">The name of a file or a url.
        /// </param>
        /// <param name="feedback">The object to use for messages or <code>null</code> for no feedback.
        /// </param>
        /// <exception cref=""> ParserException if the string is not a valid url or file.
        ///
        /// </exception>
        public static WebRequest OpenConnection(string resource, ParserFeedback feedback)
        {
            System.Uri url;
            WebRequest ret;

            try
            {
                url = SupportClass.CreateUri(LinkProcessor.FixSpaces(resource));
                ret = ParserHelper.OpenConnection(url, feedback);
            }
            catch (System.UriFormatException ufe)
            {
                string msg = "HTMLParser.OpenConnection() : Error in opening a connection to " + resource;
                ParserException ex = new ParserException(msg, ufe);
                if (null != feedback)
                    feedback.Error(msg, ex);
                throw ex;
            }

            return (ret);
        }

        /// <summary> Lookup a character set name.
        /// <em>Vacuous for JVM's without <code>java.nio.charset</code>.</em>
        /// This uses reflection so the code will still run under prior JDK's but
        /// in that case the default is always returned.
        /// </summary>
        /// <param name="name">The name to look up. One of the aliases for a character set.
        /// </param>
        /// <param name="_default">The name to return if the lookup fails.
        ///
        /// </param>
        public static string FindCharset(string name, string _default)
        {
            return _default;
/*			string ret;

			//UPGRADE_NOTE: Exception 'java.lang.ClassNotFoundException' was converted to 'System.Exception' which has different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1100"'
			try
			{
				System.Type cls;
				System.Reflection.MethodInfo method;
				System.Object object;

				//UPGRADE_TODO: Format of parameters of method 'java.lang.Class.forName' are different in the equivalent in .NET. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1092"'
				cls = System.Type.GetType("java.nio.charset.Charset");
				method = cls.GetMethod("forName", (System.Type[]) new System.Type[]{typeof(string)});
				object = method.Invoke(null, (System.Object[]) new System.Object[]{name});
				method = cls.GetMethod("name", (System.Type[]) new System.Type[]{});
				object = method.Invoke(object, (System.Object[]) new System.Object[]{});
				ret = (string) object;
			}
			catch (System.Exception cnfe)
			{
				// for reflection exceptions, assume the name is correct
				ret = name;
			}
			catch (System.MethodAccessException nsme)
			{
				// for reflection exceptions, assume the name is correct
				ret = name;
			}
			catch (System.UnauthorizedAccessException ia)
			{
				// for reflection exceptions, assume the name is correct
				ret = name;
			}
			catch (System.Reflection.TargetInvocationException ita)
			{
				// java.nio.charset.IllegalCharsetNameException
				// and java.nio.charset.UnsupportedCharsetException
				// return the default
				ret = _default;
			}

			return (ret);
*/
        }
    }
}
