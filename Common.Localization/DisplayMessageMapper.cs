using Common.Localization;
using Common.Models;
using System.Linq;

namespace Common.Mappers
{
    public static class DisplayMessageMapper
    {
        public static DisplayMessage MapNetworkIsolationMessage(params string[] groups)
        {
            var title = "Network Disconnected";
            var message = "Your device administrator has disconnected your device from all of the networks for security reasons. Please contact your local IT Help Desk for assistance";

            if (groups.Any(x => x == "malaysia"))
            {
                title = "Rangkaian Terputus";
                message = string.Concat("Pentadbir peranti anda telah memutuskan sambungan peranti anda daripada semua rangkaian organisasi atas sebab keselamatan. Sila hubungi pusat sokongan IT anda untuk mendapatkan bantuan.", "\r\n\r\n", message);
            }
            else if (groups.Any(x => x == "thailand"))
            {
                title = "แจ้งระงับการเชื่อมต่อระบบของนินจาแวนทั้งหมด";
                message = string.Concat("ผู้ดูแลอุปกรณ์ของคุณได้ตัดการเชื่อมต่อของอุปกรณ์ของคุณออกจากเครือข่ายขององค์กรทั้งหมดเพื่อเหตุผลด้านความปลอดภัย\r\nกรุณาติดต่อแผนก IT เพื่อขอรับความช่วยเหลือ\r\n\r\n", message);
            }
            else if (groups.Any(x => x == "indonesia"))
            {
                title = "Jaringan Terputus";
                message = string.Concat("Administrator telah memutuskan koneksi perangkat anda pada semua jaringan milik perusahaan untuk alasan keamanan. Harap hubungi lokal IT untuk mendapatkan bantuan lebih lanjut.\r\n\r\n", message);
            }
            else if (groups.Any(x => x == "philippines"))
            {
                title = "Nadiskonekta ang Network";
                message = string.Concat("Nadiskonekta ng administrator ng iyong device ang iyong device mula sa lahat ng network ng organisasyon para sa mga kadahilanang panseguridad. Mangyaring makipag-ugnayan sa iyong lokal na IT Help Desk para sa tulong\r\n\r\n", message);
            }
            else if (groups.Any(x => x == "vietnam"))
            {
                title = "Mất kết nối mạng";
                message = string.Concat("Quản trị viên đã ngắt kết nối các thiết bị của bạn khỏi tất cả các mạng của tổ chức vì lý do bảo mật. Vui lòng liên hệ IT Help Desk để được hỗ trợ.\r\n\r\n", message);
            }

            return new DisplayMessage
            {
                MessageType = NotifyType.Error,
                Title = title,
                Message = message
            };
        }
    }
}
