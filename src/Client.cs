using System;
using CoAP;
using Google.Protobuf;
using System.Collections.Generic;

namespace LEDBridge
{
    public class Client
    {
        Dictionary<Guid, Device> mDevices;
        Dictionary<uint, Group> mGroups;

        public event EventHandler<Device> DeviceDiscovered;
        public event EventHandler<Group> GroupDiscovered;

        /* we need a lock otherwise responses from different devices will race */
        private System.Threading.Mutex mGroupsLock;

        public Client()
        {
            mDevices = new Dictionary<Guid, Device>();
            mGroups = new Dictionary<uint, Group>();
            mGroupsLock = new System.Threading.Mutex();
        }

        public void discover()
        {
            Request request = new Request(Method.GET, false);
            request.URI = new Uri("coap://239.255.0.0/");
            request.AckTimeout = -1; // no retransmission
            request.Respond += delegate (Object sender, ResponseEventArgs eventargs)
            {
                // NON requests never time out
                System.Diagnostics.Debug.Assert(eventargs.Response != null);
                // Console.WriteLine(CoAP.Util.Utils.ToString(e.Response));

                try
                {
                    Device device = processDevice(eventargs.Response);
                    processGroups(device);
                }
                catch (InvalidProtocolBufferException e)
                {
                    Console.WriteLine(e.ToString());
                }
                catch (System.IO.InvalidDataException e)
                {
                    Console.WriteLine(e.ToString());
                }

            };
            request.Multicast = true;
            request.Send();
        }

        public Group segmentDiscovered(uint groupId, string name, uint length, uint cpp)
        {
            mGroupsLock.WaitOne();

            Group group;

            if (mGroups.ContainsKey(groupId))
            {
                group = mGroups[groupId];
                group.Update(name: name, length: length, cpp: cpp);
            }
            else
            {
                group = new Group(groupId, name, length, cpp);
                mGroups.Add(group.id, group);
                EventHandler<Group> handler = GroupDiscovered;
                if (handler != null) handler(this, group);
            }
            mGroupsLock.ReleaseMutex();
            return group;
        }

        private Device processDevice(Response response)
        {
            LEDBridgeProto.DeviceInfo deviceInfo =
                    LEDBridgeProto.DeviceInfo.Parser.ParseFrom(response.Payload);

            if (deviceInfo == null)
                throw new System.IO.InvalidDataException("Invalid payload");
            if (deviceInfo.Uuid == null)
                throw new System.IO.InvalidDataException("UUID is null");

            Guid uuid;
            try
            {
                uuid = Guid.Parse(deviceInfo.Uuid);
            }
            catch (System.FormatException)
            {
                throw new System.IO.InvalidDataException("Invalid UUID");
            }

            Device device;
            if (mDevices.ContainsKey(uuid))
            {
                device = mDevices[uuid];
                device.Update(
                    endPoint: response.Source,
                    name: deviceInfo.Name,
                    rtt: response.RTT);
            }
            else
            {
                device = new Device(this, uuid, response.Source, deviceInfo.Name, response.RTT, deviceInfo.Outputs);
                mDevices[uuid] = device;
                EventHandler<Device> handler = DeviceDiscovered;
                if (handler != null) handler(this, device);
            }

            return device;
        }

        public void processGroups(Device device)
        {
            foreach (Output output in device.Outputs)
            {
                output.updateFromDevice();
            }
        }

        [System.Serializable]
        public class CoAPException : System.Exception
        {
            public CoAPException() { }
            public CoAPException(string message) : base(message) { }
            public CoAPException(string message, System.Exception inner) : base(message, inner) { }
            protected CoAPException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        [System.Serializable]
        public class CoAPCodeException : CoAPException
        {
            public int ResponseCode;
            public CoAPCodeException(int code) { ResponseCode = code; }
            public CoAPCodeException(int code, string message) : base(message) { ResponseCode = code; }
            public CoAPCodeException(int code, string message, System.Exception inner) : base(message, inner) { ResponseCode = code; }
            protected CoAPCodeException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }

        [System.Serializable]
        public class CoAPPayloadException : CoAPException
        {
            public CoAPPayloadException() { }
            public CoAPPayloadException(string message) : base(message) { }
            public CoAPPayloadException(string message, System.Exception inner) : base(message, inner) { }
            protected CoAPPayloadException(
                System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
        }
    }
}
