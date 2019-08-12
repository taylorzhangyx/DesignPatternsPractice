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
using org.htmlparser.tags;
using org.htmlparser.util;
using org.htmlparser.visitors;

namespace org.htmlparser
{
    public interface Node
    {
        /**
		 * Returns a string representation of the node. This is an important method, it allows a simple string transformation
		 * of a web page, regardless of a node.<br>
		 * Typical application code (for extracting only the text from a web page) would then be simplified to  :<br>
		 * <pre>
		 * Node node;
		 * for (Enumeration e = parser.Elements();e.hasMoreElements();) {
		 *    node = (Node)e.nextElement();
		 *    System.out.println(node.ToPlainTextString()); // Or do whatever processing you
		 * 													// wish with the plain text string
		 * }
		 * </pre>
		 */
        string ToPlainTextString();

        /**
		 * This method will make it easier when using html parser to reproduce html pages (with or without modifications)
		 * Applications reproducing html can use this method on nodes which are to be used or transferred as they were
		 * recieved, with the original html
		 */
        string ToHtml();

        /**
		 * Return the string representation of the node.
		 * Subclasses must define this method, and this is typically to be used in the manner<br>
		 * <pre>System.out.println(node)</pre>
		 * @return java.lang.String
		 */
        string ToString();

        /**
		 * Collect this node and its child nodes (if-applicable) into the collection parameter, provided the node
		 * satisfies the filtering criteria. <P/>
		 *
		 * This mechanism allows powerful filtering code to be written very easily, without bothering about collection
		 * of embedded tags separately. e.g. when we try to get all the links on a page, it is not possible to get it
		 * at the top-level, as many tags (like form tags), can contain links embedded in them. We could get the links
		 * out by checking if the current node is a form tag, and going through its contents. However, this ties us down
		 * to specific tags, and is not a very clean approach. <P/>
		 *
		 * Using CollectInto(), programs get a lot shorter. Now, the code to extract all links from a page would look
		 * like :
		 * <pre>
		 * NodeList collectionList = new NodeList();
		 * String filter = LinkTag.LINK_TAG_FILTER;
		 * foreach (Node node in parser) {
		 * 		node.CollectInto (collectionVector, filter);
		 * }
		 * </pre>
		 * Thus, collectionList will hold all the link nodes, irrespective of how
		 * deep the links are embedded. This of course implies that tags must
		 * fulfill their responsibilities toward honouring certain filters.
		 *
		 * <B>Important:</B> In order to keep performance optimal, <B>do not create</B> you own filter strings, as
		 * the internal matching occurs with the pre-existing filter string object (in the relevant class). i.e. do not
		 * make calls like :
		 * <I>CollectInto(collectionList,"-l")</I>, instead, make calls only like :
		 * <I>CollectInto(collectionList,LinkTag.LINK_TAG_FILTER)</I>.<P/>
		 *
		 * To find out if your desired tag has filtering support, check the API of the tag.
		 */
        void CollectInto(NodeList collectionList, string filter);

        /**
		 * Collect this node and its child nodes (if-applicable) into the collection parameter, provided the node
		 * satisfies the filtering criteria. <P/>
		 *
		 * This mechanism allows powerful filtering code to be written very easily, without bothering about collection
		 * of embedded tags separately. e.g. when we try to get all the links on a page, it is not possible to get it
		 * at the top-level, as many tags (like form tags), can contain links embedded in them. We could get the links
		 * out by checking if the current node is a form tag, and going through its contents. However, this ties us down
		 * to specific tags, and is not a very clean approach. <P/>
		 *
		 * Using CollectInto(), programs get a lot shorter. Now, the code to extract all links from a page would look
		 * like :
		 * <pre>
		 * NodeList collectionList = new NodeList();
		 * foreach (Node node in parser) {
		 * 		node.CollectInto (collectionVector, LinkTag.class);
		 * }
		 * </pre>
		 * Thus, collectionList will hold all the link nodes, irrespective of how
		 * deep the links are embedded.
		 */
        void CollectInto(NodeList collectionList, Type nodeType);

        /**
		 * Returns the beginning position of the tag.
		 */
        int ElementBegin { get; }

        /**
		 * Returns the ending position fo the tag
		 */
        int ElementEnd { get; }

        void Accept(NodeVisitor visitor);

        CompositeTag Parent { get; set; }

        string Text { get; set; }
    }
}
