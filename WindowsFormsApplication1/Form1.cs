using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        int samples = 45000;
        int amplitude = 20;
        GraphPane graph;
        Double[] x,y;
        bool changing = false;
        double startX, startY,endX,endY;
        double panStartX, panStartY, panEndX, panEndY;
        double panDeltaX, panDeltaY;
        bool mouseLeftpressed, mouseWheelPressed;
        Point mouseDown;

        public Form1()
        {
            InitializeComponent();
            this.timer1.Interval = 10;
            this.timer1.Start();
            this.trackBar1.ValueChanged += new System.EventHandler(trackBar1_ValueChanged);
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = samples;
            chart1.ChartAreas[0].AxisX.Interval = samples / 10;

            chart1.ChartAreas[0].AxisY.Interval = 4;
            chart1.ChartAreas[0].AxisY.Minimum = -20;
            chart1.ChartAreas[0].AxisY.Maximum = 20;
            x = new double[samples];
            y = new double[samples];
            chart1.Series["Series1"].Points.DataBindXY(x, y);
            chart1.MouseWheel += new System.Windows.Forms.MouseEventHandler(zoom_MouseWheel);
            chart1.MouseMove += new System.Windows.Forms.MouseEventHandler(zoom_MouseMove);
            chart1.MouseUp += new System.Windows.Forms.MouseEventHandler(zoom_MouseUp);
            chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(zoom_MouseDown);
            
        }

        private void zoom_MouseDown(object sender, EventArgs e)
        {
            chart1.Focus();
        }
        private void zoom_MouseMove(object sender, EventArgs e)
        {
            MouseEventArgs mouse = e as MouseEventArgs;

            if (mouse.Button == MouseButtons.Left)
            {
                Point mousePosNow = mouse.Location;

                if (!mouseLeftpressed)
                {
                    mouseLeftpressed = true;
                    mouseDown = mouse.Location;
                    try
                    {
                        startX = chart1.ChartAreas[0].AxisX.PixelPositionToValue(mouseDown.X);
                        startY = chart1.ChartAreas[0].AxisY.PixelPositionToValue(mouseDown.Y);
                    }
                    catch (ArgumentException)
                    {

                    }
                    catch (InvalidOperationException)
                    {

                    }
                }
                // the distance the mouse has been moved since mouse was pressed
                int deltaX = mousePosNow.X - mouseDown.X;
                int deltaY = mousePosNow.Y - mouseDown.Y;

                try
                {
                    endX = chart1.ChartAreas[0].AxisX.PixelPositionToValue(mousePosNow.X);
                    endY = chart1.ChartAreas[0].AxisY.PixelPositionToValue(mousePosNow.Y);
                }
                catch (ArgumentException)
                {

                }
                catch (InvalidOperationException)
                {

                }
                // calculate new offset of image based on the current zoom factor
               
                chart1.Series["MouseZoom"].Points.Clear();
                chart1.Series["MouseZoom"].Points.AddXY(startX, startY);
                chart1.Series["MouseZoom"].Points.AddXY(endX, startY);
                chart1.Series["MouseZoom"].Points.AddXY(endX, endY);
                chart1.Series["MouseZoom"].Points.AddXY(startX, endY);
                chart1.Series["MouseZoom"].Points.AddXY(startX, startY);
            }
            else if (mouse.Button == MouseButtons.Middle)
            {
                Point mousePosNow = mouse.Location;

                if (!mouseWheelPressed)
                {
                    mouseWheelPressed = true;
                    mouseDown = mouse.Location;

                    panStartX = chart1.ChartAreas[0].AxisX.PixelPositionToValue(mouseDown.X);
                    panStartY = chart1.ChartAreas[0].AxisY.PixelPositionToValue(mouseDown.Y);
                    panDeltaX = chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum;
                    panDeltaY = chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum;
                }

                try
                {
                    panEndX = chart1.ChartAreas[0].AxisX.PixelPositionToValue(mousePosNow.X);
                    panEndY = chart1.ChartAreas[0].AxisY.PixelPositionToValue(mousePosNow.Y);
                }
                catch (ArgumentException)
                {

                }
                catch (InvalidOperationException)
                {

                }

                    double max, min;
                    max = chart1.ChartAreas[0].AxisX.Maximum - (panEndX - panStartX);
                    max = Math.Min(max, samples);
                    min = chart1.ChartAreas[0].AxisX.Minimum - (panEndX - panStartX);
                    min = Math.Max(min, 0);              

                if (max - min == panDeltaX)
                { 
                    chart1.ChartAreas[0].AxisX.Maximum = max;
                    chart1.ChartAreas[0].AxisX.Minimum = min;
                }

                    max = chart1.ChartAreas[0].AxisY.Maximum - (panEndY - panStartY);
                    max = Math.Min(max, amplitude);
                    min = chart1.ChartAreas[0].AxisY.Minimum - (panEndY - panStartY);
                    min = Math.Max(min, -amplitude);

                if (max - min == panDeltaY)
                {
                    chart1.ChartAreas[0].AxisY.Maximum = max;
                    chart1.ChartAreas[0].AxisY.Minimum = min;
                }
                
                

               

            }
             
        }

        private void zoom_MouseUp(object sender, EventArgs e)
        {
            
            mouseWheelPressed = false;

            if (mouseLeftpressed && Math.Abs(endX - startX) >= 1 && Math.Abs(endY - startY) >= 0.1)
            {
                
                chart1.ChartAreas[0].AxisX.Maximum = Math.Max(startX, endX);
                chart1.ChartAreas[0].AxisX.Maximum = Math.Min(chart1.ChartAreas[0].AxisX.Maximum, samples);
                chart1.ChartAreas[0].AxisX.Minimum = Math.Min(startX, endX);
                chart1.ChartAreas[0].AxisX.Minimum = Math.Max(chart1.ChartAreas[0].AxisX.Minimum, 0);
                chart1.ChartAreas[0].AxisX.Interval = Math.Abs(chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) / 10;
            
                chart1.ChartAreas[0].AxisY.Maximum = Math.Max(startY, endY);
                chart1.ChartAreas[0].AxisY.Maximum = Math.Min(chart1.ChartAreas[0].AxisY.Maximum, amplitude);
                chart1.ChartAreas[0].AxisY.Minimum = Math.Min(startY, endY);
                chart1.ChartAreas[0].AxisY.Minimum = Math.Max(chart1.ChartAreas[0].AxisY.Minimum, -amplitude);
                chart1.ChartAreas[0].AxisY.Interval = Math.Abs(endY - startY) / 10;

            }
            mouseLeftpressed = false;
            chart1.Series["MouseZoom"].Points.Clear();

            
        }

      
        private void zoom_MouseWheel(object sender, MouseEventArgs e)
        {

            
            double delta, min, max, middle;

            middle = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);

            double zoomFactor = (1 - 0.25 * e.Delta / 120);
            
            max = chart1.ChartAreas[0].AxisX.Maximum * zoomFactor;
            min = chart1.ChartAreas[0].AxisX.Minimum * zoomFactor;            
            delta = max - min;

            if (middle + delta / 2 > samples)
            {
                max = samples;
                min = samples - delta;

            }
            else if (middle - delta / 2 < 0) 
            {
                min = 0;
                max = delta;
            }
            else
            {
                max = middle + delta / 2;
                min = middle - delta / 2;
            }

            if(max - min < samples)
            {
                chart1.ChartAreas[0].AxisX.Maximum = max;
                chart1.ChartAreas[0].AxisX.Minimum = min;     
            }
            else
            {
                chart1.ChartAreas[0].AxisX.Maximum = samples;
                chart1.ChartAreas[0].AxisX.Minimum = 0;    
            }

            middle = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
            max = chart1.ChartAreas[0].AxisY.Maximum * zoomFactor;
            min = chart1.ChartAreas[0].AxisY.Minimum * zoomFactor;
            delta = max - min;

            if (middle + delta / 2 > amplitude)
            {
                max = amplitude;
                min = amplitude - delta;

            }
            else if (middle - delta / 2 < -amplitude)
            {
                min = -amplitude;
                max = -amplitude + delta;
            }
            else
            {
                max = middle + delta / 2;
                min = middle - delta / 2;

            }

            if (max - min < 2 * amplitude)
            {
                chart1.ChartAreas[0].AxisY.Maximum = max;
                chart1.ChartAreas[0].AxisY.Minimum = min;
            }
            else
            {
                chart1.ChartAreas[0].AxisY.Maximum = amplitude;
                chart1.ChartAreas[0].AxisY.Minimum = -amplitude;
            }

            chart1.ChartAreas[0].AxisX.Interval = (chart1.ChartAreas[0].AxisX.Maximum - chart1.ChartAreas[0].AxisX.Minimum) / 10;
            chart1.ChartAreas[0].AxisY.Interval = (chart1.ChartAreas[0].AxisY.Maximum - chart1.ChartAreas[0].AxisY.Minimum) / 10;
            /*
            //chart1.ChartAreas[0].AxisY.Maximum = amplitude;
            chart1.ChartAreas[0].AxisY.Minimum = -amplitude;
            chart1.ChartAreas[0].AxisY.Interval = amplitude * 2 / 10;
            */
           
        }

        public void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            changing = true;
            label1.Text = trackBar1.Value.ToString();
            samples = trackBar1.Value;
            x = new double[samples];
            y = new double[samples];
            
            changing = false;
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!changing)
            { 
                chart1.Series.SuspendUpdates();
                Random rand = new Random();
                chart1.Series["Series1"].Points.Clear();
                for (int r = 0; r < samples || changing; r += 1)
                {
                    //chart1.Series["Series1"].Points.AddXY(r, 15 * Math.Sin(r / 100) + rand.NextDouble());
                   x[r] = r;
                   y[r] = 15 * Math.Sin((double)r / 10000) + rand.NextDouble();
                }
                updateChart(x, y,samples);
   
                chart1.Series.ResumeUpdates();               
            }
        }

        public void updateChart(Double[] x, Double[] y, int samples)
        {
            int max = (int)chart1.ChartAreas[0].AxisX.Maximum;
            int min = (int)chart1.ChartAreas[0].AxisX.Minimum;

            Double downsample;
            if ((max - min) > 2000)
                downsample = (double)(max - min) / 2000;
            else
                downsample = 1;

            int _samples = 0;
            int r = min;
            for (; r < max; r += (int)downsample)
            {
                chart1.Series["Series1"].Points.AddXY(x[r], y[r]);
               // if (_samples == 2000)
                 //   break;
                _samples++;
            }
         //   Console.WriteLine(x[r - 1]);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButton1.Checked)
                chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            chart1.Series["Series1"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
        }

       
    }
}
