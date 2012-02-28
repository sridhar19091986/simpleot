using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Xml.Linq;
using SimpleOT.Commons.Scripting;

namespace SimpleOT.Commons
{
	public static class Extensions
	{
		private static Random _random = new Random ();

        #region Event

		public static void Raise<T> (this EventHandler handler, object sender, T eventArgs) where T : EventArgs
		{
			if (handler != null)
				handler (sender, eventArgs);
		}

		public static void Raise<T> (this EventHandler<T> handler, object sender, T args) where T : EventArgs
		{
			if (handler != null)
				handler (sender, args);
		}

        #endregion

        #region DateTime

		public static long Milliseconds (this DateTime dateTime)
		{
			return dateTime.Ticks / 10000;
		}

        #endregion

        #region Xml

		public static ushort GetUInt16 (this XAttribute attribute)
		{
			if (attribute == null)
				return 0;

			ushort value;
			ushort.TryParse (attribute.Value, out value);
			return value;
		}

		public static short GetInt16 (this XAttribute attribute)
		{
			if (attribute == null)
				return 0;

			short value;
			short.TryParse (attribute.Value, out value);
			return value;
		}

		public static int GetInt32 (this XAttribute attribute)
		{
			if (attribute == null)
				return 0;

			int value;
			int.TryParse (attribute.Value, out value);
			return value;
		}

		public static uint GetUInt32 (this XAttribute attribute)
		{
			if (attribute == null)
				return 0;

			uint value;
			uint.TryParse (attribute.Value, out value);
			return value;
		}

		public static float GetFloat (this XAttribute attribute)
		{
			if (attribute == null)
				return 0;

			float value;
			float.TryParse (attribute.Value, out value);
			return value;
		}

		public static string GetString (this XAttribute attribute)
		{
			if (attribute == null)
				return "";

			return attribute.Value;
		}

        #endregion

        #region List
		public static void Shuffle<T> (this IList<T> list, int offset)
		{
			var n = list.Count;
			while (n > offset) {
				n--;
				var k = _random.Next (n + 1);
				var value = list [k];
				list [k] = list [n];
				list [n] = value;
			}
		}

		public static bool IsEmpty<T> (this IList<T> list)
		{
			return list == null || list.Count == 0;
		}

        #endregion

        #region Database

		public static byte[] GetBinary (this DbDataReader reader, string name)
		{
			var value = reader [name];

			if (value is byte[])
				return (byte[])value;

			return null;
		}

		public static int GetInt32 (this DbDataReader reader, string name)
		{
			var value = reader [name];

			if (value is int)
				return (int)value;

			return 0;
		}

		public static long GetInt64 (this DbDataReader reader, string name)
		{
			var value = reader [name];

			if (value is long)
				return (long)value;

			return 0;
		}

		public static string GetString (this DbDataReader reader, string name)
		{
			var value = reader [name];

			if (value is string)
				return (string)value;

			return string.Empty;
		}

        #endregion
	}
}

