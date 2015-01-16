using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.RegistrySerialization
{
    public interface ISerializer
    {
        void Serialize(object obj);
        object Deserialize(Type objType);
    }
}
