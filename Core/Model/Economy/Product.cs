using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAge.Core.Model.Economy
{
    public class Product
    {
        public string Name { get; set; } = "Default";
        public Type GetType
        { 
            get
            {
                return this.GetType();
            }
        } 
        public float Count = 1f;
    }

    public class Book : Product
    {
        public Book()
        {
            Name = "Book";
        }
    }

    public class Food : Product
    {
        public Food()
        {
            Name = "Food";
        }
    }
}
