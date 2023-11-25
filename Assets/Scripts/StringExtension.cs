using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class StringExtension
{
    private static TextInfo myTI = new CultureInfo("en-US",false).TextInfo;
    public static string ToTitleCase(this string _string)
    {
        return myTI.ToTitleCase(_string);
    }
}
