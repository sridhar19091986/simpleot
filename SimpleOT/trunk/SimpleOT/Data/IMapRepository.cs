using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleOT.Domain;

namespace SimpleOT.Data
{
    public interface IMapRepository
    {
        Map Load(string name);
        void Save(Map map);
    }
}
