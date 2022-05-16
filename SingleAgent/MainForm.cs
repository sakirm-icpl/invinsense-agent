using Newtonsoft.Json;
using RestSharp;
using SingleAgent.Monitor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace SingleAgent
{
    public partial class MainForm : Form
    {
        private readonly Image NotFoundImage = Properties.Resources.gray;
        private readonly Image ErrorImage = Properties.Resources.red;
        private readonly Image WarnImage = Properties.Resources.yellow;
        private readonly Image OkImage = Properties.Resources.green;

        private readonly ExtendedServiceController Dbytes = new ExtendedServiceController("DBytesService");
        private readonly ExtendedServiceController wazuh = new ExtendedServiceController("WazuhSvc");
        private readonly ExtendedServiceController Sysmon = new ExtendedServiceController("Sysmon64");
        private readonly ExtendedServiceController Dejavu = new ExtendedServiceController("Spooler");
        private readonly ExtendedServiceController Invinsesne = new ExtendedServiceController("Invinsense3.0");
       
        private readonly Dictionary<string, bool> _healthStatus = new Dictionary<string, bool>
        {
            {"Deceptive Bytes", true },
            {"Windows Defender", true },
            {"Wazuh", true },
            {"Microsoft Sysmon", true },
            {"Lateral Movement Protection", true },
            {"Invinsense3.0",true }
        };

        public MainForm()
        {
            InitializeComponent();

            Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            wazuh.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Wazuh", pbWazuh, e.Status);
            Dbytes.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Deceptive Bytes", pbDbytes, e.Status);
            Sysmon.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Microsoft Sysmon", pbSysmon, e.Status);
            Dejavu.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Lateral Movement Protection", pbDejavu, e.Status);
            // Invinsesne.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatus("Invinsense3.0", pbInvinsense, e.Status);
            Invinsesne.StatusChanged += (object sender, ServiceStatusEventArgs e) => UpdateStatusInvinsense("Invinsense3.0",e.Status);
            Invinsesne.StatusChanged += (object sender, ServiceStatusEventArgs e) => callApi("Invinsense3.0", e.Status);

            UpdateStatus("Wazuh", pbWazuh, wazuh.Status, false);
            UpdateStatus("Deceptive Bytes", pbDbytes, Dbytes.Status, false);
            UpdateStatus("Microsoft Sysmon", pbSysmon, Sysmon.Status, false);
            UpdateStatus("Dejavu", pbDejavu, Dejavu.Status, false);
            UpdateStatusInvinsense("Invinsense3.0",Invinsesne.Status,false);
           

            AvCheckTick(tmAvCheck, new EventArgs());

            MouseDownFilter mouseFilter = new MouseDownFilter(this);
            mouseFilter.FormClicked += FormClicked;
            Application.AddMessageFilter(mouseFilter);
        }

        private void UpdateStatusInvinsense(string name, ServiceControllerStatus? status, bool showNotification = true)
        {
           //throw new NotImplementedException();
            var message = $"Status Changed:" + status.ToString();
            var icon = ToolTipIcon.Warning;

            if (status == null)
            {
                message = "Not found";
                icon = ToolTipIcon.Warning;
                
            }
            else if (status == ServiceControllerStatus.Running)
            {
                message = "Back to normal";
                icon = ToolTipIcon.Info;
                notifyIcon.Visible = true;
                callApi("Invinsense3.0", Invinsesne.Status);

            }
            else if (status == ServiceControllerStatus.Stopped)
            {
                message = "Service stopped";
                icon = ToolTipIcon.Error;
                notifyIcon.Visible= false;
                ServiceControllerStatus=ServiceControllerStatus.Stopped;
                callApi("Invinsense3.0", Invinsesne.Status);

            }
            else
            {
                this.Close();
            }
            if (showNotification)
            {
                notifyIcon.ShowBalloonTip(5000, name, message, icon);
            }
           

            UpdateNotificationIcon(name, icon == ToolTipIcon.Info);
        }
        public void callApi(string name, ServiceControllerStatus? status)
        {
            string currtime = DateTime.Now.ToString("hh:mm:ss.fff tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            string url= "localhost:5000/ser/" + "/" + name + "/" + status + "/" + "/" + currtime;
            //WebRequest.Create(url);
            Console.WriteLine(name);
            Console.WriteLine(status);
            Console.WriteLine(currtime);
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
       //     request.AddHeader("Cookie", "session=eyJvaWRjX2NzcmZfdG9rZW4iOiJGZ1pLRElOTGJWb0ZkVzBZam5vVEtzM3VqU0dmVE5GMyJ9.Yn3zBQ.lU1M8o8Bany_NWAhw41kOMAJpSI");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Timeout = 900000;
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
               // streamWriter.Write(JsonConvert.SerializeObject(model));
                streamWriter.Flush();
                streamWriter.Close();
            }
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;

                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);
                    string result = streamReader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void UpdateStatus(string name, PictureBox pictureBox, ServiceControllerStatus? status, bool showNotification = true)
        {
            var message = $"Status Changed:" + status.ToString();
            var icon = ToolTipIcon.Warning;

            if (status == null)
            {
                message = "Not found";
                icon = ToolTipIcon.Warning;
                pictureBox.Image = NotFoundImage;
            }
            else if (status == ServiceControllerStatus.Running)
            {
                message = "Back to normal";
                icon = ToolTipIcon.Info;
                pictureBox.Image = OkImage;
            }
            else if (status == ServiceControllerStatus.Stopped)
            {
                message = "Service stopped";
                icon = ToolTipIcon.Error;
                pictureBox.Image = ErrorImage;
            }
            else
            {
                pictureBox.Image = WarnImage;
            }

            if (showNotification)
            {
                notifyIcon.ShowBalloonTip(5000, name, message, icon);
            }

            UpdateNotificationIcon(name, icon == ToolTipIcon.Info);
        }

        private void UpdateNotificationIcon(string name, bool status)
        {
            _healthStatus[name] = status;

            if(_healthStatus.Any(x=> !x.Value))
            {
                notifyIcon.Icon = Properties.Resources.red_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Not all services are healthy";
            }
            else
            {
                notifyIcon.Icon = Properties.Resources.green_logo_22_22;
                notifyIcon.Text = "Invinsense 3.0 - Healthy"; 
            }
        }

        private string avLastStatus = "";

        public ServiceControllerStatus ServiceControllerStatus { get; private set; }

        private void AvCheckTick(object sender, EventArgs e)
        {
            var status = ServiceHelper.AVStatus("Windows Defender");

            if (avLastStatus == status)
            {
                return;
            }

            avLastStatus = status;

            var message = "Not found";
            var icon = ToolTipIcon.Error;

            switch (status)
            {
                case "Enabled":
                    pbDefender.Image = OkImage;
                    message = "Healthy";
                    icon = ToolTipIcon.Info;
                    break;
                case "Need Update":
                    pbDefender.Image = WarnImage;
                    message = "Out of date";
                    icon = ToolTipIcon.Warning;
                    break;
                case "Disabled":
                    pbDefender.Image = ErrorImage;
                    message = "Disabled";
                    icon = ToolTipIcon.Error;
                    break;
                default:
                    pbDefender.Image = NotFoundImage;
                    break;
            }

            notifyIcon.ShowBalloonTip(5000, "Windows Defender", message, icon);
        }

        private void MainFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void FormClicked(object sender, EventArgs e)
        {
            Hide();
        }

        private void ShowClick(object sender, EventArgs e)
        {
            BringToTop();
        }

        private void BringToTop()
        {
            //Checks if the method is called from UI thread or not
            if (InvokeRequired)
            {
                Invoke(new Action(BringToTop));
            }
            else
            {
                Visible = true;

                WindowState = FormWindowState.Normal;

                //Keeps the current topmost status of form
                bool top = TopMost;

                //Brings the form to top
                TopMost = true;

                //Set form's topmost status back to whatever it was
                TopMost = top;
            }
        }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
           int nLeftRect,     // x-coordinate of upper-left corner
           int nTopRect,      // y-coordinate of upper-left corner
           int nRightRect,    // x-coordinate of lower-right corner
           int nBottomRect,   // y-coordinate of lower-right corner
           int nWidthEllipse, // width of ellipse
           int nHeightEllipse // height of ellipse
        );

     }

}