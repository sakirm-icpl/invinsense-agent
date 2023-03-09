using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HardwareInformation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void ProcId_Click(object sender, EventArgs e)
        {
            lblPid.Text = HardwareInfo.GetProcessorId();
        }

        private void HDDSNo_Click(object sender, EventArgs e)
        {
            lblHDD.Text = HardwareInfo.GetHDDSerialNo();
        }

        private void BoardMake_Click(object sender, EventArgs e)
        {
            lblBM.Text = HardwareInfo.GetBoardMaker();
        }

        private void BIOSMkr_Click(object sender, EventArgs e)
        {
            lblBios.Text = HardwareInfo.GetBIOSmaker();
        }

        private void PhysicalMem_Click(object sender, EventArgs e)
        {
            lblPM.Text = HardwareInfo.GetPhysicalMemory();
        }

        private void CPUSpeed_Click(object sender, EventArgs e)
        {
            lblCS.Text = HardwareInfo.GetCpuSpeedInGHz().ToString();
        }

        private void CPUMkr_Click(object sender, EventArgs e)
        {
            lblCM.Text = HardwareInfo.GetCPUManufacturer();
        }
    }
}
