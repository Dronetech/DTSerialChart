using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace DTSerialChart
{
    public partial class Form1 : Form
    {
        int x;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            cmbPort.DataSource = ports;
           
        }

        private void btnStart_Click(object sender, EventArgs e)
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

            var port = new SerialPort(cmbPort.SelectedText);
            port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            if (!port.IsOpen)
            {
                port.BaudRate = Convert.ToInt16(cmbBaud.SelectedText);
                port.Open();              
            }

        }

        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string line = ((SerialPort)sender).ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), line);
        }

        private delegate void LineReceivedEvent(string line);

        private void LineReceived(string Line)
        {
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

    }
}
