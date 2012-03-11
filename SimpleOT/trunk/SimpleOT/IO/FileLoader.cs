using System.IO;
using NLog;

namespace SimpleOT.IO
{
    public class FileLoader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        static class Constants
        {
            public const byte NODE_START = 0xFE;
            public const byte NODE_END = 0xFF;
            public const byte ESCAPE = 0xFD;
        }

        protected byte[] _buffer;
        protected FileStream _fileStream;
        protected BinaryReader _reader;
        protected FileLoaderNode _root;

        public FileLoaderNode GetRootNode()
        {
            return _root;
        }

        public bool OpenFile(string fileName)
        {
            _fileStream = File.Open(fileName, FileMode.Open);
            _reader = new BinaryReader(_fileStream);

            var version = _reader.ReadUInt32();

            if(version > 0)
            {
                logger.Error("Invalid file version.");
                _reader.Close();
                return false;
            }

            if(SafeSeek(4))
            {
                _root = new FileLoaderNode {Start = 4};

                if (_reader.ReadByte() == Constants.NODE_START)
                    return ParseNode(_root);
            }

            return false;
        }

        private bool ParseNode(FileLoaderNode node)
        {
            var currentNode = node;
            int val;

            while (true)
            {
                // read node type
                val = _fileStream.ReadByte();
                if (val != -1)
                {
                    
                    currentNode.Type = val;
                    var setPropSize = false;

                    while (true)
                    {
                        // search child and next node
                        val = _fileStream.ReadByte();

                        
                        if (val == Constants.NODE_START)
                        {
                            var childNode = new FileLoaderNode {Start = _fileStream.Position};
                            setPropSize = true;

                            currentNode.PropsSize = _fileStream.Position - currentNode.Start - 2;
                            currentNode.Child = childNode;

                            if (!ParseNode(childNode))
                                return false;
                        }
                        else if (val == Constants.NODE_END)
                        {
                            if (!setPropSize)
                                currentNode.PropsSize = _fileStream.Position - currentNode.Start - 2;

                            val = _fileStream.ReadByte();

                            if (val != -1)
                            {
                                if (val == Constants.NODE_START)
                                {
                                    // start next node
                                    var nextNode = new FileLoaderNode {Start = _fileStream.Position};
                                    currentNode.Next = nextNode;
                                    currentNode = nextNode;
                                    break;
                                }
                                
                                if (val == Constants.NODE_END)
                                {
                                    // up 1 level and move 1 position back
                                    // safeTell(pos) && safeSeek(pos)
                                    _fileStream.Seek(-1, SeekOrigin.Current);
                                    return true;
                                }
                                
                                // bad format
                                return false;

                            }

                            // end of file?
                            return true;
                        }
                        else if (val == Constants.ESCAPE)
                        {
                            _fileStream.ReadByte();
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private byte[] GetProps(FileLoaderNode node, out long size)
        {
            if (_buffer == null || _buffer.Length < node.PropsSize)
                _buffer = new byte[node.PropsSize];

            _fileStream.Seek(node.Start + 1, SeekOrigin.Begin);
            _fileStream.Read(_buffer, 0, (int)node.PropsSize);

            uint j = 0;
            var escaped = false;

            for (uint i = 0; i < node.PropsSize; ++i, ++j)
            {
                if (_buffer[i] == Constants.ESCAPE)
                {
                    ++i;
                    _buffer[j] = _buffer[i];
                    escaped = true;
                }
                else if (escaped)
                {
                    _buffer[j] = _buffer[i];
                }
            }
            size = j;
            return _buffer;
        }

        public bool GetProps(FileLoaderNode node, out PropertyReader props)
        {
            long size;
            var buff = GetProps(node, out size);
            
            if (buff == null)
            {
                props = null;
                return false;
            }

            props = new PropertyReader(new MemoryStream(buff, 0, (int)size));
            return true;
        }

        protected bool SafeSeek(long pos)
        {
            if (_fileStream == null || _fileStream.Length < pos)
                return false;

            return _fileStream.Seek(pos, SeekOrigin.Begin) == pos;
        }
    }
	
	public class FileLoaderNode
    {
        public long Start { get; set; }
        public long PropsSize { get; set; }
        public long Type { get; set; }
        public FileLoaderNode Next { get; set; }
        public FileLoaderNode Child { get; set; }
    }
}