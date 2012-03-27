using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
    public interface IMapRepository
    {
        void Load(Map map, string fileName);
        void Save(Map map, string fileName);
    }
}
