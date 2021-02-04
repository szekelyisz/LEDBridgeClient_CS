using System;

namespace LEDBridge
{
    public class Device
    {
        private Client mClient;
        public readonly Guid mUUID;
        private String mName;
        private Output[] mOutputs;
        private double mRTT;
        private System.Net.EndPoint mEndPoint;
        private DateTime mLastSeen;

        public String Name => mName;
        public System.Net.EndPoint EndPoint => mEndPoint;
        public Output[] Outputs => mOutputs;

        public Device(Client client, Guid uuid, System.Net.EndPoint endPoint,
            string name,
            double rtt,
            uint outputs)
        {
            mClient = client;
            mUUID = uuid;
            mRTT = rtt;
            mName = name;
            mEndPoint = endPoint;
            mOutputs = new Output[outputs];
            mLastSeen = DateTime.UtcNow;

            for (uint c = 0; c != outputs; c++)
            {
                mOutputs[c] = new Output(mClient, this, c);
                mOutputs[c].updateFromDevice();
            }
        }

        public void Update(
            System.Net.EndPoint endPoint,
            string name,
            double rtt)
        {
            mRTT = rtt;
            mName = name;
            mEndPoint = endPoint;
            mLastSeen = DateTime.UtcNow;
        }

        public new string ToString() => $"{mUUID} {mName} {mEndPoint.ToString()}";
    }
}
