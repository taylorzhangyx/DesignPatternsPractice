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
//
// This class was contributed by
// Derrick Oswald
//
using System;
using System.Collections;

namespace org.htmlparser.util
{
    /// <summary> Translate numeric character references and character entity references to unicode characters.
    /// Based on tables found at <a href="http://www.w3.org/TR/REC-html40/sgml/entities.html">
    /// http://www.w3.org/TR/REC-html40/sgml/entities.html</a>
    /// <p>Typical usage:
    /// <pre>
    /// String s = Translate.decode (getTextFromHtmlPage ());
    /// </pre>
    /// </summary>
    /// <author>  <a href='mailto:DerrickOswald@users.sourceforge.net?subject=Character Reference Translation class'>Derrick Oswald</a>
    /// 
    /// </author>
    public class Translate
    {
        /// <summary> Table mapping entity reference kernel to character.
        /// <p><code>String</code>-><code>Character</code>
        /// </summary>
        protected static IDictionary refChar;

        /// <summary> Table mapping character to entity reference kernel.
        /// <p><code>Character</code>-><code>String</code>
        /// </summary>
        protected static IDictionary charRefTable;

        /// <summary> Private constructor.
        /// This class is fully static and thread safe.
        /// </summary>
        private Translate()
        {
        }

        /// <summary> Convert a reference to a unicode character.
        /// Convert a single numeric character reference or character entity reference
        /// to a unicode character.
        /// </summary>
        /// <param name="value">The string to convert. Of the form &xxxx; or &amp;#xxxx; with
        /// or without the leading ampersand or trailing semi-colon.
        /// </param>
        /// <returns> The converted character or '\0' (zero) if the string is an
        /// invalid reference.
        /// 
        /// </returns>
        public static char ConvertToChar(string value)
        {
            int length;
            char ret;

            ret = (char) (0);

            length = value.Length;
            if (0 < length)
            {
                if ('&' == value[0])
                {
                    value = value.Substring(1);
                    length--;
                }
                if (0 < length)
                {
                    if (';' == value[length - 1])
                        value = value.Substring(0, (--length) - (0));
                    if (0 < length)
                    {
                        if ('#' == value[0])
                            try
                            {
                                ret = (char) System.Int32.Parse(value.Substring(1));
                            }
                            catch (System.FormatException)
                            {
                                /* failed conversion, return 0 */
                            }
                        else
                        {
                            object val = refChar[value];
                            if (val != null)
                                ret = (char) val;
                        }
                    }
                }
            }

            return (ret);
        }

        /// <summary> Decode a string containing references.
        /// Change all numeric character reference and character entity references
        /// to unicode characters.
        /// </summary>
        /// <param name="string">The string to translate.
        /// 
        /// </param>
        public static string Decode(string value)
        {
            int index;
            int length;
            int amp;
            int semi;
            string code;
            char character;
            System.Text.StringBuilder ret;
            ret = new System.Text.StringBuilder(value.Length);
            index = 0;
            length = value.Length;

            while ((index < length) && (- 1 != (amp = value.IndexOf((System.Char) '&', index))))
            {
                ret.Append(value.Substring(index, (amp) - (index)));
                index = amp + 1;
                if (amp < length - 1)
                {
                    semi = value.IndexOf((System.Char) ';', amp);
                    if (- 1 != semi)
                        code = value.Substring(amp, (semi + 1) - (amp));
                    else
                        code = value.Substring(amp);
                    if (0 != (character = ConvertToChar(code)))
                        index += code.Length - 1;
                    else
                        character = '&';
                }
                else
                    character = '&';
                ret.Append(character);
            }
            if (index < length)
                ret.Append(value.Substring(index));
            return (ret.ToString());
        }

        /// <summary> Convert a character to a character entity reference.
        /// Convert a unicode character to a character entity reference of
        /// the form &xxxx;.
        /// </summary>
        /// <param name="character">The character to convert.
        /// </param>
        /// <returns> The converted character or <code>null</code> if the character
        /// is not one of the known entity references.
        /// 
        /// </returns>
        public static string ConvertToString(System.Char character)
        {
            System.Text.StringBuilder buffer;
            string ret;
            if (null != (ret = (string) charRefTable[character]))
            {
                buffer = new System.Text.StringBuilder(ret.Length + 2);
                buffer.Append('&');
                buffer.Append(ret);
                buffer.Append(';');
                ret = buffer.ToString();
            }
            return (ret);
        }

        /// <summary> Convert a character to a numeric character reference.
        /// Convert a unicode character to a numeric character reference of
        /// the form &amp;#xxxx;.
        /// </summary>
        /// <param name="character">The character to convert.
        /// </param>
        /// <returns> The converted character.
        /// 
        /// </returns>
        public static string ConvertToString(int character)
        {
            System.Text.StringBuilder ret;
            ret = new System.Text.StringBuilder(13); /* &#2147483647; */
            ret.Append("&#");
            ret.Append(character);
            ret.Append(';');
            return (ret.ToString());
        }

        /// <summary> Encode a string to use references.
        /// Change all characters that are not ASCII to their numeric character
        /// reference or character entity reference.
        /// This implementation is inefficient, allocating a new
        /// <code>Character</code> for each character in the string,
        /// but this class is primarily intended to decode strings
        /// so efficiency and speed in the encoding was not a priority.
        /// </summary>
        /// <param name="string">The string to translate.
        /// 
        /// </param>
        public static string Encode(string value)
        {
            int length;
            char c;
            System.Char character;
            string converted;
            System.Text.StringBuilder ret;
            ret = new System.Text.StringBuilder(value.Length*6);
            length = value.Length;
            for (int i = 0; i < length; i++)
            {
                c = value[i];
                character = c;
                if (null != (converted = ConvertToString(character)))
                    ret.Append(converted);
                else if (!((c > 0x001F) && (c < 0x007F)))
                {
                    converted = ConvertToString(c);
                    ret.Append(converted);
                }
                else
                    ret.Append(character);
            }
            return (ret.ToString());
        }

        static Translate()
        {
            {
                refChar = new Hashtable(1000);

                // Portions � International Organization for Standardization 1986
                // Permission to copy in any form is granted for use with
                // conforming SGML systems and applications as defined in
                // ISO 8879, provided this notice is included in all copies.
                // Character entity set. Typical invocation:
                // <!ENTITY % HTMLlat1 PUBLIC
                // "-//W3C//ENTITIES Latin 1//EN//HTML">
                // %HTMLlat1;
                refChar["nbsp"] = '\u00a0'; // no-break space = non-breaking space, U+00A0 ISOnum
                refChar["iexcl"] = '\u00a1'; // inverted exclamation mark, U+00A1 ISOnum
                refChar["cent"] = '\u00a2'; // cent sign, U+00A2 ISOnum
                refChar["pound"] = '\u00a3'; // pound sign, U+00A3 ISOnum
                refChar["curren"] = '\u00a4'; // currency sign, U+00A4 ISOnum
                refChar["yen"] = '\u00a5'; // yen sign = yuan sign, U+00A5 ISOnum
                refChar["brvbar"] = '\u00a6'; // broken bar = broken vertical bar, U+00A6 ISOnum
                refChar["sect"] = '\u00a7'; // section sign, U+00A7 ISOnum
                refChar["uml"] = '\u00a8'; // diaeresis = spacing diaeresis, U+00A8 ISOdia
                refChar["copy"] = '\u00a9'; // copyright sign, U+00A9 ISOnum
                refChar["ordf"] = '\u00aa'; // feminine ordinal indicator, U+00AA ISOnum
                refChar["laquo"] = '\u00ab';
                    // left-pointing double angle quotation mark = left pointing guillemet, U+00AB ISOnum
                refChar["not"] = '\u00ac'; // not sign, U+00AC ISOnum
                refChar["shy"] = '\u00ad'; // soft hyphen = discretionary hyphen, U+00AD ISOnum
                refChar["reg"] = '\u00ae'; // registered sign = registered trade mark sign, U+00AE ISOnum
                refChar["macr"] = '\u00af'; // macron = spacing macron = overline = APL overbar, U+00AF ISOdia
                refChar["deg"] = '\u00b0'; // degree sign, U+00B0 ISOnum
                refChar["plusmn"] = '\u00b1'; // plus-minus sign = plus-or-minus sign, U+00B1 ISOnum
                refChar["sup2"] = '\u00b2'; // superscript two = superscript digit two = squared, U+00B2 ISOnum
                refChar["sup3"] = '\u00b3'; // superscript three = superscript digit three = cubed, U+00B3 ISOnum
                refChar["acute"] = '\u00b4'; // acute accent = spacing acute, U+00B4 ISOdia
                refChar["micro"] = '\u00b5'; // micro sign, U+00B5 ISOnum
                refChar["para"] = '\u00b6'; // pilcrow sign = paragraph sign, U+00B6 ISOnum
                refChar["middot"] = '\u00b7'; // middle dot = Georgian comma = Greek middle dot, U+00B7 ISOnum
                refChar["cedil"] = '\u00b8'; // cedilla = spacing cedilla, U+00B8 ISOdia
                refChar["sup1"] = '\u00b9'; // superscript one = superscript digit one, U+00B9 ISOnum
                refChar["ordm"] = '\u00ba'; // masculine ordinal indicator, U+00BA ISOnum
                refChar["raquo"] = '\u00bb';
                    // right-pointing double angle quotation mark = right pointing guillemet, U+00BB ISOnum
                refChar["frac14"] = '\u00bc'; // vulgar fraction one quarter = fraction one quarter, U+00BC ISOnum
                refChar["frac12"] = '\u00bd'; // vulgar fraction one half = fraction one half, U+00BD ISOnum
                refChar["frac34"] = '\u00be'; // vulgar fraction three quarters = fraction three quarters, U+00BE ISOnum
                refChar["iquest"] = '\u00bf'; // inverted question mark = turned question mark, U+00BF ISOnum
                refChar["Agrave"] = '\u00c0';
                    // latin capital letter A with grave = latin capital letter A grave, U+00C0 ISOlat1
                refChar["Aacute"] = '\u00c1'; // latin capital letter A with acute, U+00C1 ISOlat1
                refChar["Acirc"] = '\u00c2'; // latin capital letter A with circumflex, U+00C2 ISOlat1
                refChar["Atilde"] = '\u00c3'; // latin capital letter A with tilde, U+00C3 ISOlat1
                refChar["Auml"] = '\u00c4'; // latin capital letter A with diaeresis, U+00C4 ISOlat1
                refChar["Aring"] = '\u00c5';
                    // latin capital letter A with ring above = latin capital letter A ring, U+00C5 ISOlat1
                refChar["AElig"] = '\u00c6'; // latin capital letter AE = latin capital ligature AE, U+00C6 ISOlat1
                refChar["Ccedil"] = '\u00c7'; // latin capital letter C with cedilla, U+00C7 ISOlat1
                refChar["Egrave"] = '\u00c8'; // latin capital letter E with grave, U+00C8 ISOlat1
                refChar["Eacute"] = '\u00c9'; // latin capital letter E with acute, U+00C9 ISOlat1
                refChar["Ecirc"] = '\u00ca'; // latin capital letter E with circumflex, U+00CA ISOlat1
                refChar["Euml"] = '\u00cb'; // latin capital letter E with diaeresis, U+00CB ISOlat1
                refChar["Igrave"] = '\u00cc'; // latin capital letter I with grave, U+00CC ISOlat1
                refChar["Iacute"] = '\u00cd'; // latin capital letter I with acute, U+00CD ISOlat1
                refChar["Icirc"] = '\u00ce'; // latin capital letter I with circumflex, U+00CE ISOlat1
                refChar["Iuml"] = '\u00cf'; // latin capital letter I with diaeresis, U+00CF ISOlat1
                refChar["ETH"] = '\u00d0'; // latin capital letter ETH, U+00D0 ISOlat1
                refChar["Ntilde"] = '\u00d1'; // latin capital letter N with tilde, U+00D1 ISOlat1
                refChar["Ograve"] = '\u00d2'; // latin capital letter O with grave, U+00D2 ISOlat1
                refChar["Oacute"] = '\u00d3'; // latin capital letter O with acute, U+00D3 ISOlat1
                refChar["Ocirc"] = '\u00d4'; // latin capital letter O with circumflex, U+00D4 ISOlat1
                refChar["Otilde"] = '\u00d5'; // latin capital letter O with tilde, U+00D5 ISOlat1
                refChar["Ouml"] = '\u00d6'; // latin capital letter O with diaeresis, U+00D6 ISOlat1
                refChar["times"] = '\u00d7'; // multiplication sign, U+00D7 ISOnum
                refChar["Oslash"] = '\u00d8';
                    // latin capital letter O with stroke = latin capital letter O slash, U+00D8 ISOlat1
                refChar["Ugrave"] = '\u00d9'; // latin capital letter U with grave, U+00D9 ISOlat1
                refChar["Uacute"] = '\u00da'; // latin capital letter U with acute, U+00DA ISOlat1
                refChar["Ucirc"] = '\u00db'; // latin capital letter U with circumflex, U+00DB ISOlat1
                refChar["Uuml"] = '\u00dc'; // latin capital letter U with diaeresis, U+00DC ISOlat1
                refChar["Yacute"] = '\u00dd'; // latin capital letter Y with acute, U+00DD ISOlat1
                refChar["THORN"] = '\u00de'; // latin capital letter THORN, U+00DE ISOlat1
                refChar["szlig"] = '\u00df'; // latin small letter sharp s = ess-zed, U+00DF ISOlat1
                refChar["agrave"] = '\u00e0';
                    // latin small letter a with grave = latin small letter a grave, U+00E0 ISOlat1
                refChar["aacute"] = '\u00e1'; // latin small letter a with acute, U+00E1 ISOlat1
                refChar["acirc"] = '\u00e2'; // latin small letter a with circumflex, U+00E2 ISOlat1
                refChar["atilde"] = '\u00e3'; // latin small letter a with tilde, U+00E3 ISOlat1
                refChar["auml"] = '\u00e4'; // latin small letter a with diaeresis, U+00E4 ISOlat1
                refChar["aring"] = '\u00e5';
                    // latin small letter a with ring above = latin small letter a ring, U+00E5 ISOlat1
                refChar["aelig"] = '\u00e6'; // latin small letter ae = latin small ligature ae, U+00E6 ISOlat1
                refChar["ccedil"] = '\u00e7'; // latin small letter c with cedilla, U+00E7 ISOlat1
                refChar["egrave"] = '\u00e8'; // latin small letter e with grave, U+00E8 ISOlat1
                refChar["eacute"] = '\u00e9'; // latin small letter e with acute, U+00E9 ISOlat1
                refChar["ecirc"] = '\u00ea'; // latin small letter e with circumflex, U+00EA ISOlat1
                refChar["euml"] = '\u00eb'; // latin small letter e with diaeresis, U+00EB ISOlat1
                refChar["igrave"] = '\u00ec'; // latin small letter i with grave, U+00EC ISOlat1
                refChar["iacute"] = '\u00ed'; // latin small letter i with acute, U+00ED ISOlat1
                refChar["icirc"] = '\u00ee'; // latin small letter i with circumflex, U+00EE ISOlat1
                refChar["iuml"] = '\u00ef'; // latin small letter i with diaeresis, U+00EF ISOlat1
                refChar["eth"] = '\u00f0'; // latin small letter eth, U+00F0 ISOlat1
                refChar["ntilde"] = '\u00f1'; // latin small letter n with tilde, U+00F1 ISOlat1
                refChar["ograve"] = '\u00f2'; // latin small letter o with grave, U+00F2 ISOlat1
                refChar["oacute"] = '\u00f3'; // latin small letter o with acute, U+00F3 ISOlat1
                refChar["ocirc"] = '\u00f4'; // latin small letter o with circumflex, U+00F4 ISOlat1
                refChar["otilde"] = '\u00f5'; // latin small letter o with tilde, U+00F5 ISOlat1
                refChar["ouml"] = '\u00f6'; // latin small letter o with diaeresis, U+00F6 ISOlat1
                refChar["divide"] = '\u00f7'; // division sign, U+00F7 ISOnum
                refChar["oslash"] = '\u00f8';
                    // latin small letter o with stroke, = latin small letter o slash, U+00F8 ISOlat1
                refChar["ugrave"] = '\u00f9'; // latin small letter u with grave, U+00F9 ISOlat1
                refChar["uacute"] = '\u00fa'; // latin small letter u with acute, U+00FA ISOlat1
                refChar["ucirc"] = '\u00fb'; // latin small letter u with circumflex, U+00FB ISOlat1
                refChar["uuml"] = '\u00fc'; // latin small letter u with diaeresis, U+00FC ISOlat1
                refChar["yacute"] = '\u00fd'; // latin small letter y with acute, U+00FD ISOlat1
                refChar["thorn"] = '\u00fe'; // latin small letter thorn, U+00FE ISOlat1
                refChar["yuml"] = '\u00ff'; // latin small letter y with diaeresis, U+00FF ISOlat1
                // Mathematical, Greek and Symbolic characters for HTML
                // Character entity set. Typical invocation:
                // <!ENTITY % HTMLsymbol PUBLIC
                // "-//W3C//ENTITIES Symbols//EN//HTML">
                // %HTMLsymbol;
                // Portions � International Organization for Standardization 1986:
                // Permission to copy in any form is granted for use with
                // conforming SGML systems and applications as defined in
                // ISO 8879, provided this notice is included in all copies.
                // Relevant ISO entity set is given unless names are newly introduced.
                // New names (i.e., not in ISO 8879 list) do not clash with any
                // existing ISO 8879 entity names. ISO 10646 character numbers
                // are given for each character, in hex. CDATA values are decimal
                // conversions of the ISO 10646 values and refer to the document
                // character set. Names are ISO 10646 names.
                // Latin Extended-B
                refChar["fnof"] = '\u0192'; // latin small f with hook = function = florin, U+0192 ISOtech
                // Greek
                refChar["Alpha"] = '\u0391'; // greek capital letter alpha, U+0391
                refChar["Beta"] = '\u0392'; // greek capital letter beta, U+0392
                refChar["Gamma"] = '\u0393'; // greek capital letter gamma, U+0393 ISOgrk3
                refChar["Delta"] = '\u0394'; // greek capital letter delta, U+0394 ISOgrk3
                refChar["Epsilon"] = '\u0395'; // greek capital letter epsilon, U+0395
                refChar["Zeta"] = '\u0396'; // greek capital letter zeta, U+0396
                refChar["Eta"] = '\u0397'; // greek capital letter eta, U+0397
                refChar["Theta"] = '\u0398'; // greek capital letter theta, U+0398 ISOgrk3
                refChar["Iota"] = '\u0399'; // greek capital letter iota, U+0399
                refChar["Kappa"] = '\u039a'; // greek capital letter kappa, U+039A
                refChar["Lambda"] = '\u039b'; // greek capital letter lambda, U+039B ISOgrk3
                refChar["Mu"] = '\u039c'; // greek capital letter mu, U+039C
                refChar["Nu"] = '\u039d'; // greek capital letter nu, U+039D
                refChar["Xi"] = '\u039e'; // greek capital letter xi, U+039E ISOgrk3
                refChar["Omicron"] = '\u039f'; // greek capital letter omicron, U+039F
                refChar["Pi"] = '\u03a0'; // greek capital letter pi, U+03A0 ISOgrk3
                refChar["Rho"] = '\u03a1'; // greek capital letter rho, U+03A1
                // there is no Sigmaf, and no U+03A2 character either
                refChar["Sigma"] = '\u03a3'; // greek capital letter sigma, U+03A3 ISOgrk3
                refChar["Tau"] = '\u03a4'; // greek capital letter tau, U+03A4
                refChar["Upsilon"] = '\u03a5'; // greek capital letter upsilon, U+03A5 ISOgrk3
                refChar["Phi"] = '\u03a6'; // greek capital letter phi, U+03A6 ISOgrk3
                refChar["Chi"] = '\u03a7'; // greek capital letter chi, U+03A7
                refChar["Psi"] = '\u03a8'; // greek capital letter psi, U+03A8 ISOgrk3
                refChar["Omega"] = '\u03a9'; // greek capital letter omega, U+03A9 ISOgrk3
                refChar["alpha"] = '\u03b1'; // greek small letter alpha, U+03B1 ISOgrk3
                refChar["beta"] = '\u03b2'; // greek small letter beta, U+03B2 ISOgrk3
                refChar["gamma"] = '\u03b3'; // greek small letter gamma, U+03B3 ISOgrk3
                refChar["delta"] = '\u03b4'; // greek small letter delta, U+03B4 ISOgrk3
                refChar["epsilon"] = '\u03b5'; // greek small letter epsilon, U+03B5 ISOgrk3
                refChar["zeta"] = '\u03b6'; // greek small letter zeta, U+03B6 ISOgrk3
                refChar["eta"] = '\u03b7'; // greek small letter eta, U+03B7 ISOgrk3
                refChar["theta"] = '\u03b8'; // greek small letter theta, U+03B8 ISOgrk3
                refChar["iota"] = '\u03b9'; // greek small letter iota, U+03B9 ISOgrk3
                refChar["kappa"] = '\u03ba'; // greek small letter kappa, U+03BA ISOgrk3
                refChar["lambda"] = '\u03bb'; // greek small letter lambda, U+03BB ISOgrk3
                refChar["mu"] = '\u03bc'; // greek small letter mu, U+03BC ISOgrk3
                refChar["nu"] = '\u03bd'; // greek small letter nu, U+03BD ISOgrk3
                refChar["xi"] = '\u03be'; // greek small letter xi, U+03BE ISOgrk3
                refChar["omicron"] = '\u03bf'; // greek small letter omicron, U+03BF NEW
                refChar["pi"] = '\u03c0'; // greek small letter pi, U+03C0 ISOgrk3
                refChar["rho"] = '\u03c1'; // greek small letter rho, U+03C1 ISOgrk3
                refChar["sigmaf"] = '\u03c2'; // greek small letter final sigma, U+03C2 ISOgrk3
                refChar["sigma"] = '\u03c3'; // greek small letter sigma, U+03C3 ISOgrk3
                refChar["tau"] = '\u03c4'; // greek small letter tau, U+03C4 ISOgrk3
                refChar["upsilon"] = '\u03c5'; // greek small letter upsilon, U+03C5 ISOgrk3
                refChar["phi"] = '\u03c6'; // greek small letter phi, U+03C6 ISOgrk3
                refChar["chi"] = '\u03c7'; // greek small letter chi, U+03C7 ISOgrk3
                refChar["psi"] = '\u03c8'; // greek small letter psi, U+03C8 ISOgrk3
                refChar["omega"] = '\u03c9'; // greek small letter omega, U+03C9 ISOgrk3
                refChar["thetasym"] = '\u03d1'; // greek small letter theta symbol, U+03D1 NEW
                refChar["upsih"] = '\u03d2'; // greek upsilon with hook symbol, U+03D2 NEW
                refChar["piv"] = '\u03d6'; // greek pi symbol, U+03D6 ISOgrk3
                // General Punctuation
                refChar["bull"] = '\u2022'; // bullet = black small circle, U+2022 ISOpub
                // bullet is NOT the same as bullet operator, U+2219
                refChar["hellip"] = '\u2026'; // horizontal ellipsis = three dot leader, U+2026 ISOpub
                refChar["prime"] = '\u2032'; // prime = minutes = feet, U+2032 ISOtech
                refChar["Prime"] = '\u2033'; // double prime = seconds = inches, U+2033 ISOtech
                refChar["oline"] = '\u203e'; // overline = spacing overscore, U+203E NEW
                refChar["frasl"] = '\u2044'; // fraction slash, U+2044 NEW
                // Letterlike Symbols
                refChar["weierp"] = '\u2118'; // script capital P = power set = Weierstrass p, U+2118 ISOamso
                refChar["image"] = '\u2111'; // blackletter capital I = imaginary part, U+2111 ISOamso
                refChar["real"] = '\u211c'; // blackletter capital R = real part symbol, U+211C ISOamso
                refChar["trade"] = '\u2122'; // trade mark sign, U+2122 ISOnum
                refChar["alefsym"] = '\u2135'; // alef symbol = first transfinite cardinal, U+2135 NEW
                // alef symbol is NOT the same as hebrew letter alef,
                // U+05D0 although the same glyph could be used to depict both characters
                // Arrows
                refChar["larr"] = '\u2190'; // leftwards arrow, U+2190 ISOnum
                refChar["uarr"] = '\u2191'; // upwards arrow, U+2191 ISOnum
                refChar["rarr"] = '\u2192'; // rightwards arrow, U+2192 ISOnum
                refChar["darr"] = '\u2193'; // downwards arrow, U+2193 ISOnum
                refChar["harr"] = '\u2194'; // left right arrow, U+2194 ISOamsa
                refChar["crarr"] = '\u21b5'; // downwards arrow with corner leftwards = carriage return, U+21B5 NEW
                refChar["lArr"] = '\u21d0'; // leftwards double arrow, U+21D0 ISOtech
                // ISO 10646 does not say that lArr is the same as the 'is implied by' arrow
                // but also does not have any other character for that function. So ? lArr can
                // be used for 'is implied by' as ISOtech suggests
                refChar["uArr"] = '\u21d1'; // upwards double arrow, U+21D1 ISOamsa
                refChar["rArr"] = '\u21d2'; // rightwards double arrow, U+21D2 ISOtech
                // ISO 10646 does not say this is the 'implies' character but does not have
                // another character with this function so ?
                // rArr can be used for 'implies' as ISOtech suggests
                refChar["dArr"] = '\u21d3'; // downwards double arrow, U+21D3 ISOamsa
                refChar["hArr"] = '\u21d4'; // left right double arrow, U+21D4 ISOamsa
                // Mathematical Operators
                refChar["forall"] = '\u2200'; // for all, U+2200 ISOtech
                refChar["part"] = '\u2202'; // partial differential, U+2202 ISOtech
                refChar["exist"] = '\u2203'; // there exists, U+2203 ISOtech
                refChar["empty"] = '\u2205'; // empty set = null set = diameter, U+2205 ISOamso
                refChar["nabla"] = '\u2207'; // nabla = backward difference, U+2207 ISOtech
                refChar["isin"] = '\u2208'; // element of, U+2208 ISOtech
                refChar["notin"] = '\u2209'; // not an element of, U+2209 ISOtech
                refChar["ni"] = '\u220b'; // contains as member, U+220B ISOtech
                // should there be a more memorable name than 'ni'?
                refChar["prod"] = '\u220f'; // n-ary product = product sign, U+220F ISOamsb
                // prod is NOT the same character as U+03A0 'greek capital letter pi' though
                // the same glyph might be used for both
                refChar["sum"] = '\u2211'; // n-ary sumation, U+2211 ISOamsb
                // sum is NOT the same character as U+03A3 'greek capital letter sigma'
                // though the same glyph might be used for both
                refChar["minus"] = '\u2212'; // minus sign, U+2212 ISOtech
                refChar["lowast"] = '\u2217'; // asterisk operator, U+2217 ISOtech
                refChar["radic"] = '\u221a'; // square root = radical sign, U+221A ISOtech
                refChar["prop"] = '\u221d'; // proportional to, U+221D ISOtech
                refChar["infin"] = '\u221e'; // infinity, U+221E ISOtech
                refChar["ang"] = '\u2220'; // angle, U+2220 ISOamso
                refChar["and"] = '\u2227'; // logical and = wedge, U+2227 ISOtech
                refChar["or"] = '\u2228'; // logical or = vee, U+2228 ISOtech
                refChar["cap"] = '\u2229'; // intersection = cap, U+2229 ISOtech
                refChar["cup"] = '\u222a'; // union = cup, U+222A ISOtech
                refChar["int"] = '\u222b'; // integral, U+222B ISOtech
                refChar["there4"] = '\u2234'; // therefore, U+2234 ISOtech
                refChar["sim"] = '\u223c'; // tilde operator = varies with = similar to, U+223C ISOtech
                // tilde operator is NOT the same character as the tilde, U+007E,
                // although the same glyph might be used to represent both
                refChar["cong"] = '\u2245'; // approximately equal to, U+2245 ISOtech
                refChar["asymp"] = '\u2248'; // almost equal to = asymptotic to, U+2248 ISOamsr
                refChar["ne"] = '\u2260'; // not equal to, U+2260 ISOtech
                refChar["equiv"] = '\u2261'; // identical to, U+2261 ISOtech
                refChar["le"] = '\u2264'; // less-than or equal to, U+2264 ISOtech
                refChar["ge"] = '\u2265'; // greater-than or equal to, U+2265 ISOtech
                refChar["sub"] = '\u2282'; // subset of, U+2282 ISOtech
                refChar["sup"] = '\u2283'; // superset of, U+2283 ISOtech
                // note that nsup, 'not a superset of, U+2283' is not covered by the Symbol
                // font encoding and is not included. Should it be, for symmetry?
                // It is in ISOamsn
                refChar["nsub"] = '\u2284'; // not a subset of, U+2284 ISOamsn
                refChar["sube"] = '\u2286'; // subset of or equal to, U+2286 ISOtech
                refChar["supe"] = '\u2287'; // superset of or equal to, U+2287 ISOtech
                refChar["oplus"] = '\u2295'; // circled plus = direct sum, U+2295 ISOamsb
                refChar["otimes"] = '\u2297'; // circled times = vector product, U+2297 ISOamsb
                refChar["perp"] = '\u22a5'; // up tack = orthogonal to = perpendicular, U+22A5 ISOtech
                refChar["sdot"] = '\u22c5'; // dot operator, U+22C5 ISOamsb
                // dot operator is NOT the same character as U+00B7 middle dot
                // Miscellaneous Technical
                refChar["lceil"] = '\u2308'; // left ceiling = apl upstile, U+2308 ISOamsc
                refChar["rceil"] = '\u2309'; // right ceiling, U+2309 ISOamsc
                refChar["lfloor"] = '\u230a'; // left floor = apl downstile, U+230A ISOamsc
                refChar["rfloor"] = '\u230b'; // right floor, U+230B ISOamsc
                refChar["lang"] = '\u2329'; // left-pointing angle bracket = bra, U+2329 ISOtech
                // lang is NOT the same character as U+003C 'less than'
                // or U+2039 'single left-pointing angle quotation mark'
                refChar["rang"] = '\u232a'; // right-pointing angle bracket = ket, U+232A ISOtech
                // rang is NOT the same character as U+003E 'greater than'
                // or U+203A 'single right-pointing angle quotation mark'
                // Geometric Shapes
                refChar["loz"] = '\u25ca'; // lozenge, U+25CA ISOpub
                // Miscellaneous Symbols
                refChar["spades"] = '\u2660'; // black spade suit, U+2660 ISOpub
                // black here seems to mean filled as opposed to hollow
                refChar["clubs"] = '\u2663'; // black club suit = shamrock, U+2663 ISOpub
                refChar["hearts"] = '\u2665'; // black heart suit = valentine, U+2665 ISOpub
                refChar["diams"] = '\u2666'; // black diamond suit, U+2666 ISOpub
                // Special characters for HTML
                // Character entity set. Typical invocation:
                // <!ENTITY % HTMLspecial PUBLIC
                // "-//W3C//ENTITIES Special//EN//HTML">
                // %HTMLspecial;
                // Portions � International Organization for Standardization 1986:
                // Permission to copy in any form is granted for use with
                // conforming SGML systems and applications as defined in
                // ISO 8879, provided this notice is included in all copies.
                // Relevant ISO entity set is given unless names are newly introduced.
                // New names (i.e., not in ISO 8879 list) do not clash with any
                // existing ISO 8879 entity names. ISO 10646 character numbers
                // are given for each character, in hex. CDATA values are decimal
                // conversions of the ISO 10646 values and refer to the document
                // character set. Names are ISO 10646 names.
                // C0 Controls and Basic Latin
                refChar["quot"] = '\''; // quotation mark = APL quote, U+0022 ISOnum
                refChar["amp"] = '\u0026'; // ampersand, U+0026 ISOnum
                refChar["lt"] = '\u003c'; // less-than sign, U+003C ISOnum
                refChar["gt"] = '\u003e'; // greater-than sign, U+003E ISOnum
                // Latin Extended-A
                refChar["OElig"] = '\u0152'; // latin capital ligature OE, U+0152 ISOlat2
                refChar["oelig"] = '\u0153'; // latin small ligature oe, U+0153 ISOlat2
                // ligature is a misnomer, this is a separate character in some languages
                refChar["Scaron"] = '\u0160'; // latin capital letter S with caron, U+0160 ISOlat2
                refChar["scaron"] = '\u0161'; // latin small letter s with caron, U+0161 ISOlat2
                refChar["Yuml"] = '\u0178'; // latin capital letter Y with diaeresis, U+0178 ISOlat2
                // Spacing Modifier Letters
                refChar["circ"] = '\u02c6'; // modifier letter circumflex accent, U+02C6 ISOpub
                refChar["tilde"] = '\u02dc'; // small tilde, U+02DC ISOdia
                // General Punctuation
                refChar["ensp"] = '\u2002'; // en space, U+2002 ISOpub
                refChar["emsp"] = '\u2003'; // em space, U+2003 ISOpub
                refChar["thinsp"] = '\u2009'; // thin space, U+2009 ISOpub
                refChar["zwnj"] = '\u200c'; // zero width non-joiner, U+200C NEW RFC 2070
                refChar["zwj"] = '\u200d'; // zero width joiner, U+200D NEW RFC 2070
                refChar["lrm"] = '\u200e'; // left-to-right mark, U+200E NEW RFC 2070
                refChar["rlm"] = '\u200f'; // right-to-left mark, U+200F NEW RFC 2070
                refChar["ndash"] = '\u2013'; // en dash, U+2013 ISOpub
                refChar["mdash"] = '\u2014'; // em dash, U+2014 ISOpub
                refChar["lsquo"] = '\u2018'; // left single quotation mark, U+2018 ISOnum
                refChar["rsquo"] = '\u2019'; // right single quotation mark, U+2019 ISOnum
                refChar["sbquo"] = '\u201a'; // single low-9 quotation mark, U+201A NEW
                refChar["ldquo"] = '\u201c'; // left double quotation mark, U+201C ISOnum
                refChar["rdquo"] = '\u201d'; // right double quotation mark, U+201D ISOnum
                refChar["bdquo"] = '\u201e'; // double low-9 quotation mark, U+201E NEW
                refChar["dagger"] = '\u2020'; // dagger, U+2020 ISOpub
                refChar["Dagger"] = '\u2021'; // double dagger, U+2021 ISOpub
                refChar["permil"] = '\u2030'; // per mille sign, U+2030 ISOtech
                refChar["lsaquo"] = '\u2039'; // single left-pointing angle quotation mark, U+2039 ISO proposed
                // lsaquo is proposed but not yet ISO standardized
                refChar["rsaquo"] = '\u203a'; // single right-pointing angle quotation mark, U+203A ISO proposed
                // rsaquo is proposed but not yet ISO standardized
                refChar["euro"] = '\u20ac'; // euro sign, U+20AC NEW
            }
            {
                charRefTable = new Hashtable(refChar.Count);
                foreach (string key in refChar.Keys)
                {
                    System.Char character = (System.Char) refChar[key];
                    charRefTable[character] = key;
                }
            }
        }
    }
}
