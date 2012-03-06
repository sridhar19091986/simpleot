using System;

namespace SimpleOT.Data
{
	public class Character
	{
		private int _id;
		private string _name;
		
		public Character ()
		{
		}
		
		public int Id{get{return _id;}set{_id = value;}}
		public string Name{get{return _name;}set{_name = value;}}
	}
}

