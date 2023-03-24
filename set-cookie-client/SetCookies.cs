using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SetCookies

{

    class Program

    {
        
        [DllImport("ieframe.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool IESetProtectedModeCookie(string url, string name, string data, int flags);

        [DllImport("wininet.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool InternetSetCookie(string url, string name, string data);


        static void Main(string[] args)
        {

        IESetProtectedModeCookie("http://bbc.co.uk", "BBCPGstat", "0%3A-; expires = Sat,01-Jan-2030 00:00:00 GMT; path=/", 0);
        IESetProtectedModeCookie("http://bbc.co.uk", "BBCPGstat", "0%3A-; expires = Sat,01-Jan-2030 00:00:00 GMT; path=/", 0x10);
        InternetSetCookie("http://bbc.co.uk", "BBCPGstat", "0%3A-; expires = Sat,01-Jan-2030 00:00:00 GMT; path=/");

        }
    }
    
}
