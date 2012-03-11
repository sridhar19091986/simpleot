using System;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
	public interface IItemTypeRepository
	{
		ItemType Get(ushort id);
		ItemType GetByClientId(ushort id);
	}
}

