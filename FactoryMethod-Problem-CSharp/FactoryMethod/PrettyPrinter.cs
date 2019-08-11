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
using System.IO;
using System.Xml;

namespace Industriallogic.FactoryMethod
{
    public class PrettyPrinter
    {
        public String Format(String inputXML)
        {
            String formatted = "";
            try
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(inputXML));
                reader.WhitespaceHandling = WhitespaceHandling.None;

                while (reader.Read())
                    formatted += GetNodeText(reader);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return formatted;
        }

        private static String GetNodeText(XmlReader reader)
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    return FormatElement(reader);
                case XmlNodeType.Text:
                    return String.Format(reader.Value);
                case XmlNodeType.EndElement:
                    return String.Format("</{0}>", reader.Name);
                case XmlNodeType.Attribute:
                    return String.Format("{0}", reader.Name);
            }

            return "";
        }

        private static String FormatElement(XmlReader reader)
        {
            String attributes = FormatAttributes(reader);
            string format = "<{0}{1}>";
            if (reader.IsEmptyElement)
                format += "</{0}>";
            return String.Format(format, reader.Name, attributes);
        }

        private static string FormatAttributes(XmlReader reader)
        {
            String attributes = "";
            if (reader.HasAttributes)
            {
                for (int attribute = 0; attribute < reader.AttributeCount; attribute++)
                {
                    reader.MoveToAttribute(attribute);
                    attributes += String.Format(" {0}=\"{1}\"", reader.Name, reader.Value);
                }
                reader.MoveToElement(); //Moves the reader back to the element node.
            }
            return attributes;
        }
    }
}
