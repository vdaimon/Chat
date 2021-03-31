using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFClient
{
    public class ConnectedClient
    {
        public string Name { get; }
        public bool NewMessageFlag { get; set; }

        public ConnectedClient(string name)
        {
            Name = name;
            NewMessageFlag = false;
        }
    }
}
