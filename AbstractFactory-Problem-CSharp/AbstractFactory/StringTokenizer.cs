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

/**
 * A C# port of Sun's version of StringTokenizer (from JDK1.2?), done by Gil
 */

public class StringTokenizer
{
    private int currentPosition;
    private int maxPosition;
    private string str;
    private string delimiters;
    private bool retTokens;

    /**
	 * Constructs a string tokenizer for the specified string. All
	 * characters in the <code>delim</code> argument are the delimiters
	 * for separating tokens.
	 * <p>
	 * If the <code>returnTokens</code> flag is <code>true</code>, then
	 * the delimiter characters are also returned as tokens. Each
	 * delimiter is returned as a string of length one. If the flag is
	 * <code>false</code>, the delimiter characters are skipped and only
	 * serve as separators between tokens.
	 *
	 * @param   str            a string to be parsed.
	 * @param   delim          the delimiters.
	 * @param   returnTokens   flag indicating whether to return the delimiters
	 *                         as tokens.
	 */

    public StringTokenizer(string str, string delim, bool returnTokens)
    {
        currentPosition = 0;
        this.str = str;
        maxPosition = str.Length;
        delimiters = delim;
        retTokens = returnTokens;
    }

    /**
	 * Constructs a string tokenizer for the specified string. The
	 * characters in the <code>delim</code> argument are the delimiters
	 * for separating tokens. Delimiter characters themselves will not
	 * be treated as tokens.
	 *
	 * @param   str     a string to be parsed.
	 * @param   delim   the delimiters.
	 */

    public StringTokenizer(string str, string delim) : this(str, delim, false)
    {
    }

    /**
	 * Constructs a string tokenizer for the specified string. The
	 * tokenizer uses the default delimiter set, which is
	 * <code>"&#92;t&#92;n&#92;r&#92;f"</code>: the space character, the tab
	 * character, the newline character, the carriage-return character,
	 * and the form-feed character. Delimiter characters themselves will
	 * not be treated as tokens.
	 *
	 * @param   str   a string to be parsed.
	 */

    public StringTokenizer(string str) : this(str, " \t\n\r\f", false)
    {
    }

    /**
	 * Skips delimiters.
	 */

    private void SkipDelimiters()
    {
        while (!retTokens &&
               (currentPosition < maxPosition) &&
               (delimiters.IndexOf(str[currentPosition]) >= 0))
        {
            currentPosition++;
        }
    }

    /**
	 * Tests if there are more tokens available from this tokenizer's string.
	 * If this method returns <tt>true</tt>, then a subsequent call to
	 * <tt>nextToken</tt> with no argument will successfully return a token.
	 *
	 * @return  <code>true</code> if and only if there is at least one token
	 *          in the string after the current position; <code>false</code>
	 *          otherwise.
	 */

    public bool HasMoreTokens()
    {
        SkipDelimiters();
        return (currentPosition < maxPosition);
    }

    /**
	 * Returns the next token from this string tokenizer.
	 *
	 * @return     the next token from this string tokenizer.
	 * @exception  NoSuchElementException  if there are no more tokens in this
	 *               tokenizer's string.
	 */

    public string NextToken()
    {
        SkipDelimiters();

        if (currentPosition >= maxPosition)
        {
            throw new Exception();
        }

        int start = currentPosition;
        while ((currentPosition < maxPosition) &&
               (delimiters.IndexOf(str[currentPosition]) < 0))
        {
            currentPosition++;
        }
        if (retTokens && (start == currentPosition) &&
            (delimiters.IndexOf(str[currentPosition]) >= 0))
        {
            currentPosition++;
        }
        return str.Substring(start, currentPosition - start);
    }

    /**
	 * Returns the next token in this string tokenizer's string. First,
	 * the set of characters considered to be delimiters by this
	 * <tt>StringTokenizer</tt> object is changed to be the characters in
	 * the string <tt>delim</tt>. Then the next token in the string
	 * after the current position is returned. The current position is
	 * advanced beyond the recognized token.  The new delimiter set
	 * remains the default after this call.
	 *
	 * @param      delim   the new delimiters.
	 * @return     the next token, after switching to the new delimiter set.
	 * @exception  NoSuchElementException  if there are no more tokens in this
	 *               tokenizer's string.
	 */

    public String NextToken(String delim)
    {
        delimiters = delim;
        return NextToken();
    }

    /**
	 * Returns the same value as the <code>hasMoreTokens</code>
	 * method. It exists so that this class can implement the
	 * <code>Enumeration</code> interface.
	 *
	 * @return  <code>true</code> if there are more tokens;
	 *          <code>false</code> otherwise.
	 * @see     java.util.Enumeration
	 * @see     java.util.StringTokenizer#hasMoreTokens()
	 */

    public bool HasMoreElements()
    {
        return HasMoreTokens();
    }

    /**
	 * Returns the same value as the <code>nextToken</code> method,
	 * except that its declared return value is <code>Object</code> rather than
	 * <code>String</code>. It exists so that this class can implement the
	 * <code>Enumeration</code> interface.
	 *
	 * @return     the next token in the string.
	 * @exception  NoSuchElementException  if there are no more tokens in this
	 *               tokenizer's string.
	 * @see        java.util.Enumeration
	 * @see        java.util.StringTokenizer#nextToken()
	 */

    public Object NextElement()
    {
        return NextToken();
    }

    /**
	 * Calculates the number of times that this tokenizer's
	 * <code>nextToken</code> method can be called before it generates an
	 * exception. The current position is not advanced.
	 *
	 * @return  the number of tokens remaining in the string using the current
	 *          delimiter set.
	 * @see     java.util.StringTokenizer#nextToken()
	 */

    public int CountTokens()
    {
        int count = 0;
        int currpos = currentPosition;

        while (currpos < maxPosition)
        {
            /*
			 * This is just skipDelimiters(); but it does not affect
			 * currentPosition.
			 */
            while (!retTokens &&
                   (currpos < maxPosition) &&
                   (delimiters.IndexOf(str[currpos]) >= 0))
            {
                currpos++;
            }

            if (currpos >= maxPosition)
            {
                break;
            }

            int start = currpos;
            while ((currpos < maxPosition) &&
                   (delimiters.IndexOf(str[currpos]) < 0))
            {
                currpos++;
            }
            if (retTokens && (start == currpos) &&
                (delimiters.IndexOf(str[currpos]) >= 0))
            {
                currpos++;
            }
            count++;
        }
        return count;
    }
}
