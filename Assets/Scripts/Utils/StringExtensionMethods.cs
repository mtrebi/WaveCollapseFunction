using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringExtensionMethods {
  public static bool Contains(this string source, string toCheck, System.StringComparison comp) {
    return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
  }
}

