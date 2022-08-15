﻿using Common;

namespace AgentService
{
    partial class SingleAgentService
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // SingleAgentService
            // 
            this.ServiceName = Constants.SingleAgentName;
            this.EventLog.Log = Constants.LogGroupName;
            this.EventLog.Source = Constants.SingleAgentName;

        }

        #endregion
    }
}
