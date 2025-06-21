using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotAge.Core.Model.Economy;
using DotAge.Core.View;

namespace DotAge.Core.Model.Region
{
    public class Region
    {
        public string Name { get; set; } = "Default";

        public Dictionary<Type, ProductValue> StoreProductIndex = new Dictionary<Type, ProductValue>();
        public string GetAllProductInfo
        {
            get
            {
                return string.Join("\n", StoreProductIndex.Select(x => x.Key.Name + " : " + x.Value.Price.ToString() + "c"));
            }
        }

        public bool Crash(int _entityID)
        {
            //GameData
            return true;
        }
    }

    public class City : Region
    {


    }

    class Kingdom : City
    {
        public Kingdom()
        {

        }
    }
}
