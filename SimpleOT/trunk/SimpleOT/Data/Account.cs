using System;
using System.Collections.Generic;

namespace SimpleOT.Data
{
	public class Account
	{
		private int _id;
		private string _login;
		private string _password;
		private string _email;
		private IList<Character> _characters;
		
		public Account ()
		{
			this._characters = new List<Character>();
		}
		
		public int Id{get{return _id;}set{_id = value;}}
		public string Login{get{return _login;}set{_login = value;}}
		public string Password{get{return _password;}set{_password = value;}}
		public string Email{get{return _email;}set{_email = value;}}
		public IList<Character> Characters{get{return _characters;}}
	}
}

