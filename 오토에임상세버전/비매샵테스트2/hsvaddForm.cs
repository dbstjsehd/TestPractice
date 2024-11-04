using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 비매샵테스트
{
    public partial class hsvaddForm : Form
    {
        autoaimColorForm _frm;

         string prevText1 = string.Empty;
        string prevText2 = string.Empty;
        string prevText3 = string.Empty;
        string prevText4 = string.Empty;
        string prevText5 = string.Empty;
        string prevText6 = string.Empty;

        public hsvaddForm()
        {
            InitializeComponent();
        }

        public hsvaddForm(autoaimColorForm frm)
        {
            InitializeComponent();
            _frm = frm;
        }

        private void label7_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://ko.wikipedia.org/wiki/HSV_%EC%83%89_%EA%B3%B5%EA%B0%84");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _frm.hsvChange(Double.Parse(textBox1.Text),Double.Parse(textBox2.Text),Double.Parse(textBox3.Text) / 100.0,Double.Parse(textBox4.Text) / 100.0,Double.Parse(textBox5.Text) / 100.0,Double.Parse(textBox6.Text) / 100.0);


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }

            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText2;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
            }
            else
            {
                if(value > 360.0)
                {
                    temp.Text = "360.0";
                }
                if(value <= Double.Parse(textBox1.Text))
                {
                    textBox1.Text = temp.Text;
                }

                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                prevText2 = temp.Text;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }

            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText3;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
            }
            else
            {
                  if(value > 100.0)
                {
                    temp.Text = "100.0";
                }
                   if(value >= Double.Parse(textBox4.Text))
                {
                    textBox4.Text = temp.Text;
                }


                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                prevText3 = temp.Text;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }


            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText4;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
            }
            else
            {
                   if(value > 100.0)
                {
                    temp.Text = "100.0";
                }
                    if(value <= Double.Parse(textBox3.Text))
                {
                    textBox3.Text = temp.Text;
                }

                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                prevText4 = temp.Text;
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }


            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText5;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
            }
            else
            {
                   if(value > 100.0)
                {
                    temp.Text = "100.0";
                }
                    if(value >= Double.Parse(textBox6.Text))
                {
                    textBox6.Text = temp.Text;
                }

                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                prevText5 = temp.Text;
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }

            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText6;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
            }
            else
            {
                   if(value > 100.0)
                {
                    temp.Text = "100.0";
                }
                    if(value <= Double.Parse(textBox5.Text))
                {
                    textBox5.Text = temp.Text;
                }
                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                prevText6 = temp.Text;
            }
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
             double value = 0;
            TextBox temp = (TextBox)sender;

              if(temp.Text.Length == 0)
            {
                temp.Text = "0.0";
                return;
            }

            //입력받은 값이 double 형으로 변환활 수 있는 지 확인
            if (double.TryParse(temp.Text, out value) == false)
            {
                //변환할 수 없으면 이전 텍스트 값으로 재 설정
                temp.Text = prevText1;
                //커서 위치를 텍스트의 제일 마지막으로 위치시킴
                temp.Select(temp.Text.Length, 0);
                
            }
            else
            {
                //변환할 수 있으면 현재 값을 이전 값으로 저장해 둠.
                if(value > 360.0)
                {
                    temp.Text = "360.0";
                }
                if(value >= Double.Parse(textBox2.Text))
                {
                    textBox2.Text = temp.Text;
                }

                prevText1 = temp.Text;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine(e.KeyChar);
             if(e.KeyChar == '-')
            {
                e.Handled = true;
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.rapidtables.com/convert/color/rgb-to-hsv.html");
        }
    }
}
