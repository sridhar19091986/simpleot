using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleOT.Commons.Net
{
    public abstract class SimpleHandler : IHandler
    {
        protected Connection Connection { get; private set; }

        protected abstract bool ParsePacket(Message message);

        #region IProtocol implementation
        public virtual void ConnectionOpen(Connection connecion)
        {
            this.Connection = connecion;
        }

        public virtual void ConnectionClosed(Connection connection)
        {
            this.Connection = null;
        }

        public virtual void ExceptionCaught(Connection connection, Exception exception)
        {
            Console.WriteLine("Error: " + exception.Message);
            Connection.Close();
        }

        public virtual void MessageReceived(Connection connection, Message message)
        {
            if (message.ReadableBytes < 2)
                return;

            message.MarkReaderIndex();
            int length = message.GetUShort();

            if (length + 2 > message.Capacity)
                throw new Exception("Corrupted packet, the packet size must not exceed " + message.Capacity);

            if (message.ReadableBytes < length)
            {
                message.ResetReaderIndex();
                return;
            }

            while (message.ReaderIndex < length + 2)
            {
                if (!ParsePacket(message))
                {
                    message.ReaderIndex = length + 2;
                    break;
                }
            }

            if (message.ReadableBytes > 0)
                message.DiscardReadBytes();
            else
                message.Clear();
        }

        public virtual void MessageSent(Connection connection, Message message)
        {
            message.PutUShort(message.ReaderIndex, (ushort)(message.WritableBytes - 2));
        }
		
		public virtual void MessageReceiveTimeout(Connection connection) 
		{
			
		}
		
		public virtual void MessageSendTimeout(Connection connection)
		{
			
		}
        #endregion
    }
}
