using System.Management;

namespace UsersDetails
{
    class Program
    {
        public static void Main(String[] args)
        {
            ManagementObjectSearcher srch = new ManagementObjectSearcher("root\\CIMV2", "select * from win32_useraccount");
            foreach (ManagementObject mObj in srch.Get())
            { 
                string username = mObj["name"]+Environment.NewLine;
                Console.WriteLine("Logged in user: " + username);
            }
        }
    }
}


