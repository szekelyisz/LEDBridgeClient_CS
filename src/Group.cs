using System.Drawing;
using System.Net.Sockets;
using System.Net;
namespace LEDBridge
{
    public class Group
    {
        public readonly uint id;
        private string mName;
        public string Name => mName;
        private uint mLength;
        public uint Length => mLength;
        private uint mCpp;
        public uint Cpp => mCpp;

        private Socket mSocket;

        public Group(uint id, string name, uint length, uint cpp)
        {
            System.Diagnostics.Debug.Assert(id > 0 && id < 65536);
            this.id = id;
            mName = name;
            mLength = length;
            mCpp = cpp;
            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mSocket.Connect(IPAddress.Parse($"239.255.{id / 255}.{id % 255}"), 5000);
        }

        public void Update(string name, uint length, uint cpp)
        {
            mName = name;
            mLength = length;
            mCpp = cpp;
        }

        void send(Color[] pixels)
        {
            byte[] buffer = new byte[mLength * mCpp];
            uint i = 0;
            uint npixels = System.Math.Min((uint)pixels.Length, mLength);

            for (uint p = 0; p != npixels; p++)
            {
                buffer[i++] = pixels[p].R;
                buffer[i++] = pixels[p].G;
                buffer[i++] = pixels[p].B;
                if (mCpp > 3) buffer[i++] = pixels[p].A;
            }

            mSocket.Send(buffer);
        }
    }
}