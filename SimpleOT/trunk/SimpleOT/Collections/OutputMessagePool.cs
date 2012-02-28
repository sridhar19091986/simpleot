using System;
using System.Collections.Generic;
using System.Threading;
using SimpleOT.Net;
using SimpleOT.Threading;

namespace SimpleOT.Collections
{
	public class OutputMessagePool
	{
		private Stack<Message> _messages;
		private IList<Message> _autoSendMessages;
		private int _count;
		private int _minSize;
		private int _maxSize;
		private long _frameTime;
		
		public OutputMessagePool (int minSize, int maxSize)
		{
			_minSize = minSize;
			_maxSize = maxSize;
			
			_messages = new Stack<Message> (_maxSize);
			_autoSendMessages = new List<Message> ();
			
			_frameTime = DateTime.Now.Ticks;
		}
		
		public void ProcessEnqueueMessages ()
		{
			_frameTime = DateTime.Now.Ticks;
			
			lock (_autoSendMessages) {
				
				for (int i = _autoSendMessages.Count - 1; i >= 0; i--) {
					
					var message = _autoSendMessages [i];
					
					if (DateTime.Now.Ticks - _frameTime > Constants.MESSAGE_SEND_MAX_TIME)
						_autoSendMessages.RemoveAt (i);
					
					if (message.WritableBytes > Constants.MESSAGE_SEND_MIN_SIZE ||
					   _frameTime - message.FrameTime > Constants.MESSAGE_SEND_MIN_TIME) {
						message.Connection.Send (message);
						_autoSendMessages.RemoveAt (i);
					}
				}
			}
		}
		
		public Message Get (Connection connection, bool autoSend)
		{
			if (connection == null)
				throw new ArgumentNullException ("connection");
			
			lock (_messages) {
				var buffer = _count < _minSize || _messages.Count == 0 ? Create () : _messages.Pop ();
				return Configure (buffer, connection, autoSend);
			}
		}
		
		public void Put (Message message)
		{
			if (message == null)
				throw new ArgumentNullException ("message");
			
			lock (_messages) {
				if (_messages.Count < _maxSize)
					_messages.Push (message);
				else
					Dispose (message);
			}
		}
		
		public void PutSend(Message message)
		{
			if (message == null)
				throw new ArgumentNullException ("message");
			
			lock(_autoSendMessages)
				_autoSendMessages.Add(message);
		}
		
		private Message Configure (Message message, Connection connection, bool autoSend)
		{
			if (autoSend) {
				lock (_autoSendMessages)
					_autoSendMessages.Add (message);
			}
			
			message.WriterIndex = Constants.MESSAGE_HEADER_SIZE + Constants.ADLER_CHECKSUM_SIZE + 2;
			message.Connection = connection;
			message.FrameTime = _frameTime;
			
			return message;
		}
		
		private Message Create ()
		{
			_count++;
			return new Message ();
		}
				
		private void Dispose (Message value)
		{
			if (value != null)
				_count--;
		}
	}
}

