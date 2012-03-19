using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
    public class MapFileRepository : IMapRepository
    {
        private IItemTypeRepository _itemTypeRepository;

        public MapFileRepository(IItemTypeRepository itemTypeRepository)
        {

        }

        public Map Load(string name)
        {

            return null;
        }

        public void Save(Map map)
        {
            throw new NotImplementedException();
        }
    }
}
