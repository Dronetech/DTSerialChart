using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace DTSerialChart
{
    public partial class Form1 : Form
    {
        int x;
        Boolean started = false;
        SerialPort port; 
        public Form1()
        {
            Thread t = new Thread(new ThreadStart(SplashScreen));
            t.Start();
            Thread.Sleep(4000);
            InitializeComponent();
            t.Abort();
           
           
        }

        private void SplashScreen()
        {
            Application.Run(new SplashScreen());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            refreshSerialPorts();

            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            lblVersion.Text = "Version: " + fvi.FileVersion;
           
        }

        private void refreshSerialPorts()
        {
            var ports = SerialPort.GetPortNames();
            cmbPort.DataSource = ports;

            cmbBaud.SelectedIndex = 0;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!started)
            {
                chart1.Series.Clear();
                chart1.ChartAreas[0].AxisX.Minimum = 0;
                chart1.ChartAreas[0].AxisX.Maximum = Convert.ToInt16(txtScale.Text);
                chart1.ChartAreas[0].AxisY.Maximum = Convert.ToInt16(txtMax.Text);
                chart1.ChartAreas[0].AxisY.Minimum = Convert.ToInt16(txtMin.Text);


                foreach (var item in txtFormat.Text.Split(','))
                {
                    chart1.Series.Add(item.Split('|')[1]);
                    chart1.Series[item.Split('|')[1]].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                }

                port = new SerialPort(cmbPort.SelectedItem.ToString());
                port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
                if (!port.IsOpen)
                {
                    port.BaudRate = Convert.ToInt16(cmbBaud.SelectedItem.ToString());
                    port.Open();
                }
               cmbBaud.Enabled = cmbPort.Enabled= txtFormat.Enabled = txtMax.Enabled = txtMin.Enabled = txtScale.Enabled = btnRefresh.Enabled= false;
               started = true;
               btnStart.Text = "Stop";
            }
            else
            {
                if (port.IsOpen)
                {
                    port.Close();
                }

                cmbBaud.Enabled = cmbPort.Enabled = txtFormat.Enabled = txtMax.Enabled = txtMin.Enabled = txtScale.Enabled = btnRefresh.Enabled= true;
                x = 0;
                started = false;
                btnStart.Text = "Start";
            }

        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string line = ((SerialPort)sender).ReadLine();
                this.BeginInvoke(new LineReceivedEvent(LineReceived), line);

            }
            catch (Exception)
            {
                
               
            }
         
        }

        private delegate void LineReceivedEvent(string line);

        private void LineReceived(string Line)
        {
            txtLog.Text += Line;
            txtLog.Text += "\r\n";
            if (x >= Convert.ToInt16(txtScale.Text))
            {
                x = 0;
                foreach (var item in txtFormat.Text.Split(','))
                {
                    chart1.Series[item.Split('|')[1]].Points.Clear();
                }

            }

            foreach (var item in txtFormat.Text.Split(','))
            {
                chart1.Series[item.Split('|')[1]].Points.AddXY(x, Line.Split(',')[Convert.ToInt32(item.Split('|')[0])]);
            }

            x++;


        }

        private void btnSaveImage_Click(object sender, EventArgs e)
        {
            chart1.SaveImage(Guid.NewGuid().ToString()+".png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(goHome));
            t.Start();
          
        }

        private void goHome()
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(linkLabel1.Text);
            Process.Start(sInfo);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            refreshSerialPorts();
        }

    }
}
