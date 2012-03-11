using System.IO;
using System.Text;

namespace SimpleOT.IO
{
    public class PropertyWriter : BinaryWriter
    {
        public PropertyWriter(Stream stream)
            : base(stream) { }

        public override void Write(string value)
        {
            Write((ushort)value.Length);
            Write(Encoding.Default.GetBytes(value));
        }
    }
}
