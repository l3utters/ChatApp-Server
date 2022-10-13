using System;
using System.Net.Sockets;

namespace ServerTest
{

    public class User
    {
        public Socket Connection { get; set; }
        public string Name { get; set; }

        public User(string n, Socket a)
        {
            Name = n;
            Connection = a;
        }

        public Socket GetSocket()
        {
            return this.Connection;
        }

        public Boolean GetSocket(Socket test)
        {
            if (this.Connection == test)
                return true;
            else
                return false;
        }

        public Boolean GetName(string temp)
        {
            if (this.Name.CompareTo(temp) == 0)
                return true;
            else
                return false;
        }
    }


}
