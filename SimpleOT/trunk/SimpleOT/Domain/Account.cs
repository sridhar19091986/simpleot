using System;
using System.Collections.Generic;

namespace SimpleOT.Domain
{
    public class Account
    {
        private int _id;
        private string _name;
        private string _password;
        private long _premmiumEnd;
        private int _warnings;

        private IList<string> _characters;

        public Account()
        {
            this._characters = new List<string>();
        }

        public int Id { get { return _id; } set { _id = value; } }
        public string Name { get { return _name; } set { _name = value; } }
        public string Password { get { return _password; } set { _password = value; } }
        public long PremmiumEnd { get { return _premmiumEnd; } set { _premmiumEnd = value; } }
        public int Warnings { get { return _warnings; } set { _warnings = value; } }

        public IList<string> Characters { get { return _characters; } }

        public ushort PremiumDaysLeft
        {
            get
            {
                var now = DateTime.Now.Milliseconds();
                if (_premmiumEnd < now)
                    return 0;

                return (ushort)Math.Ceiling(((double)(_premmiumEnd - now)) / 86400.0);
            }
        }
    }
}

