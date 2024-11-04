using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 비매샵테스트
{
    public partial class autoaimForm : Form
    {
        Form1 _frm;
        public autoaimForm()
        {
            InitializeComponent();
        }
        public autoaimForm(Form1 frm)
        {
            InitializeComponent();
            _frm = frm;
        }

     
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
             if(textBox7.Text.Length == 0)
            {
                textBox7.Text = "0";
                Form1.autoaim_Xaxis = 0;


                return;
            }
            int i;
            if(int.TryParse(textBox7.Text,out i))
            {
                if(i > 399)
                {
                    textBox7.Text = "399";
                    Form1.autoaim_Xaxis = 399;
                }
                else
                {
                    Form1.autoaim_Xaxis = i;
                    
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 0)
            {
                textBox2.Text = "0";
                Form1.autoaim_Yaxis = 0;
                
                return;
            }
            int i;
            if (int.TryParse(textBox2.Text, out i))
            {
                if (i > 299)
                {
                    Form1.autoaim_Yaxis = 299;
                    textBox2.Text = "299";
                    
                }
                else
                {
                    Form1.autoaim_Yaxis = i;
                    
                }
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length == 0)
            {
                textBox3.Text = "0";
                Form1.autoaim_colorRange = 0;
                return;
            }
            int i;
            if (int.TryParse(textBox3.Text, out i))
            {
                if (i > 255)
                {
                    textBox3.Text = "255";
                    Form1.autoaim_colorRange = 255;
                }
                else
                {
                    Form1.autoaim_colorRange = i;
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Form1.autoaim_Xoff = Decimal.ToInt32(numericUpDown1.Value);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Form1.autoaim_Yoff = Decimal.ToInt32(numericUpDown2.Value);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Form1.autoaim_YaxisOff = checkBox1.Checked;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
              if (textBox6.Text.Length == 0)
            {
                Form1.autoaim_mouseSpeed분자 = 1;
                textBox6.Text = "1";
                double temp = 1.0 / int.Parse(textBox5.Text);
                textBox4.Text = "" +temp;
                Form1.autoaim_mouseSpeed = temp;

                return;
            }
            int i;
            if (int.TryParse(textBox6.Text, out i))
            {
                if (i == 0)
                {
                    Form1.autoaim_mouseSpeed분자 = 1;
                    textBox6.Text = "1";
                    double temp = 1.0 / int.Parse(textBox5.Text);
                    textBox4.Text = "" + temp;
                    Form1.autoaim_mouseSpeed = temp;
                }
                else
                {
                    Form1.autoaim_mouseSpeed분자 = i;
                    double temp = (double)i / int.Parse(textBox5.Text);
                    textBox4.Text = "" + temp;
                    Form1.autoaim_mouseSpeed = temp;
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
              if (textBox5.Text.Length == 0)
            {
                Form1.autoaim_mouseSpeed분모 = 1;
                double temp = int.Parse(textBox6.Text) / 1.0;
                textBox5.Text = "1";
                textBox4.Text = "" + temp;
                Form1.autoaim_mouseSpeed = temp;
                return;
            }


            int i;
            if (int.TryParse(textBox5.Text, out i))
            {
                if (i == 0)
                {
                    Form1.autoaim_mouseSpeed분모 = 1;
                    double temp = int.Parse(textBox6.Text) / 1.0;
                    textBox5.Text = "1";
                    textBox4.Text = "" + temp;
                    Form1.autoaim_mouseSpeed = temp;
                }
                else
                {
                    Form1.autoaim_mouseSpeed분모 = i;
                    double temp = int.Parse(textBox6.Text) / (double)i;
                    textBox4.Text = "" + temp;
                    Form1.autoaim_mouseSpeed = temp;
                }
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
              if (textBox11.Text.Length == 0)
            {
                Form1.autoaim_YmouseSpeed분자 = 1;
                textBox11.Text = "1";
                double temp = 1.0 / int.Parse(textBox10.Text);
                textBox9.Text = "" +temp;
                Form1.autoaim_YmouseSpeed = temp;

                return;
            }
            int i;
            if (int.TryParse(textBox11.Text, out i))
            {
                if (i == 0)
                {
                    Form1.autoaim_YmouseSpeed분자 = 1;
                    textBox11.Text = "1";
                    double temp = 1.0 / int.Parse(textBox10.Text);
                    textBox9.Text = "" + temp;
                    Form1.autoaim_YmouseSpeed = temp;
                }
                else
                {
                    Form1.autoaim_YmouseSpeed분자 = i;
                    double temp = (double)i / int.Parse(textBox10.Text);
                    textBox9.Text = "" + temp;
                    Form1.autoaim_YmouseSpeed = temp;
                }
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (textBox10.Text.Length == 0)
            {
                Form1.autoaim_YmouseSpeed분모 = 1;
                double temp = int.Parse(textBox11.Text) / 1.0;
                textBox10.Text = "1";
                textBox9.Text = "" + temp;
                Form1.autoaim_YmouseSpeed = temp;
                return;
            }


            int i;
            if (int.TryParse(textBox10.Text, out i))
            {
                if (i == 0)
                {
                    Form1.autoaim_YmouseSpeed분모 = 1;
                    double temp = int.Parse(textBox11.Text) / 1.0;
                    textBox10.Text = "1";
                    textBox9.Text = "" + temp;
                    Form1.autoaim_YmouseSpeed = temp;
                }
                else
                {
                    Form1.autoaim_YmouseSpeed분모 = i;
                    double temp = int.Parse(textBox11.Text) / (double)i;
                    textBox9.Text = "" + temp;
                    Form1.autoaim_YmouseSpeed = temp;
                }
            }
        }



        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            double gamma = (trackBar1.Value / 10000.0);
            textBox13.Text = gamma.ToString();
            Form1.gammaValueChange(gamma);

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                _frm.toCopyFromScreen();
                Form1.screenCaptureType = 0;
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            { 
                _frm.toPrintwindow();   
                Form1.screenCaptureType = 1;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
            {
                _frm.toImageProcessCopyFromScreen();
                groupBox4.Visible = true;
                Form1.screenCaptureType = 2;
            }
            else
            {
                groupBox4.Visible=false;
            }
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            textBox8.Text = trackBar2.Value.ToString();
            Form1.mouseStep = trackBar2.Value;
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            textBox12.Text = trackBar3.Value.ToString();
            Form1.mouseSpeed = trackBar3.Value;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                _frm.toSimpleMouseMove();
                groupBox6.Visible = true;
                Form1.mouseType = 0;
            }
            else
            {
                groupBox6.Visible = false;
            }
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
            {
                _frm.toSmoothMouseMove();
                groupBox8.Visible = true;
                Form1.mouseType = 1;
            }
            else
            {
                groupBox8.Visible = false;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)
            {
                _frm.toInjectMouseMove();
                 groupBox6.Visible = true;
                Form1.mouseType = 2;
            }
            else
            {
                 groupBox6.Visible = false;
            }
        }

        private void autoaimForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = Form1.autoaim_mouseButton;
            try { 
                if(Form1.total_dance_number != 0) { 
                    comboBox2.Items.Clear();
                    comboBox2.Items.Add("사용 안함");
                    for(int i =1 ;i <= Form1.total_dance_number; i++)
                    {
                        comboBox2.Items.Add(i+"번 춤 풀기");
                    }
                    comboBox2.SelectedIndex = Form1.dance_bug;
                    button1.Text = "다른 파일 선택";

                    label4.Text = Path.GetFileNameWithoutExtension(Form1.dance_name);;
                        Font ft = new Font(label4.Font.Name, 7);
                        label4.Font = ft;

                }
                else
                {
                    comboBox2.SelectedIndex = 0;
                }
            }
            catch
            {

            }

            

            RadioButton[] radioButtons = new RadioButton[] {radioButton1,radioButton2,radioButton4,radioButton8,radioButton3,radioButton10,radioButton11,radioButton12,radioButton9 ,radioButton14, radioButton15};

            radioButtons[Form1.screenCaptureType].Checked = true;

            if(Form1.mouseType == 1)
            {
                radioButton6.Checked = true;
                trackBar3.Value = Form1.mouseSpeed;
                textBox12.Text = ""+Form1.mouseSpeed;
            }
            else if(Form1.mouseType == 2)
            {
                radioButton7.Checked = true;
                trackBar2.Value = Form1.mouseStep;
                textBox8.Text = ""+Form1.mouseStep;
            }
            else
            {
                trackBar2.Value = Form1.mouseStep;
                textBox8.Text = ""+Form1.mouseStep;
            }
            
            trackBar1.Value = (int)(Form1.gammaValue * 10000.0);
            textBox13.Text = "" +Form1.gammaValue;

            textBox7.Text = "" + Form1.autoaim_Xaxis;
            textBox2.Text = "" + Form1.autoaim_Yaxis;
            textBox3.Text = "" + Form1.autoaim_colorRange;

            numericUpDown1.Value = Form1.autoaim_Xoff;
            numericUpDown2.Value = Form1.autoaim_Yoff;

            checkBox1.Checked = Form1.autoaim_YaxisOff;

            textBox6.Text = "" + Form1.autoaim_mouseSpeed분자;
            textBox5.Text = "" + Form1.autoaim_mouseSpeed분모;

            textBox11.Text = "" + Form1.autoaim_YmouseSpeed분자;
            textBox10.Text = "" + Form1.autoaim_YmouseSpeed분모;

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Form1.autoaim_mouseButton = comboBox1.SelectedIndex;
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toBlur();
            Form1.screenCaptureType = 3;
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            _frm.tosharp();
            Form1.screenCaptureType = 8;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toBlurTest();
            Form1.screenCaptureType = 4;
        }

        private void radioButton10_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toBoxFilter();
            Form1.screenCaptureType = 5;
        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toMedianBlur();
            Form1.screenCaptureType = 6;
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toGaussianBlur();
            Form1.screenCaptureType = 7;
        }

        private void radioButton13_CheckedChanged(object sender, EventArgs e)
        {
            _frm.toBilateralFilter();
        }

        private void radioButton14_CheckedChanged(object sender, EventArgs e)
        {
            _frm.to16Bit565();


            Form1.screenCaptureType = 9;
        }

        private void radioButton15_CheckedChanged(object sender, EventArgs e)
        {
            _frm.to24bit();

            Form1.screenCaptureType = 10;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Form1.dance_bug = comboBox2.SelectedIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int number = 0;
            
            {
                OpenFileDialog openFile = new OpenFileDialog();
                openFile.InitialDirectory = Form1.suddenRootpath + @"game\sa_weapons\models\emotion\";
                openFile.Title = "춤 버그 파일 선택";
                openFile.DefaultExt = "ltb";
                openFile.Filter = "서든어택 이모션 파일(*.ltb)|*.ltb";

                if (openFile.ShowDialog() == DialogResult.OK)
                {
                    
                    byte[] bytes = File.ReadAllBytes(openFile.FileName);

                    for(int i = 0; i < bytes.Length -4; i++)
                    {
                        if ((bytes[i] | 32) == 0x65)
                        {
                            if((bytes[i + 1] | 32) == 109)
                            {
                                for(int j = 1 ; j < 10; j++)
                                {
                                    if(bytes[i+2] == (48 + j))
                                    {
                                        if(j > number)
                                        {
                                            number = j;
                                        }
                                    } 
                                }
                            }
                        }

                    }

                    if(number == 0)
                    {
                        MessageBox.Show("이 파일에서는 춤(댄스)을 찾을 수 없습니다.");
                    }
                    else {
                        string simpleName = Path.GetFileNameWithoutExtension(openFile.FileName);
                        if (!Form1.dance_name.Equals(""))
                        {
                            File.Copy(Form1.fullpath+"춤 원본\\"+Path.GetFileNameWithoutExtension(Form1.dance_name)+".ltb", Form1.dance_name, true);
                        }
                        Form1.dance_name = openFile.FileName;
                         comboBox2.Items.Clear();
                        comboBox2.Items.Add("사용 안함");
                        for(int i =1 ;i <= number; i++)
                        {
                           comboBox2.Items.Add(i+"번 춤 풀기");
                        }
                        comboBox2.SelectedIndex = 0;
                        Form1.dance_bug = 0;
                        button1.Text = "다른 파일 선택";
                        Form1.total_dance_number = number;
                        label4.Text = simpleName;
                        Font ft = new Font(label4.Font.Name, 7);
                        label4.Font = ft;

                        try
                        {
                            File.Copy(openFile.FileName, Form1.fullpath + "춤 원본\\" + simpleName + ".ltb", false);
                        }
                        catch
                        {

                        }

                    }


                }


            }
            
          
        }

       
    }
}
