using System;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
	public interface IAccountRepository
	{
		void Save(Account account);
		Account Load(string name);
	}
}

