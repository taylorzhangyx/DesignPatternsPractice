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
using System.Collections;
using System.IO;
using System.Net;
using org.htmlparser.parserHelper;
using BodyScanner = org.htmlparser.scanners.BodyScanner;
using BulletListScanner = org.htmlparser.scanners.BulletListScanner;
using DivScanner = org.htmlparser.scanners.DivScanner;
using FormScanner = org.htmlparser.scanners.FormScanner;
using HeadScanner = org.htmlparser.scanners.HeadScanner;
using HtmlScanner = org.htmlparser.scanners.HtmlScanner;
using LinkScanner = org.htmlparser.scanners.LinkScanner;
using MetaTagScanner = org.htmlparser.scanners.MetaTagScanner;
using TagScanner = org.htmlparser.scanners.TagScanner;
using TitleScanner = org.htmlparser.scanners.TitleScanner;
using EndTag = org.htmlparser.tags.EndTag;
using ImageTag = org.htmlparser.tags.ImageTag;
using LinkTag = org.htmlparser.tags.LinkTag;
using MetaTag = org.htmlparser.tags.MetaTag;
using Tag = org.htmlparser.tags.Tag;
using org.htmlparser.util;
using DefaultParserFeedback = org.htmlparser.util.DefaultParserFeedback;
using NodeIterator = org.htmlparser.util.NodeIterator;
using NodeList = org.htmlparser.util.NodeList;
using ParserException = org.htmlparser.util.ParserException;
using ParserFeedback = org.htmlparser.util.ParserFeedback;
using NodeVisitor = org.htmlparser.visitors.NodeVisitor;

namespace org.htmlparser
{
    /// <summary> This is the class that the user will use, either to get an iterator into
    /// the html page or to directly parse the page and print the results
    /// <BR>
    /// Typical usage of the parser is as follows : <BR>
    /// [1] Create a parser object - passing the URL and a feedback object to the parser<BR>
    /// [2] Register the common scanners. See {@link #RegisterScanners()} <BR>
    /// You wouldn't do this if you wanted to configure a custom lightweight parser. In that case,
    /// you would add the scanners of your choice using {@link #AddScanner(TagScanner)}<BR>
    /// [3] Enumerate through the elements from the parser object <BR>
    /// It is important to note that the parsing occurs when you enumerate, ON DEMAND. This is a thread-safe way,
    /// and you only get control back after a particular element is parsed and returned.
    /// *
    /// <BR>
    /// Below is some sample code to parse Yahoo.com and print all the tags.
    /// <pre>
    /// Parser parser = new Parser("http://www.yahoo.com",new DefaultParserFeedback());
    /// // In this example, we are registering all the common scanners
    /// parser.RegisterScanners();
    /// foreach (Node node in parser) {
    ///     node.Print();
    /// }
    /// </pre> Below is some sample code to parse Yahoo.com and print only the text
    /// information. This scanning will run faster, as there are no scanners
    /// registered here.
    /// <pre>
    /// Parser parser = new Parser("http://www.yahoo.com",new DefaultParserFeedback());
    /// // In this example, none of the scanners need to be registered
    /// // as a string node is not a tag to be scanned for.
    /// foreach (Node node in parser) {
    ///     if (node instanceof StringNode) {
    ///         StringNode stringNode = (StringNode)node;
    ///         System.out.println(stringNode.Text);
    ///     }
    /// }
    /// </pre>
    /// The above snippet will print out only the text contents in the html document.<br>
    /// Here's another snippet that will only print out the link urls in a document.
    /// This is an example of adding a link scanner.
    /// <pre>
    /// Parser parser = new Parser("http://www.yahoo.com",new DefaultParserFeedback());
    /// parser.AddScanner(new LinkScanner("-l"));
    /// foreach (Node node in parser) {
    ///     if (node instanceof LinkTag) {
    ///         LinkTag linkTag = (LinkTag)node;
    ///         System.out.println(linkTag.Link);
    ///     }
    /// }
    /// </pre>
    /// </summary>
    public class Parser
    {
        private void InitBlock()
        {
            parserHelper = new ParserHelper();
        }

        /// <returns> String lineSeparator that will be used in ToHTML()
        ///
        /// </returns>
        /// <param name="lineSeparator">New Line separator to be used
        ///
        /// </param>
        public static string LineSeparator
        {
            get { return lineSeparator; }

            set { lineSeparator = value; }
        }

        /// <summary> Return the version string of this parser.
        /// </summary>
        /// <returns> A string of the form:
        /// <pre>
        /// "[floating point number] ([build-type] [build-date])"
        /// </pre>
        ///
        /// </returns>
        public static string Version
        {
            get { return (VERSION_STRING); }
        }

        /// <summary> Return the version number of this parser.
        /// </summary>
        /// <returns> A floating point number, the whole number part is the major
        /// version, and the fractional part is the minor version.
        ///
        /// </returns>
        public static double VersionNumber
        {
            get { return (VERSION_NUMBER); }
        }

        /// <summary> Handles the current connection.
        /// </summary>
        /// <returns> The connection is either created by the parser or passed into this
        /// parser by setting the <code>Connection</code> property.
        /// </returns>
        ///
        /// </seealso>
        /// <summary> Set the connection for this parser.
        /// This method sets four of the fields in the parser object;
        /// <code>resourceLocn</code>, <code>url</code>, <code>character_set</code>
        /// and <code>reader</code>. It does not adjust the <code>scanners</code> list
        /// or <code>feedback</code> object. The four fields are set atomically by
        /// this method, either they are all set or none of them is set. Trying to
        /// set the connection to null is a noop.
        /// </summary>
        /// <param name="connection">A fully conditioned connection. The connect()
        /// method will be called so it need not be connected yet.
        /// </param>
        /// <exception cref=""> ParserException if the character set specified in the
        /// HTTP header is not supported, or an i/o exception occurs creating the
        /// reader.
        ///
        /// </exception>
        public WebRequest Connection
        {
            get { return (url); }

            set
            {
                string res;
                NodeReader rd;
                string chs;
                WebRequest con;

                if (null != value)
                {
                    res = URL;
                    rd = Reader;
                    chs = Encoding;
                    con = Connection;
                    try
                    {
                        resourceLocn = value.RequestUri.ToString();
                        url = value;
                        character_set = getCharacterSet(url);
                        CreateReader();
                    }
                    catch (WebException we)
                    {
                        string msg = "Error in opening a connection to " + value.RequestUri.ToString();
                        ParserException ex = new ParserException(msg, we);
                        feedback.Error(msg, ex);
                        resourceLocn = res;
                        url = con;
                        character_set = chs;
                        reader = rd;
                        throw ex;
                    }
                }
            }
        }

        /// <summary> Return the URL currently being parsed.
        /// </summary>
        /// <returns> The url passed into the constructor or the file name
        /// passed to the constructor modified to be a URL.
        ///
        /// </returns>
        /// <summary> Set the URL for this parser.
        /// This method sets four of the fields in the parser object;
        /// <code>resourceLocn</code>, <code>url</code>, <code>character_set</code>
        /// and <code>reader</code>. It does not adjust the <code>scanners</code> list
        /// or <code>feedback</code> object.Trying to set the url to null or an
        /// empty string is a noop.
        /// </summary>
        /// <seealso cref="">#Connection
        ///
        /// </seealso>
        public string URL
        {
            get { return (resourceLocn); }

            set
            {
                if ((null != value) && value.Length > 0)
                    Connection = ParserHelper.OpenConnection(value, Feedback);
            }
        }

        /// <summary> The current encoding.
        /// This item is et from the HTTP header but may be overridden by meta
        /// tags in the head, so this may change after the head has been parsed.
        /// </summary>
        /// <summary> Set the encoding for this parser.
        /// If there is no connection (getConnection() returns null) it simply sets
        /// the character set name stored in the parser (Note: the reader object
        /// which must have been set in the constructor or by <code>setReader()</code>,
        /// may or may not be using this character set).
        /// Otherwise (getConnection() doesn't return null) it does this by reopening the
        /// input stream of the connection and creating a reader that uses this
        /// character set. In this case, this method sets two of the fields in the
        /// parser object; <code>character_set</code> and <code>reader</code>.
        /// It does not adjust <code>resourceLocn</code>, <code>url</code>,
        /// <code>scanners</code> or <code>feedback</code>. The two fields are set
        /// atomicly by this method, either they are both set or none of them is set.
        /// Trying to set the encoding to null or an empty string is a noop.
        /// </summary>
        /// <exception cref=""> ParserException If the opening of the reader
        ///
        /// </exception>
        public string Encoding
        {
            get { return (character_set); }

            set
            {
                string chs;
                NodeReader rd;
                BufferedStream inStream;

                if ((null != value) && value.Length > 0)
                {
                    if (null == Connection)
                        character_set = value;
                    else
                    {
                        rd = Reader;
                        chs = Encoding;
                        inStream = input;
                        try
                        {
                            character_set = value;
                            RecreateReader();
                        }
                        catch (IOException ioe)
                        {
                            string msg = "setEncoding() : Error in opening a connection to " +
                                         Connection.RequestUri.ToString();
                            ParserException ex = new ParserException(msg, ioe);
                            feedback.Error(msg, ex);
                            character_set = chs;
                            reader = rd;
                            input = inStream;
                            throw ex;
                        }
                    }
                }
            }
        }

        /// <summary> Returns the reader associated with the parser
        /// </summary>
        /// <returns> NodeReader
        ///
        /// </returns>
        /// <summary> Set the reader for this parser.
        /// This method sets four of the fields in the parser object;
        /// <code>resourceLocn</code>, <code>url</code>, <code>character_set</code>
        /// and <code>reader</code>. It does not adjust the <code>scanners</code> list
        /// or <code>feedback</code> object. The <code>url</code> is set to
        /// null since this cannot be determined from the reader. The
        /// <code>character_set</code> is set to the default character set since
        /// this cannot be determined from the reader.
        /// Trying to set the reader to <code>null</code> is a noop.
        /// </summary>
        /// <param name="rd">The reader object to use. This reader will be bound to this
        /// parser after this call.
        ///
        /// </param>
        public NodeReader Reader
        {
            get { return reader; }

            set
            {
                if (null != value)
                {
                    resourceLocn = value.URL;
                    reader = value;
                    character_set = DEFAULT_CHARSET;
                    url = null;
                    reader.Parser = this;
                }
            }
        }

        /// <summary> Get the number of scanners registered currently in the scanner.
        /// </summary>
        /// <returns> int number of scanners registered
        ///
        /// </returns>
        public int NumScanners
        {
            get { return scanners.Count; }
        }

        /// <summary> Get an enumeration of scanners registered currently in the parser
        /// </summary>
        /// <returns> Enumeration of scanners currently registered in the parser
        ///
        /// </returns>
        /// <summary> This method is to be used to change the set of scanners in the current parser.
        /// </summary>
        /// <param name="newScanners">Vector holding scanner objects to be used during the parsing process.
        ///
        /// </param>
        public IDictionary Scanners
        {
            get { return scanners; }
            set { scanners = (null == value) ? new Hashtable() : value; }
        }

        /// <summary> Returns the feedback.
        /// </summary>
        /// <returns> ParserFeedback
        ///
        /// </returns>
        /// <summary> Sets the feedback object used in scanning.
        /// </summary>
        /// <param name="fb">The new feedback object to use.
        ///
        /// </param>
        public ParserFeedback Feedback
        {
            get { return feedback; }

            set { feedback = (null == value) ? noFeedback : value; }
        }

        /// <summary> Tells the parser to decode nodes using org.htmlparser.util.Translate.decode()
        /// </summary>
        public bool ShouldDecodeNodes
        {
            get { return shouldDecodeNodes; }
            set { this.shouldDecodeNodes = value; }
        }

        // Please don't change the formatting of the version variables below.
        // This is done so as to facilitate ant script processing.

        /// <summary> The floating point version number.
        /// </summary>
        public const double VERSION_NUMBER = 1.4;

        /// <summary> The type of version.
        /// </summary>
        public const string VERSION_TYPE = "Integration Build";

        /// <summary> The date of the version.
        /// </summary>
        public const string VERSION_DATE = "Jun 01, 2003";

        /// <summary> The display version.
        /// </summary>
        public static readonly string VERSION_STRING = "" + VERSION_NUMBER + " (" + VERSION_TYPE + " " + VERSION_DATE +
                                                       ")";

        // End of formatting

        /// <summary> Flag to tell the parser to decode nodes while parsing.
        /// Decoding occurs via the method, org.htmlparser.util.Translate.decode()
        /// </summary>
        private bool shouldDecodeNodes = false;

        /// <summary> The default charset.
        /// This should be <code>ISO-8859-1</code>,
        /// see RFC 2616 (http://www.ietf.org/rfc/rfc2616.txt?number=2616) section 3.7.1
        /// Another alias is "8859_1".
        /// </summary>
        protected const string DEFAULT_CHARSET = "ISO-8859-1";

        /// <summary>  Trigger for charset detection.
        /// </summary>
        protected const string CHARSET_STRING = "charset";

        /// <summary> Feedback object.
        /// </summary>
        protected ParserFeedback feedback;

        /// <summary> The URL or filename to be parsed.
        /// </summary>
        protected string resourceLocn;

        /// <summary> The html reader associated with this parser.
        /// </summary>
        protected NodeReader reader;

        /// <summary> The list of scanners to apply at the top level.
        /// </summary>
        private IDictionary scanners;

        /// <summary> The encoding being used to decode the connection input stream.
        /// </summary>
        protected string character_set;

        /// <summary> The source for HTML.
        /// </summary>
        protected WebRequest url;

        /// <summary> The bytes extracted from the source.
        /// </summary>
        protected BufferedStream input;

        /// <summary> Variable to store lineSeparator.
        /// This is setup to read <code>line.separator</code> from the System property.
        /// However it can also be changed using the mutator methods.
        /// This will be used in the toHTML() methods in all the sub-classes of Node.
        /// </summary>
        protected static string lineSeparator;

        /// <summary> A quiet message sink.
        /// Use this for no feedback.
        /// </summary>
        public static ParserFeedback noFeedback;

        /// <summary> A verbose message sink.
        /// Use this for output on <code>System.out</code>.
        /// </summary>
        public static ParserFeedback stdout;

        private ParserHelper parserHelper;
        private long markedPosition;

        //
        // Constructors
        //

        /// <summary> Zero argument constructor.
        /// The parser is in a safe but useless state.
        /// Set the reader or connection using Reader or Connection.
        /// </summary>
        /// <seealso cref="">#Reader
        /// </seealso>
        /// <seealso cref="">#Connection
        ///
        /// </seealso>
        public Parser()
        {
            InitBlock();
            Feedback = null;
            Scanners = null;
            resourceLocn = null;
            reader = null;
            character_set = DEFAULT_CHARSET;
            url = null;
            input = null;
            Tag.TagParser = new TagParser(Feedback);
        }

        /// <summary> This constructor enables the construction of test cases, with readers
        /// associated with test string buffers. It can also be used with readers of the user's choice
        /// streaming data into the parser.<p/>
        /// <B>Important:</B> If you are using this constructor, and you would like to use the parser
        /// to parse multiple times (multiple calls to parser.GetEnumerator()), you must ensure the following:<br>
        /// <ul>
        /// <li>Before the first parse, you must mark the reader for a length that you anticipate (the size of the stream).</li>
        /// <li>After the first parse, calls to GetEnumerator() must be preceded by calls to:
        /// <pre>
        /// parser.getReader().Reset();
        /// </pre>
        /// </li>
        /// </ul>
        /// </summary>
        /// <param name="rd">The reader to draw characters from.
        /// </param>
        /// <param name="fb">The object to use when information,
        /// warning and error messages are produced. If <em>null</em> no feedback
        /// is provided.
        ///
        /// </param>
        public Parser(NodeReader rd, ParserFeedback fb)
        {
            InitBlock();
            Feedback = fb;
            Scanners = null;
            resourceLocn = null;
            reader = null;
            character_set = DEFAULT_CHARSET;
            url = null;
            input = null;
            Reader = rd;
            Tag.TagParser = new TagParser(feedback);
        }

        /// <summary> Constructor for custom HTTP access.
        /// </summary>
        /// <param name="connection">A fully conditioned connection.
        /// </param>
        /// <param name="fb">The object to use for message communication.
        ///
        /// </param>
        public Parser(WebRequest connection, ParserFeedback fb)
        {
            InitBlock();
            Feedback = fb;
            Scanners = null;
            resourceLocn = null;
            reader = null;
            character_set = DEFAULT_CHARSET;
            url = null;
            input = null;
            Tag.TagParser = new TagParser(feedback);
            Connection = connection;
        }

        /// <summary> Creates a Parser object with the location of the resource (URL or file)
        /// You would typically create a DefaultParserFeedback object and pass it in.
        /// </summary>
        /// <param name="resourceLocn">Either the URL or the filename (autodetects).
        /// A standard HTTP GET is performed to read the content of the URL.
        /// </param>
        /// <param name="feedback">The ParserFeedback object to use when information,
        /// warning and error messages are produced. If <em>null</em> no feedback
        /// is provided.
        /// </param>
        /// <seealso cref="">#Parser(WebRequest, ParserFeedback)
        ///
        /// </seealso>
        public Parser(string resourceLocn, ParserFeedback feedback)
            : this(ParserHelper.OpenConnection(resourceLocn, feedback), feedback)
        {
        }

        /// <summary> Creates a Parser object with the location of the resource (URL or file).
        /// A DefaultParserFeedback object is used for feedback.
        /// </summary>
        /// <param name="resourceLocn">Either the URL or the filename (autodetects).
        ///
        /// </param>
        public Parser(string resourceLocn) : this(resourceLocn, stdout)
        {
        }

        /// <summary> This constructor is present to enable users to plug in their own readers.
        /// A DefaultParserFeedback object is used for feedback. It can also be used with readers of the user's choice
        /// streaming data into the parser.<p/>
        /// <B>Important:</B> If you are using this constructor, and you would like to use the parser
        /// to parse multiple times (multiple calls to parser.GetEnumerator()), you must ensure the following:<br>
        /// <ul>
        /// <li>Before the first parse, you must mark the reader for a length that you anticipate (the size of the stream).</li>
        /// <li>After the first parse, calls to GetEnumerator() must be preceded by calls to:
        /// <pre>
        /// parser.getReader().Reset();
        /// </pre>
        /// </li>
        /// </summary>
        /// <param name="reader">The source for HTML to be parsed.
        ///
        /// </param>
        public Parser(NodeReader reader) : this(reader, stdout)
        {
        }

        /// <summary> Constructor for non-standard access.
        /// A DefaultParserFeedback object is used for feedback.
        /// </summary>
        /// <param name="connection">A fully conditioned connection.
        /// <seealso cref="">#Parser(WebRequest, ParserFeedback)
        ///
        /// </seealso>
        public Parser(WebRequest connection) : this(connection, stdout)
        {
        }

        //
        // Internal methods
        //

        /// <summary> Open a stream reader on the <code>InputStream</code>.
        /// Revise the character set to it's default value if an
        /// <code>UnsupportedEncodingException</code> is thrown.
        /// </summary>
        /// <exception cref=""> UnsupportedEncodingException in the unlikely event that
        /// the default character set is not supported on this platform.
        ///
        /// </exception>
        protected virtual StreamReader CreateInputStreamReader()
        {
            StreamReader ret;

            try
            {
                //UPGRADE_TODO: Constructor 'java.io.InputStreamReader.InputStreamReader' was converted to 'StreamReader' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
                ret = new StreamReader(input, System.Text.Encoding.GetEncoding(character_set));
            }
            catch (IOException)
            {
                System.Text.StringBuilder msg;
                string message;

                msg = new System.Text.StringBuilder(1024);
                msg.Append(url.RequestUri.ToString());
                msg.Append(" has an encoding (");
                msg.Append(character_set);
                msg.Append(") which is not supported, using ");
                msg.Append(DEFAULT_CHARSET);
                message = msg.ToString();
                feedback.Warning(message);
                character_set = DEFAULT_CHARSET;
                //UPGRADE_TODO: Constructor 'java.io.InputStreamReader.InputStreamReader' was converted to 'StreamReader' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
                ret = new StreamReader(input, System.Text.Encoding.GetEncoding(character_set));
            }

            return (ret);
        }

        /// <summary> Create a new reader for the URLConnection object.
        /// The current character set is used to transform the input stream
        /// into a character reader.
        /// </summary>
        /// <exception cref=""> IOException if there is a problem constructing the reader.
        /// </exception>
        /// <seealso cref="">#CreateInputStreamReader()
        /// </seealso>
        /// <seealso cref="">#getEncoding()
        ///
        /// </seealso>
        protected virtual void CreateReader()
        {
            Stream stream;
            StreamReader inReader;

            stream = url.GetResponse().GetResponseStream();
            input = new BufferedStream(stream);
            if (stream.CanSeek)
            {
                markedPosition = input.Position;
            }
            inReader = CreateInputStreamReader();
            reader = new NodeReader(inReader, resourceLocn);
            reader.Parser = this;
        }

        /// <summary> Create a new reader for the URL object but reuse the input stream.
        /// The current character set is used to transform the input stream
        /// into a character reader. Defaults to <code>CreateReader()</code> if
        /// there is no existing input stream.
        /// </summary>
        /// <exception cref=""> IOException if there is a problem constructing the reader.
        /// </exception>
        /// <seealso cref="">#CreateReader()
        /// </seealso>
        /// <seealso cref="">#CreateInputStreamReader()
        /// </seealso>
        /// <seealso cref="">#getEncoding()
        ///
        /// </seealso>
        protected virtual void RecreateReader()
        {
            if (null == input)
                CreateReader();
            else
            {
                StreamReader inReader;
                if (input.CanSeek)
                {
                    input.Position = markedPosition;
                    markedPosition = input.Position;
                }
                inReader = CreateInputStreamReader();
                reader = new NodeReader(inReader, resourceLocn);
                reader.Parser = this;
            }
        }

        /// <summary> Try and extract the character set from the HTTP header.
        /// </summary>
        /// <param name="connection">The connection with the charset info.
        /// </param>
        /// <returns> The character set name to use for this HTML page.
        ///
        /// </returns>
        protected virtual string getCharacterSet(WebRequest connection)
        {
            string field = "Content-Type";

            string contentType;
            string ret;

            ret = DEFAULT_CHARSET;
            contentType = connection.GetResponse().Headers.Get(field);
            if (null != contentType)
                ret = getCharset(contentType);

            return (ret);
        }

        /// <summary> Get a CharacterSet name corresponding to a charset parameter.
        /// </summary>
        /// <param name="content">A text line of the form:
        /// <pre>
        /// text/html; charset=Shift_JIS
        /// </pre>
        /// which is applicable both to the HTTP header field Content-Type and
        /// the meta tag http-equiv="Content-Type".
        /// Note this method also handles non-compliant quoted charset directives such as:
        /// <pre>
        /// text/html; charset="UTF-8"
        /// </pre>
        /// and
        /// <pre>
        /// text/html; charset='UTF-8'
        /// </pre>
        /// </param>
        /// <returns> The character set name to use when reading the input stream.
        /// For JDKs that have the Charset class this is qualified by passing
        /// the name to FindCharset() to render it into canonical form.
        /// If the charset parameter is not found in the given string, the default
        /// character set is returned.
        /// </returns>
        /// <seealso cref="">ParserHelper#FindCharset
        /// </seealso>
        /// <seealso cref="">#DEFAULT_CHARSET
        ///
        /// </seealso>
        protected virtual string getCharset(string content)
        {
            int index;
            string ret;

            ret = DEFAULT_CHARSET;
            if (null != content)
            {
                index = content.IndexOf(CHARSET_STRING);

                if (index != - 1)
                {
                    content = content.Substring(index + CHARSET_STRING.Length).Trim();
                    if (content.StartsWith("="))
                    {
                        content = content.Substring(1).Trim();
                        index = content.IndexOf(";");
                        if (index != - 1)
                            content = content.Substring(0, (index) - (0));

                        //remove any double quotes from around charset string
                        if (content.StartsWith("\"") && content.EndsWith("\"") && (1 < content.Length))
                            content = content.Substring(1, (content.Length - 1) - (1));

                        //remove any single quote from around charset string
                        if (content.StartsWith("'") && content.EndsWith("'") && (1 < content.Length))
                            content = content.Substring(1, (content.Length - 1) - (1));

                        ret = ParserHelper.FindCharset(content, ret);
                        // Charset names are not case-sensitive;
                        // that is, case is always ignored when comparing charset names.
                        if (!ret.ToUpper().Equals(content.ToUpper()))
                        {
                            feedback.Info("detected charset \"" + content + "\", using \"" + ret + "\"");
                        }
                    }
                }
            }

            return (ret);
        }

        //
        // Public methods
        //

        /// <summary> Add a new Tag Scanner.
        /// In typical situations where you require a no-frills parser, use the RegisterScanners() method to add the most
        /// common parsers. But when you wish to either compose a parser with only certain scanners registered, use this method.
        /// It is advantageous to register only the scanners you want, in order to achieve faster parsing speed. This method
        /// would also be of use when you have developed custom scanners, and need to register them into the parser.
        /// </summary>
        /// <param name="scanner">TagScanner object (or derivative) to be added to the list of registered scanners
        ///
        /// </param>
        public virtual void AddScanner(TagScanner scanner)
        {
            foreach (string id in scanner.ID)
            {
                scanners[id] = scanner;
            }
            scanner.Feedback = feedback;
        }

        /// <summary> Returns an iterator (enumeration) to the html nodes. Each node can be a tag/endtag/
        /// string/link/image<br>
        /// This is perhaps the most important method of this class. In typical situations, you will need to use
        /// the parser like this :
        /// <pre>
        /// Parser parser = new Parser("http://www.yahoo.com");
        /// parser.RegisterScanners();
        /// foreach (Node node in parser) {
        ///     if (node instanceof StringNode) {
        ///         // Downcasting to StringNode
        ///         StringNode stringNode = (StringNode)node;
        ///         // Do whatever processing you want with the string node
        ///         System.out.println(stringNode.getText());
        ///     }
        ///     // Check for the node or tag that you want
        ///     if (node instanceof ...) {
        ///         // Downcast, and process
        ///     }
        /// }
        /// </pre>
        /// </summary>
        public NodeIterator GetEnumerator()
        {
            bool remove_scanner;
            NodeIterator ret;

            remove_scanner = false;
            ret = new NodeIterator(reader, resourceLocn, feedback);
            ret = CreateNodeIterator(remove_scanner, ret);

            return (ret);
        }

        public NodeIterator CreateNodeIterator(bool remove_scanner, NodeIterator ret)
        {
            Node node;
            MetaTag meta;
            string httpEquiv;
            string charset;
            EndTag end;
            if (null != url)
                try
                {
                    if (null == scanners["-m"])
                    {
                        AddScanner(new MetaTagScanner("-m"));
                        remove_scanner = true;
                    }

                    /* Read up to </HEAD> looking for charset directive */
                    while (ret.MoveNext())
                    {
                        node = (Node) ret.Current;
                        if (node is MetaTag)
                        {
                            // check for charset on Content-Type
                            meta = (MetaTag) node;
                            httpEquiv = meta["HTTP-EQUIV"];
                            if ("Content-Type".ToUpper().Equals(httpEquiv.ToUpper()))
                            {
                                charset = getCharset(meta["CONTENT"]);
                                if (!charset.ToUpper().Equals(character_set.ToUpper()))
                                {
                                    // oops, different character set, restart
                                    character_set = charset;
                                }
                                // once we see the Content-Type meta tag we're finished the pre-read
                                break;
                            }
                        }
                        else if (node is EndTag)
                        {
                            end = (EndTag) node;
                            if (end.TagName.ToUpper().Equals("HEAD".ToUpper()))
                                // or, once we see the </HEAD> tag we're finished the pre-read
                                break;
                        }
                    }
                    // Reset the reader
                    RecreateReader();
                    ret = new NodeIterator(reader, resourceLocn, feedback);
                }
/* TODO				catch (UnsupportedEncodingException uee)
				{
					string msg = "elements() : The content of " + url.RequestUri.ToString() + " has an encoding which is not supported";
					ParserException ex = new ParserException(msg, uee);
					feedback.Error(msg, ex);
					throw ex;
				}
*/
                catch (IOException ioe)
                {
                    string msg = "elements() : Error in opening a connection to " + url.RequestUri.ToString();
                    ParserException ex = new ParserException(msg, ioe);
                    feedback.Error(msg, ex);
                    throw ex;
                }
                finally
                {
                    if (remove_scanner)
                        scanners.Remove("-m");
                }
            return ret;
        }

        /// <summary> Flush the current scanners registered. The registered scanners list becomes empty with this call.
        /// </summary>
        public virtual void FlushScanners()
        {
            scanners = new System.Collections.Hashtable();
        }

        /// <summary> Return the scanner registered in the parser having the
        /// given id
        /// </summary>
        /// <param name="id">The id of the requested scanner
        /// </param>
        /// <returns> TagScanner The Tag Scanner
        ///
        /// </returns>
        public virtual TagScanner GetScanner(string id)
        {
            return (TagScanner) scanners[id];
        }

        /// <summary> Parse the given resource, using the filter provided
        /// </summary>
        public virtual void Parse(string filter)
        {
            foreach (Node node in this)
            {
                if (node != null)
                {
                    if (filter == null)
                        Console.WriteLine(node.ToString());
                    else
                    {
                        // There is a filter. Find if the associated filter of this node
                        // matches the specified filter
                        if (!(node is Tag))
                            continue;
                        Tag tag = (Tag) node;
                        TagScanner scanner = tag.ThisScanner;
                        if (scanner == null)
                            continue;

                        string tagFilter = scanner.Filter;
                        if (tagFilter == null)
                            continue;
                        if (tagFilter.Equals(filter))
                            Console.WriteLine(node.ToString());
                    }
                }
                else
                    Console.WriteLine("Node is null");
            }
        }

        /// <summary> This method should be invoked in order to register some common scanners. The scanners that get added are : <br>
        /// LinkScanner    (filter key "-l")<br>
        /// HTMLImageScanner   (filter key "-i")<br>
        /// HTMLScriptScanner  (filter key "-s") <br>
        /// HTMLStyleScanner   (filter key "-t") <br>
        /// HTMLJspScanner     (filter key "-j") <br>
        /// HTMLAppletScanner  (filter key "-a") <br>
        /// HTMLMetaTagScanner (filter key "-m") <br>
        /// HTMLTitleScanner   (filter key "-t") <br>
        /// HTMLDoctypeScanner (filter key "-d") <br>
        /// HTMLFormScanner    (filter key "-f") <br>
        /// HTMLFrameSetScanner(filter key "-r") <br>
        /// HTMLBaseHREFScanner(filter key "-b") <br>
        /// <br>
        /// Call this method after creating the Parser object. e.g. <BR>
        /// <pre>
        /// Parser parser = new Parser("http://www.yahoo.com");
        /// parser.RegisterScanners();
        /// </pre>
        /// </summary>
        public virtual void RegisterScanners()
        {
            if (scanners.Count > 0)
            {
                System.Console.Error.WriteLine(
                    "RegisterScanners() should be called first, when no other scanner has been registered.");
                System.Console.Error.WriteLine(
                    "Other scanners already exist, hence this method call wont have any effect");
                return;
            }
            LinkScanner linkScanner = new LinkScanner(LinkTag.LINK_TAG_FILTER);
            // Note - The BaseHREF and Image scanners share the same
            // link processor - internally linked up with the factory
            // method in the link scanner class
            AddScanner(linkScanner);
            AddScanner(linkScanner.CreateImageScanner(ImageTag.IMAGE_TAG_FILTER));
            AddScanner(new TitleScanner("-T"));
            AddScanner(new FormScanner("-f", this));
            AddScanner(new BulletListScanner("-bulletList", this));
            AddScanner(new DivScanner("-div"));
        }

        /// <summary> Make a call to RegisterDomScanners(), instead of RegisterScanners(),
        /// when you are interested in retrieving a Dom representation of the html
        /// page. Upon parsing, you will receive an Html object - which will contain
        /// children, one of which would be the body. This is still evolving, and in
        /// future releases, you might see consolidation of Html - to provide you
        /// with methods to access the body and the head.
        /// </summary>
        public virtual void RegisterDomScanners()
        {
            RegisterScanners();
            AddScanner(new HtmlScanner());
            AddScanner(new BodyScanner());
            AddScanner(new HeadScanner());
        }

        /// <summary> Removes a specified scanner object. You can create
        /// an anonymous object as a parameter. This method
        /// will use the scanner's key and remove it from the
        /// registry of scanners.
        /// e.g.
        /// <pre>
        /// removeScanner(new FormScanner(""));
        /// </pre>
        /// </summary>
        /// <param name="scanner">TagScanner object to be removed from the list of registered scanners
        ///
        /// </param>
        public virtual void RemoveScanner(TagScanner scanner)
        {
            scanners.Remove(scanner.ID[0]);
        }

        /// <summary> The main program, which can be executed from the command line
        /// if you build htmlparser as an application instead of a class library
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            Console.WriteLine("HTMLParser v" + VERSION_STRING);
            if (args.Length < 1 || args[0].Equals("-help"))
            {
                Console.WriteLine();
                Console.WriteLine("Syntax : htmlparser <resourceLocn/website> -l");
                Console.WriteLine(
                    "   <resourceLocn> the name of the file to be parsed (with complete path if not in current directory)");
                Console.WriteLine("   -l Show only the link tags extracted from the document");
                Console.WriteLine("   -i Show only the image tags extracted from the document");
                Console.WriteLine("   -s Show only the Javascript code extracted from the document");
                Console.WriteLine("   -t Show only the Style code extracted from the document");
                Console.WriteLine("   -a Show only the Applet tag extracted from the document");
                Console.WriteLine("   -j Parse JSP tags");
                Console.WriteLine("   -m Parse Meta tags");
                Console.WriteLine("   -T Extract the Title");
                Console.WriteLine("   -f Extract forms");
                Console.WriteLine("   -r Extract frameset");
                Console.WriteLine("   -help This screen");
                Console.WriteLine();
                Console.WriteLine("HTML Parser home page : http://htmlparser.sourceforge.net");
                Console.WriteLine();
                Console.WriteLine("Example : htmlparser http://www.yahoo.com");
                Console.WriteLine();
                Console.WriteLine(
                    "If you have any doubts, please join the HTMLParser mailing list (user/developer) from the HTML Parser home page instead of mailing any of the contributors directly. You will be surprised with the quality of open source support. ");
                Environment.Exit(- 1);
            }
            try
            {
                Parser parser = new Parser(args[0]);
                Console.WriteLine("Parsing " + parser.URL);
                parser.RegisterScanners();
                try
                {
                    if (args.Length == 2)
                    {
                        parser.Parse(args[1]);
                    }
                    else
                        parser.Parse(null);
                }
                catch (SystemException e)
                {
                    SupportClass.WriteStackTrace(e, Console.Error);
                }
            }
            catch (ParserException e)
            {
                SupportClass.WriteStackTrace(e, Console.Error);
            }
        }

        public virtual void VisitAllNodesWith(NodeVisitor visitor)
        {
            foreach (Node node in this)
            {
                node.Accept(visitor);
            }
            visitor.FinishedParsing();
        }


        public virtual Node[] ExtractAllNodesThatAre(Type nodeType)
        {
            NodeList nodeList = new NodeList();
            foreach (Node node in this)
            {
                node.CollectInto(nodeList, nodeType);
            }
            return nodeList.ToNodeArray();
        }

        /// <summary> Creates the parser on an input string.
        /// </summary>
        /// <param name="">inputHTML
        /// </param>
        /// <returns> Parser
        ///
        /// </returns>
        public static Parser CreateParser(string inputHTML)
        {
            NodeReader reader = new NodeReader(new StringReader(inputHTML), "");
            return new Parser(reader, new NoFeedback());
        }

        public static Parser CreateLinkRecognizingParser(string inputHTML)
        {
            Parser parser = CreateParser(inputHTML);
            parser.AddScanner(new LinkScanner(LinkTag.LINK_TAG_FILTER));
            return parser;
        }

        static Parser()
        {
            lineSeparator = Environment.NewLine;
            noFeedback = new DefaultParserFeedback(DefaultParserFeedback.QUIET);
            stdout = new DefaultParserFeedback();
        }
    }
}
