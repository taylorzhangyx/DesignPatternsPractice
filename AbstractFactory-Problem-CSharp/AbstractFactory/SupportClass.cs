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

public class SupportClass
{
    /*******************************/

    public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
    {
        stream.Write(throwable.StackTrace);
        stream.Flush();
    }

    /*******************************/

    public static int GetConstructorModifiers(System.Reflection.ConstructorInfo constructor)
    {
        int temp;
        if (constructor.IsPublic)
            temp = 1;
        else if (constructor.IsPrivate)
            temp = 2;
        else
            temp = 4;
        return temp;
    }

    /// Constructs a System.Uri object with the given URL. Behaves exactly like the constructor, with one
    /// addition: If the URL is missing a protocol, a UriFormatException is thrown. This makes for identical
    /// behaviour to Java's new URL(...)
    public static System.Uri CreateUri(string url)
    {
        if (url != null && !(url.StartsWith("file://") || url.StartsWith("http://") || url.StartsWith("localhost")))
            throw (new UriFormatException("Missing protocol"));

        return new System.Uri(url);
    }

    public static System.Uri CreateUri(System.Uri uri, string path)
    {
        return new System.Uri(uri, path);
    }
}
