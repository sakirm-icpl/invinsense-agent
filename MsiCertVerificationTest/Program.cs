using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System;

namespace MsiCertVerificationTest
{
    internal class Program
    {
        static void Main()
        {
            ShowCertificateInfo("../../../artifacts/wazuh/wazuh-agent-4.3.9-1.msi");
            ShowCertificateInfo("../../../artifacts/osquery/osquery-5.5.1.msi");
            ShowCertificateInfo("../../../artifacts/dbytes/DeceptiveBytes.EPS.x64.msi");

            Console.WriteLine("Click to close");
            Console.ReadLine();
        }

        static void ShowCertificateInfo(string msiPath)
        {
            try
            {
                X509Certificate x509 = X509Certificate.CreateFromSignedFile(msiPath);
                if (x509.GetHashCode() != 0)
                {
                    Console.WriteLine(x509.ToString(true));
                }
                else
                {
                    Console.WriteLine("Assembly isn't signed by a software publisher certificate");
                }
            }
            catch (COMException ce)
            {
                // using a test certificate without trusting the test root ?
                Console.WriteLine(ce.Message);

            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
