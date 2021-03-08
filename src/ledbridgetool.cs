using System;
using System.Threading.Tasks;

namespace LEDBridge
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();

            client.DeviceDiscovered += DeviceDiscovered;
            client.GroupDiscovered += GroupDiscovered;

            client.discover();

            Task.Delay(System.Threading.Timeout.Infinite).Wait();
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
