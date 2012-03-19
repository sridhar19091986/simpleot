using System;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
	public interface IAccountRepository
	{
		Account Load(string name);
        void Save(Account account);
	}
}

