using System;
using CoAP;
using System.Collections.Generic;

namespace LEDBridge
{
    public class Output
    {
        Client mClient;
        Device mDevice;
        uint mIndex;
        List<Segment> mSegments;

        public Output(Client client, Device device, uint index)
        {
            mClient = client;
            mDevice = device;
            mIndex = index;
            mSegments = new List<Segment>();
        }

        public void updateFromDevice()
        {
            Request request = Request.NewGet();
            request.URI = new Uri($"coap://{mDevice.EndPoint.ToString()}/outputs/{mIndex}");

            request.Send();

            Response r = request.WaitForResponse(2000);
            if (r == null) throw new TimeoutException();
            if (r.Code != Code.Content) throw new Client.CoAPCodeException(r.Code, r.CodeString);

            if (r.Payload == null || r.PayloadSize == 0) return;

            LEDBridgeProto.OutputConfig outputConfig =
                    LEDBridgeProto.OutputConfig.Parser.ParseFrom(r.Payload);

            foreach (LEDBridgeProto.SegmentConfig segment in outputConfig.Segments)
            {
                Group group = mClient.segmentDiscovered(segment.Id, segment.Name, segment.Length, segment.Cpp);
                mSegments.Add(new Segment(this, group));
            }
        }
    }
}
