
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 비매샵테스트
{
    public partial class autoaimColorForm : Form
    {
        Form1 _frm;

        public int rmin = 0;
        public int rmax = 0;
        public int gmin = 0;
        public int gmax = 0;
        public int bmin = 0;
        public int bmax = 0;

        public double hmin = 0 ;
        public double hmax = 0;
        public double smin = 0;
        public double smax = 0;
        public double vmin = 0;
        public double vmax = 0;


        public autoaimColorForm()
        {
            InitializeComponent();
        }

        public void rgbChange(int r,int r2,int g, int g2,int b, int b2)
        {
            rmin = r;
            rmax = r2;
            gmin = g;
            gmax = g2;
            bmin = b;
            bmax = b2;
        }

        public void hsvChange(double h, double h2, double s, double s2, double v, double v2)
        {
            hmin = h;
            hmax = h2;
            smin = s;
            smax = s2;
            vmin = v;
            vmax = v2;
        }

        public autoaimColorForm(Form1 frm)
        {
            InitializeComponent();
            _frm = frm;


        }

           

      

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                groupBox2.Visible = true;
                Form1.colorType = 1;
                _frm.teamChange();
                listView1.Items.Clear();
                if (radioButton3.Checked) { 
                                rgbRedAdd();
                }
                else
                {
                    rgbBlueAdd();
                }
            }
            else
            {
                groupBox2.Visible = false;
            }
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            rgbRedAdd();

        }

        public void rgbRedAdd()
        {
            
            for(int i = 0 ; i < Form1.red_RGB.Count; i++)
            {
                RGBDATA temp = Form1.red_RGB.ElementAt(i);
                ListViewItem item = new ListViewItem(""+temp.rmin);
                item.SubItems.Add(""+temp.rmax);
                item.SubItems.Add(""+temp.gmin);
                item.SubItems.Add(""+temp.gmax);
                item.SubItems.Add(""+temp.bmin);
                item.SubItems.Add(""+temp.bmax);

                listView1.Items.Add(item);
            }
        }

        public void rgbBlueAdd()
        {
            for(int i = 0 ; i < Form1.blue_RGB.Count; i++)
            {
                RGBDATA temp = Form1.blue_RGB.ElementAt(i);
                ListViewItem item = new ListViewItem(""+temp.rmin);
                item.SubItems.Add(""+temp.rmax);
                item.SubItems.Add(""+temp.gmin);
                item.SubItems.Add(""+temp.gmax);
                item.SubItems.Add(""+temp.bmin);
                item.SubItems.Add(""+temp.bmax);

                listView1.Items.Add(item);
            }
        }


        public void hsvRedAdd()
        {
           for(int i = 0 ; i < Form1.red_HSV.Count; i++)
            {
                HSVDATA temp = Form1.red_HSV.ElementAt(i);
                ListViewItem item = new ListViewItem(""+temp.hmin);
                item.SubItems.Add(""+temp.hmax);
                item.SubItems.Add(""+temp.smin * 100.0);
                item.SubItems.Add(""+temp.smax * 100.0);
                item.SubItems.Add(""+temp.vmin * 100.0);
                item.SubItems.Add(""+temp.vmax * 100.0);

                listView2.Items.Add(item);
            }
        }

        public void hsvBlueAdd()
        {
           for(int i = 0 ; i < Form1.blue_HSV.Count; i++)
            {
                HSVDATA temp = Form1.blue_HSV.ElementAt(i);
                ListViewItem item = new ListViewItem(""+temp.hmin);
                item.SubItems.Add(""+temp.hmax);
                item.SubItems.Add(""+temp.smin * 100.0);
                item.SubItems.Add(""+temp.smax * 100.0);
                item.SubItems.Add(""+temp.vmin * 100.0);
                item.SubItems.Add(""+temp.vmax * 100.0);

                listView2.Items.Add(item);
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            rgbBlueAdd();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            coloraddForm loginForm = new coloraddForm(this);

            DialogResult Result = loginForm.ShowDialog();

            if (Result == DialogResult.OK)
            {
                RGBDATA temp = new RGBDATA();
                        temp.rmin = this.rmin;
                    temp.rmax = this.rmax;
                    temp.gmin = this.gmin;
                    temp.gmax = this.gmax;
                    temp.bmin = this.bmin;
                    temp.bmax = this.bmax;
                listView1.Items.Clear();

                  if (radioButton3.Checked)
                  {
                     Form1.red_RGB.Add(temp);
                    rgbRedAdd();
                  }
                  else
                  {
                    Form1.blue_RGB.Add(temp);
                    rgbBlueAdd();
                  }
            }


          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                for(int i = listView1.Items.Count -1 ; i >= 0 ; i--)
                {
                    if (listView1.Items[i].Selected)
                    {
                        Form1.red_RGB.RemoveAt(i);
                        
                    }

                }
                            listView1.Items.Clear();
                rgbRedAdd();
            }
            else
            {
                for(int i = listView1.Items.Count -1 ; i >= 0 ; i--)
                {
                    if (listView1.Items[i].Selected)
                    {
                        Form1.blue_RGB.RemoveAt(i);
                        
                    }

                }
                            listView1.Items.Clear();
                rgbBlueAdd();
                
            }

            
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                groupBox3.Visible = true;
                Form1.colorType = 2;
                _frm.teamChange();
                listView2.Items.Clear();
                if (radioButton7.Checked)
                {
                                                    hsvRedAdd();
                }
                else
                {
                    hsvBlueAdd();
                }
            }
            else
            {
                groupBox3.Visible = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            hsvaddForm loginForm = new hsvaddForm(this);

            DialogResult Result = loginForm.ShowDialog();

            if (Result == DialogResult.OK)
            {
                HSVDATA temp = new HSVDATA();
                        temp.hmin = this.hmin;
                    temp.hmax = this.hmax;
                    temp.smin = this.smin;
                    temp.smax = this.smax;
                    temp.vmin = this.vmin;
                    temp.vmax = this.vmax;
                listView2.Items.Clear();

                  if (radioButton7.Checked)
                  {
                     Form1.red_HSV.Add(temp);
                    hsvRedAdd();
                  }
                  else
                  {
                    Form1.blue_HSV.Add(temp);
                    hsvBlueAdd();
                  }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                for(int i = listView2.Items.Count -1 ; i >= 0 ; i--)
                {
                    if (listView2.Items[i].Selected)
                    {
                        Form1.red_HSV.RemoveAt(i);
                        
                    }

                }
                            listView2.Items.Clear();
                hsvRedAdd();
            }
            else
            {
                for(int i = listView2.Items.Count -1 ; i >= 0 ; i--)
                {
                    if (listView2.Items[i].Selected)
                    {
                        Form1.blue_HSV.RemoveAt(i);
                        
                    }

                }
                            listView2.Items.Clear();
                hsvBlueAdd();
                
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            if (radioButton.Checked)
            {
                Form1.colorType = 0;
                _frm.teamChange();
            }

        }

        public void rgb2hsv(int r,int g,int b,out double h, out double s, out double v)
        {
            double      min, max, delta;

            min = r < g ? r : g;
            min = min  < b ? min  : b;

            max = r > g ? r : g;
            max = max  > b ? max  : b;

            v = max;                                // v
            delta = max - min;
            if (delta < 0.00001)
            {
                s = 0;
                h = 0; // undefined, maybe nan?
                v /= 255;
                return;
            }
            if( max > 0.0 ) { // NOTE: if Max is == 0, this divide would cause a crash
                s = (delta / max);                  // s
            } else {
                // if max is 0, then r = g = b = 0              
                // s = 0, h is undefined
                s = 0.0;
                h = 0.0;                            // its now undefined
                v /= 255;
                return;
            }
            if( r >= max )                           // > is bogus, just keeps compilor happy
                h = ( g - b ) / delta;        // between yellow & magenta
            else
            if( g >= max )
                h = 2.0 + ( b - r ) / delta;  // between cyan & yellow
            else
                h = 4.0 + ( r - g ) / delta;  // between magenta & cyan

            h *= 60.0;                              // degrees

            if( h < 0.0 )
                h += 360.0;

            v /= 255;
            return;
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
           

            string fileName = "";
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.InitialDirectory = Form1.fullpath + "saveData\\";
            saveFile.Title = "잠수 형광오토 설정 저장";
            saveFile.DefaultExt = "jamsus";
            saveFile.Filter = "잠수몬 테스트(*.jamsus)|*.jamsus";

            if(saveFile.ShowDialog() == DialogResult.OK)
            {
                fileName = saveFile.FileName.ToString();


                 DataClass temp = new DataClass{
                screenCaptureType = Form1.screenCaptureType,
                mouseType = Form1.mouseType,
                mouseStep = Form1.mouseStep,
                mouseSpeed = Form1.mouseSpeed,
                mouseButton = Form1.autoaim_mouseButton,
                gammaValue = Form1.gammaValue,
                Xaxis = Form1.autoaim_Xaxis,
                Yaxis = Form1.autoaim_Yaxis,
                colorRange = Form1.autoaim_colorRange,
                Xoff = Form1.autoaim_Xoff,
                Yoff = Form1.autoaim_Yoff,
                YaxisOff = Form1.autoaim_YaxisOff,
                Xspeed = Form1.autoaim_mouseSpeed분자,
                Xspeed_ = Form1.autoaim_mouseSpeed분모,
                Yspeed = Form1.autoaim_YmouseSpeed분자,
                Yspeed_ = Form1.autoaim_YmouseSpeed분모,
                colorType = Form1.colorType,

                redRGB = Form1.red_RGB,
                blueRGB = Form1.blue_RGB,
                redHSV = Form1.red_HSV,
                blueHSV = Form1.blue_HSV
                };


                

                {
                    string saveData = Form1.screenCaptureType + "," + Form1.mouseType + "," + Form1.mouseStep + "," +
                        Form1.mouseSpeed + "," + Form1.autoaim_mouseButton + "," + Form1.gammaValue + "," + Form1.autoaim_Xaxis + "," +
                        Form1.autoaim_Yaxis + "," + Form1.autoaim_colorRange + "," + Form1.autoaim_Xoff + "," + Form1.autoaim_Yoff + "," + (Form1.autoaim_YaxisOff?"ON":"OFF") + "," +
                        Form1.autoaim_mouseSpeed분자 + "," + Form1.autoaim_mouseSpeed분모 + "," + Form1.autoaim_YmouseSpeed분자 + "," +
                        Form1.autoaim_YmouseSpeed분모 + "," + Form1.colorType +"\r\n";
                    for(int i = 0 ; i < Form1.red_RGB.Count ; i++)
                    {
                        saveData = saveData + Form1.red_RGB[i].rmin + "," + Form1.red_RGB [i].rmax + "," + Form1.red_RGB[i].gmin + "," + Form1.red_RGB [i].gmax + "," + Form1.red_RGB[i].bmin + "," + Form1.red_RGB[i].bmax + "_";
                    }
                    saveData = saveData + "\r\n";
                    for(int i = 0 ; i < Form1.blue_RGB.Count ; i++)
                    {
                        saveData = saveData + Form1.blue_RGB[i].rmin + "," + Form1.blue_RGB [i].rmax + "," + Form1.blue_RGB[i].gmin + "," + Form1.blue_RGB [i].gmax + "," + Form1.blue_RGB[i].bmin + "," + Form1.blue_RGB[i].bmax + "_";
                    }
                    saveData = saveData + "\r\n";
                    for(int i = 0 ; i < Form1.red_HSV.Count; i++)
                    {
                        saveData = saveData + Form1.red_HSV[i].hmin + "," + Form1.red_HSV [i].hmax + "," + Form1.red_HSV [i].smin + "," + Form1.red_HSV[i].smax + "," + Form1.red_HSV[i].vmin + "," + Form1.red_HSV[i].vmax + "_";
                    }
                    saveData = saveData + "\r\n";
                    for(int i = 0 ; i < Form1.blue_HSV.Count; i++)
                    {
                        saveData = saveData + Form1.blue_HSV[i].hmin + "," + Form1.blue_HSV [i].hmax + "," + Form1.blue_HSV [i].smin + "," + Form1.blue_HSV[i].smax + "," + Form1.blue_HSV[i].vmin + "," + Form1.blue_HSV[i].vmax + "_";
                    }
                    saveData = saveData + "\r\n";
                    
                    File.WriteAllText(fileName,saveData);
                }
            }

        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            hsvRedAdd();
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            listView2.Items.Clear();
            hsvBlueAdd();
        }

        private void autoaimColorForm_Load(object sender, EventArgs e)
        {
           if(Form1.colorType == 1)
            {
                radioButton2.Checked = true;

            }
            else if(Form1.colorType == 2)
            {
                radioButton4.Checked = true;
            }
        }
            public static T DeepCopy<T>(T obj)
            {
                using (var stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(stream, obj);
                    stream.Position = 0;
 
                    return (T)formatter.Deserialize(stream);
                }
            }
        private void button4_Click(object sender, EventArgs e)
        {
             string fileName = "";
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.InitialDirectory = Form1.fullpath + "saveData\\";
            openFile.Title = "잠수 형광오토 설정 불러오기";
            openFile.DefaultExt = "jamsus";
            openFile.Filter = "잠수몬 테스트(*.jamsus)|*.jamsus";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                fileName = openFile.FileName.ToString();
                string[] tempArray =  File.ReadAllLines(fileName);
                {
                    string[] tempValue = tempArray[0].Split(',');
                    Form1.screenCaptureType = int.Parse(tempValue[0]);
                    Form1.mouseType = int.Parse(tempValue[1]);
                    Form1.mouseStep = int.Parse(tempValue[2]);
                    Form1.mouseSpeed = int.Parse(tempValue[3]);
                    Form1.autoaim_mouseButton = int.Parse(tempValue[4]);
                    Form1.gammaValue = double.Parse(tempValue[5]);
                    Form1.autoaim_Xaxis = int.Parse(tempValue[6]);
                    Form1.autoaim_Yaxis = int.Parse(tempValue[7]);
                    Form1.autoaim_colorRange = int.Parse(tempValue[8]);
                    Form1.autoaim_Xoff = int.Parse(tempValue[9]);
                    Form1.autoaim_Yoff = int.Parse(tempValue[10]);
                    Form1.autoaim_YaxisOff = (tempValue[11].Equals("ON"));
                    Form1.autoaim_mouseSpeed분자 = int.Parse(tempValue[12]);
                    Form1.autoaim_mouseSpeed분모 = int.Parse(tempValue[13]);
                    Form1.autoaim_YmouseSpeed분자 = int.Parse(tempValue[14]);
                    Form1.autoaim_YmouseSpeed분모 = int.Parse(tempValue[15]);
                    Form1.colorType = int.Parse(tempValue[16]);
                }
                Form1.red_RGB.Clear();
                                    Form1.blue_RGB.Clear();
                                    Form1.red_HSV.Clear();
                                    Form1.blue_HSV.Clear();


                if(tempArray[1].Length > 6)
                {

                    string[] tempValue = tempArray[1].Split('_');
                    RGBDATA[] tempRGBDATA = new RGBDATA[tempValue.Length -1];
                    
                    for(int i = 0 ; i < tempRGBDATA.Length ; i++)
                    {
                        string[] tmp = tempValue[i].Split(',');
                        
                        tempRGBDATA[i] = new RGBDATA();
                        tempRGBDATA[i].rmin = int.Parse(tmp[0]);
                        tempRGBDATA[i].rmax = int.Parse(tmp[1]);
                        tempRGBDATA[i].gmin = int.Parse(tmp[2]);
                        tempRGBDATA[i].gmax = int.Parse(tmp[3]);
                        tempRGBDATA[i].bmin = int.Parse(tmp[4]);
                        tempRGBDATA[i].bmax = int.Parse(tmp[5]);
                        Form1.red_RGB.Add(tempRGBDATA[i]);
                    }
                    
                }
                

                if(tempArray[2].Length > 6)
                {
                    string[] tempValue = tempArray[2].Split('_');
                    RGBDATA[] tempRGBDATA = new RGBDATA[tempValue.Length -1];

                    for(int i = 0 ; i < tempRGBDATA.Length ; i++)
                    {
                        string[] tmp = tempValue[i].Split(',');
                        tempRGBDATA[i] = new RGBDATA();
                        tempRGBDATA[i].rmin = int.Parse(tmp[0]);
                        tempRGBDATA[i].rmax = int.Parse(tmp[1]);
                        tempRGBDATA[i].gmin = int.Parse(tmp[2]);
                        tempRGBDATA[i].gmax = int.Parse(tmp[3]);
                        tempRGBDATA[i].bmin = int.Parse(tmp[4]);
                        tempRGBDATA[i].bmax = int.Parse(tmp[5]);

                        Form1.blue_RGB.Add(tempRGBDATA[i]);
                    }
                }
                if(tempArray[3].Length > 6)
                {
                    string[] tempValue = tempArray[3].Split('_');
                    HSVDATA[] tempRGBDATA = new HSVDATA[tempValue.Length -1];


                    for(int i = 0 ; i <tempRGBDATA.Length  ; i++)
                    {
                        string[] tmp = tempValue[i].Split(',');
                        tempRGBDATA[i] = new HSVDATA();
                        tempRGBDATA[i].hmin = int.Parse(tmp[0]);
                        tempRGBDATA[i].hmax = int.Parse(tmp[1]);
                        tempRGBDATA[i].smin = double.Parse(tmp[2]);
                        tempRGBDATA[i].smax = double.Parse(tmp[3]);
                        tempRGBDATA[i].vmin = double.Parse(tmp[4]);
                        tempRGBDATA[i].vmax = double.Parse(tmp[5]);

                        Form1.red_HSV.Add(tempRGBDATA[i]);
                    }
                    

                }
                if(tempArray[4].Length > 6)
                {
                    string[] tempValue = tempArray[4].Split('_');
                    HSVDATA[] tempRGBDATA = new HSVDATA[tempValue.Length -1];
                    for(int i = 0 ; i <tempRGBDATA.Length ; i++)
                    {
                        string[] tmp = tempValue[i].Split(',');
                        tempRGBDATA[i] = new HSVDATA();
                        tempRGBDATA[i].hmin = int.Parse(tmp[0]);
                        tempRGBDATA[i].hmax = int.Parse(tmp[1]);
                        tempRGBDATA[i].smin = double.Parse(tmp[2]);
                        tempRGBDATA[i].smax = double.Parse(tmp[3]);
                        tempRGBDATA[i].vmin = double.Parse(tmp[4]);
                        tempRGBDATA[i].vmax = double.Parse(tmp[5]);

                        Form1.blue_HSV.Add(tempRGBDATA[i]);
                    }
                    
                }




                if (Form1.screenCaptureType == 0)
                {
                    _frm.toCopyFromScreen();
                }
                else if (Form1.screenCaptureType == 1)
                {
                    _frm.toPrintwindow();
                }
                else if (Form1.screenCaptureType == 2)
                {
                    _frm.toImageProcessCopyFromScreen();
                }
                else if (Form1.screenCaptureType == 3)
                {
                    _frm.toBlur();
                }
                else if (Form1.screenCaptureType == 4)
                {
                    _frm.toBlurTest();
                }
                else if (Form1.screenCaptureType == 5)
                {
                    _frm.toBoxFilter();
                }
                else if (Form1.screenCaptureType == 6)
                {
                    _frm.toMedianBlur();
                }
                else if (Form1.screenCaptureType == 7)
                {
                    _frm.toGaussianBlur();
                }
                else if (Form1.screenCaptureType == 8)
                {
                    _frm.tosharp();
                }
                else if (Form1.screenCaptureType == 9)
                {
                    _frm.to16Bit565();
                }
                else if (Form1.screenCaptureType == 10)
                {
                    _frm.to24bit();
                }

                if (Form1.mouseType == 0)
                {
                    _frm.toSimpleMouseMove();
                }
                else if (Form1.mouseType == 1)
                {
                    _frm.toSmoothMouseMove();
                }
                else if (Form1.mouseType == 2)
                {
                    _frm.toInjectMouseMove();
                }

               



                if (Form1.colorType == 0)
                {
                    radioButton1.Checked = true;
                }
                else if (Form1.colorType == 1)
                {
                    radioButton2.Checked = true;
                }
                else if (Form1.colorType == 2)
                {
                    radioButton4.Checked = true;
                }


                _frm.teamChange();

                listView1.Items.Clear();
                listView2.Items.Clear();
                if (radioButton3.Checked)
                {
                    rgbRedAdd();
                }
                else
                {
                    rgbBlueAdd();
                }

                if (radioButton7.Checked)
                {
                    hsvRedAdd();
                }
                else
                {
                    hsvBlueAdd();
                }


            }
        }
    }





    
    public class DataClass
    {
        public int screenCaptureType { get; set; }
        public int mouseType { get;set;}
        public int mouseStep {get;set; }
        public int mouseSpeed {get;set; }
        public int mouseButton { get;set;}

        public double gammaValue { get;set;}

        public int Xaxis { get;set;}
        public int Yaxis { get;set; }
        public int colorRange { get;set;}
        public int Xoff { get;set;}
        public int Yoff { get;set; }
        public bool YaxisOff { get;set; }
        public int Xspeed { get;set;}
        public int Xspeed_ { get;set;}
        public int Yspeed {get;set; }
        public int Yspeed_ { get;set; }



        public int colorType { get;set;}
        public List<RGBDATA> redRGB;
        public List<RGBDATA> blueRGB;
        public List<HSVDATA> redHSV;
        public List<HSVDATA> blueHSV;
    }

    [Serializable]
    public class RGBDATA
    { 
        public int rmin;
        public int rmax;
        public int gmin;
        public int gmax;
        public int bmin;
        public int bmax;
    }

    [Serializable]
    public class HSVDATA
    { 
        public double hmin;
        public double hmax;
        public double smin;
        public double smax;
        public double vmin;
        public double vmax;
    }



}
