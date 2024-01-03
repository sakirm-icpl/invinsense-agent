using Serilog;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Common.FileHelpers
{
    public static class X509CertificateHelpers
    {
        private static readonly ILogger logger = Log.ForContext(typeof(X509CertificateHelpers));

        public static string GetAttachedCertificate(string exePath)
        {
            try
            {
                var x509 = X509Certificate.CreateFromSignedFile(exePath);
                if (x509.GetHashCode() != 0)
                    return x509.ToString(true);
                else
                    logger.Information("Assembly isn't signed by a software publisher certificate");
            }
            catch (COMException ce)
            {
                // using a test certificate without trusting the test root ?
                logger.Error(ce.Message);
            }
            catch (CryptographicException e)
            {
                logger.Error(e.Message);
            }

            return null;
        }
    }
}