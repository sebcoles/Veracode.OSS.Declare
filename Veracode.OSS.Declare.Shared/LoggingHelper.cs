using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Veracode.OSS.Declare.Language;

namespace Veracode.OSS.Declare.Shared
{
    public static class LoggingHelper
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetMyMethodName()
        {
            var st = new StackTrace(new StackFrame(1));
            return st.GetFrame(0).GetMethod().Name;
        }

        public static ILocalizationRepository GetLocalizationRepository(string localization)
            => new LocalizationRepository(localization);
    }
}
