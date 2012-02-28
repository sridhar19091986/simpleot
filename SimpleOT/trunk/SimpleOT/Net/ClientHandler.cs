using System;
using NLog;
using SimpleOT.Commons.Net;
using SimpleOT.Commons;
using SimpleOT.Commons.Threading;

namespace SimpleOT.Net
{
	public class ClientHandler : IHandler
	{
		private static Logger logger = LogManager.GetCurrentClassLogger ();
		private Connection _connection;
		private LoginServer _loginServer;
		private bool _firstMessageReceived;
		private uint[] _xteaKey;
		
		public ClientHandler (LoginServer loginServer)
		{
			if (loginServer == null)
				throw new ArgumentNullException ("loginServer");
			
			this._loginServer = loginServer;
		}
		
		public void Disconnect (string message = null)
		{
			if (_connection != null)
				_connection.Close ();
		}
		
        #region IProtocol implementation
		public void ConnectionOpen (Connection connecion)
		{
			logger.Debug ("Connection open.");
			this._connection = connecion;
			this._loginServer.ClientConnect (this);
		}

		public void ConnectionClosed (Connection connection)
		{
			logger.Debug ("Connection closed.");
			this._connection = null;
			this._loginServer.ClientDisconnected (this);
		}

		public void ExceptionCaught (Connection connection, Exception exception)
		{
			logger.ErrorException ("Connection error.", exception);
			Disconnect ();
		}

		public void MessageReceived (Connection connection, Message message)
		{
			logger.Debug ("Message received, readable bytes {0}", message.ReadableBytes);
			
			//Check if we got the full packet header
			if (message.ReadableBytes < 2)
				return;

			message.MarkReaderIndex ();
			int length = message.GetUShort ();

			if (length + 2 > message.Capacity)
				throw new Exception ("Corrupted packet, the packet size must not exceed " + message.Capacity);

			if (message.ReadableBytes < length) {
				message.ResetReaderIndex ();
				return;
			}
			
			byte protocolId = message.GetByte ();
			
			if (protocolId != 0x01) 
				throw new Exception ("Invalid login protocol id.");
			
			
			var checksum = Adler.Generate (message.Buffer, message.ReaderIndex + 
          		Constants.ADLER_CHECKSUM_SIZE, message.ReadableBytes);
			
			if (checksum != message.GetUInt ())
				throw new Exception ("Corrupted packet, invalid checksum value.");
				
			var clientOS = (ClientOS)message.GetUShort ();
			var clientVersion = (ClientVersion)message.GetUShort ();
			
			if (clientVersion != ClientVersion.V861)
				throw new Exception ("Invalid client version " + (int)clientVersion);
			
			message.ReaderIndex += 12;
			
			if (message.ReadableBytes < 128)
				throw new Exception ("Invalid RSA encrypted message.");
			
			Rsa.Decrypt (message, message.ReaderIndex, 128);
			
			_xteaKey = new uint[4];
			_xteaKey [0] = message.GetUInt ();
			_xteaKey [1] = message.GetUInt ();
			_xteaKey [2] = message.GetUInt ();
			_xteaKey [3] = message.GetUInt ();
			
			var login = message.GetString ();
			var password = message.GetString ();
			
			_loginServer.Dispatcher.Add (new Task (() => _loginServer.Login (this, login, password)));
			message.Clear ();
		}

		public void MessageSent (Connection connection, Message message)
		{
			if (_xteaKey == null)
				throw new Exception ("Null XTEA key.");
			
			
			
			message.PutUShort (message.ReaderIndex, (ushort)(message.WritableBytes - 2));
		}
		
		public virtual void MessageReceiveTimeout (Connection connection)
		{
			logger.Debug ("Receive timeout.");
			Disconnect ();
		}
		
		public virtual void MessageSendTimeout (Connection connection)
		{
			logger.Debug ("Send timeout.");
			Disconnect ();
		}
		#endregion
		
		public string RemoteAddress{ get { return _connection.RemoteAddress; } }
	}
}

