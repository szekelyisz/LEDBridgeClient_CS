using System;
using System.Threading.Tasks;

namespace LEDBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");
            Client client = new Client();
            client.DeviceDiscovered += DeviceDiscovered;
            client.GroupDiscovered += GroupDiscovered;

            client.discover();
            Console.WriteLine("Query sent");

            Task.Delay(System.Threading.Timeout.Infinite).Wait();

            // Console.WriteLine("Done.");
        }

        static void DeviceDiscovered(object sender, Device device)
        {
            Console.WriteLine(device.ToString());
        }

        static void GroupDiscovered(object sender, Group group)
        {
            Console.WriteLine($"{group.id} {group.Name} {group.Length}x{group.Cpp}");
        }
    }

}
