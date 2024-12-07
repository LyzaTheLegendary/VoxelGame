using System.Net;

namespace Network.Utils
{
    public struct Addr
    {
        public string Address { get; init; }
        public int Port { get; init; }
        public Addr(string host)
        {
            if(!host.Contains(':'))
                throw new ArgumentException("Invalid host format");

            string[] strInfo = host.Split(':');

            Address = strInfo[0];
            Port = int.Parse(strInfo[1]);
        }

        public Addr(IPAddress address, int port)
        {
            string[] strInfo = address.ToString().Split(':');
            Address = strInfo[0];
            Port = port;
        }
    }
}
