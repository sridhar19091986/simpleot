using System;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
	public interface IPlayerRepository
	{
		Player Load(string name);
	}
}

