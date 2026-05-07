using System.Diagnostics;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Collections.Generic;

namespace Bloxstrap.Utility
{
    internal static class NetworkOptimizer
    {
        public static void SetFastDNS()
        {
            // find all active ethernet and wi-fi adapters
            var interfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && 
                              (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet || 
                               nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                .Select(nic => nic.Name)
                .ToList();

            if (!interfaces.Any()) return;

            var commands = new List<string>();

            foreach (var name in interfaces)
            {
                // We use double quotes for interface names in case they have spaces
                commands.Add($"netsh interface ipv4 set dns name=\"{name}\" static 1.1.1.1 primary");
                commands.Add($"netsh interface ipv4 add dns name=\"{name}\" 1.0.0.1 index=2");
            }

            commands.Add("ipconfig /flushdns");

            // Combine with && so it only proceeds if the previous command worked
            string combinedArgs = "/c " + string.Join(" && ", commands);

            RunElevatedCmd(combinedArgs);
        } // Added missing closing brace for the method

        private static void RunElevatedCmd(string arguments)
        {
            try 
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = arguments,
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
            catch (Win32Exception)
            {
                App.Logger.WriteLine("NetworkOptimizer", "User declined UAC. DNS was not changed.");
            }
        }
    }
}