using System.IO;
using System.Text;

namespace SimpleOT.IO
{
    public class PropertyReader : BinaryReader
    {
        public PropertyReader(Stream stream)
            : base(stream) { }

        public string GetString()
        {
            var len = ReadUInt16();
            return Encoding.Default.GetString(ReadBytes(len));
        }
    }
}