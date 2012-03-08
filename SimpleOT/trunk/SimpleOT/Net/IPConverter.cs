using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SimpleOT.Net
{
    public static class IPConverter
    {
        public static byte[] GetBytes(uint value)
        {
            return BitConverter.GetBytes(value);
        }

        public static uint ToUInt32(string value)
        {
            if (value == null)
                return 0;

            var ipBytes = value.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(byte.Parse).ToArray();
            return ipBytes.Length != 4 ? 0 : BitConverter.ToUInt32(ipBytes, 0);
        }

        public static uint ToUInt32(EndPoint endPoint)
        {
            if (endPoint == null)
                return 0;

            var address = endPoint.ToString();
            
            //remove the port
            var indexOf = address.IndexOf(':');
            if(indexOf != -1)
                address = address.Substring(0, indexOf);

            return ToUInt32(address);
        }

        public static string ToString(uint value)
        {
            var bytes = BitConverter.GetBytes(value);
            return String.Join(".", bytes);
        }
    }
}
