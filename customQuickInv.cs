using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows.Forms;


namespace winformLearn
{
    public partial class customQuickInv : Form
    {
        //工具类

        //存放config变量
        public class Config
        {
            public static bool isDynamicDrawing = true;

            //public static bool angleWarning_Blue = true;//没做好，可能不做了
            public static bool isWarningRed = true;
            public static bool isRelative = false;




        }

        //存放其他全局变量
        public class common
        {
            

            public static double angleSum_ = 0;
            
        }

        //public class pictureSolt
        //{
        //    public PictureBox[] pictureSolts = new PictureBox[10];
        //
        //}

        //槽位选项
        static void FillBoxList(ComboBox Boxname)
        {
            //Boxname.Items.Add("None");
            Boxname.Items.Add("----大类----");
            Boxname.Items.Add("RIFLE主武器");
            Boxname.Items.Add("PISTOL副武器");
            Boxname.Items.Add("KNIFE近战武器");
            Boxname.Items.Add("UTILITY头号特训平板");
            Boxname.Items.Add("BOOSTS增益道具(防爆盾、治疗针)");
            Boxname.Items.Add("C4(包括头号特训跳雷)");
            Boxname.Items.Add("GRENADES手雷(在区域内分别排列)");
            Boxname.Items.Add("----投掷物----");
            Boxname.Items.Add("FLASHBANG闪光弹");
            Boxname.Items.Add("HEGRENADE高爆手雷");
            Boxname.Items.Add("SMOKEGRENADE烟雾弹");
            Boxname.Items.Add("DECOYGRENADE诱饵弹");
            Boxname.Items.Add("MOLOTOV燃烧弹(瓶)");
            return;//结束当前函数
        }
        //TrackBar模块
        //令滑动条的值=文本框内容
        public static void AngleMod_TrackBar(TrackBar TrackBarName, TextBox TextBoxName)
        {
            TextBoxName.Text = TrackBarName.Value.ToString();


        }
        //TextBox模块
        //令文本框内容=滑动条的值,同时最少显示0
        public static void AngleMod_TextBox(TrackBar TrackBarName, TextBox TextBoxName)
        {
            //TrackBarName.Value = TextBoxName.Text.Convert.ToDouble();
            if (TextBoxName.Text == "" || TextBoxName.Text == "-")
            {
                TextBoxName.Text = "0";
            }
            else
            {
                //try
                //{
                //    int.Parse(TextBoxName.Text);
                //}
                //catch 
                //{
                //    MessageBox.Show("请不要输入非数字字符");
                //    return;
                //}
                //if (int.Parse(TextBoxName.Text) > 20)
                //{
                //    TextBoxName.Text = "20";
                //    TrackBarName.Value = 20;
                //}
                //else if (int.Parse(TextBoxName.Text) < 0)
                //{
                //    TextBoxName.Text = "0";
                //    TrackBarName.Value = 0;
                //}
                //else
                //{
                //    TrackBarName.Value = int.Parse(TextBoxName.Text);
                //}
                bool result = int.TryParse(TextBoxName.Text, out int x);
                if (result)
                {
                    if (x > TrackBarName.Maximum)
                    {
                        TextBoxName.Text = TrackBarName.Maximum.ToString();
                        TrackBarName.Value = TrackBarName.Maximum;
                    }
                    else if (x < TrackBarName.Minimum)
                    {
                        TextBoxName.Text = TrackBarName.Minimum.ToString();
                        TrackBarName.Value = TrackBarName.Minimum;
                    }
                    else
                    {
                        TrackBarName.Value = int.Parse(TextBoxName.Text);
                    }
                }
                else
                {
                    MessageBox.Show("请不要输入非数字字符,本软件暂时不支持小数点");
                    TextBoxName.Text = TextBoxName.Text.Remove(TextBoxName.Text.Length - 1, 1);//删去输入的非数字字符
                }
            }
        }




        public customQuickInv()
        {
            InitializeComponent();
        }


        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }




        //初始加载
        private void Form1_Load(object sender, EventArgs e)
        {
            FillBoxList(comboBox_slot1);
            FillBoxList(comboBox_slot2);
            FillBoxList(comboBox_slot3);
            FillBoxList(comboBox_slot4);
            FillBoxList(comboBox_slot5);
            FillBoxList(comboBox_slot6);
            FillBoxList(comboBox_slot7);
            FillBoxList(comboBox_slot8);
            FillBoxList(comboBox_slot9);
            FillBoxList(comboBox_slot10);


            dynamicDrawing();

            //加载画图模块
            //drawCircle();



            /*
            void EnumControls(Control container)
            {
                foreach (Control c in container.Controls)
                {
                    //c is the child control here
                    EnumControls(c);

                }
            }

            //调用
            EnumControls(this);
            */






        }

        ////////////////////////槽位是否启用模块/////////////////////////////////
        public void SoltEnableMod(CheckBox checkBoxName, ComboBox comboBoxName, TrackBar trackBarName, TextBox textBoxName)
        {
            if (checkBoxName.Checked == true)
            {
                comboBoxName.Enabled = true;
                trackBarName.Enabled = true;
                textBoxName.Enabled = true;
            }
            else
            {
                comboBoxName.Enabled = false;
                trackBarName.Enabled = false;
                textBoxName.Enabled = false;
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox1, comboBox_slot1, trackBar_angle1, textBox_angle1);
            dynamicDrawing();
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox2, comboBox_slot2, trackBar_angle2, textBox_angle2);
            dynamicDrawing();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox3, comboBox_slot3, trackBar_angle3, textBox_angle3);
            dynamicDrawing();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox4, comboBox_slot4, trackBar_angle4, textBox_angle4);
            dynamicDrawing();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox5, comboBox_slot5, trackBar_angle5, textBox_angle5);
            dynamicDrawing();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox6, comboBox_slot6, trackBar_angle6, textBox_angle6);
            dynamicDrawing();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox7, comboBox_slot7, trackBar_angle7, textBox_angle7);
            dynamicDrawing();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox8, comboBox_slot8, trackBar_angle8, textBox_angle8);
            dynamicDrawing();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox9, comboBox_slot9, trackBar_angle9, textBox_angle9);
            dynamicDrawing();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox10, comboBox_slot10, trackBar_angle10, textBox_angle10);
            dynamicDrawing();
        }

        ////////////////////////槽位是否启用模块END/////////////////////////////////

        ///////////////////////////角度控件START////////////////////////////////

        private void trackBar_startAngle_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_startAngle, textBox_startAngle);
            dynamicDrawing();
        }

        private void textBox__startAngle_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_startAngle, textBox_startAngle);
            dynamicDrawing();
        }
        private void trackBar_angle1_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle1, textBox_angle1);
            dynamicDrawing();
        }

        private void AngleBox1_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle1, textBox_angle1);
            dynamicDrawing();
        }

        private void trackBar_angle2_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle2, textBox_angle2);
            dynamicDrawing();
        }

        private void AngleBox2_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle2, textBox_angle2);
            dynamicDrawing();
        }


        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle3, textBox_angle3);
            dynamicDrawing();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle3, textBox_angle3);
            dynamicDrawing();
        }

        private void trackBar_angle4_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle4, textBox_angle4);
            dynamicDrawing();
        }

        private void textBox_angle4_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle4, textBox_angle4);
            dynamicDrawing();
        }

        private void trackBar_angle5_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle5, textBox_angle5);
            dynamicDrawing();
        }

        private void textBox_angle5_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle5, textBox_angle5);
            dynamicDrawing();
        }

        private void trackBar_angle6_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle6, textBox_angle6);
            dynamicDrawing();
        }

        private void textBox_angle6_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle6, textBox_angle6);
            dynamicDrawing();
        }

        private void trackBar_angle7_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle7, textBox_angle7);
            dynamicDrawing();
        }

        private void textBox_angle7_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle7, textBox_angle7);
            dynamicDrawing();
        }

        private void trackBar_angle8_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle8, textBox_angle8);
            dynamicDrawing();
        }

        private void textBox_angle8_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle8, textBox_angle8);
            dynamicDrawing();
        }

        private void trackBar_angle9_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle9, textBox_angle9);
            dynamicDrawing();
        }

        private void textBox_angle9_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle9, textBox_angle9);
            dynamicDrawing();
        }

        private void trackBar_angle10_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle10, textBox_angle10);
            dynamicDrawing();
        }

        private void textBox_angle10_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle10, textBox_angle10);
            dynamicDrawing();
        }
        ///////////////////////////////角度控件END////////////////////////////////////



        ///////////////////////导出模块/////////////////////////////////////////////////////////



        ///////////////////////槽位转换为物品代号////////////////////////////
        //去中文以及字符
        // source: 
        //https://www.cnblogs.com/zhaogaojian/p/9207846.html
        
        
        /// <summary>
        /// 保留字符串中的数字及字母，舍去其他字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetNumberAlpha(string source)
        {
            if (source == "None")
            {
                return "/t";
            }
            else
            {
                string pattern = "[A-Za-z0-9]";
                string strRet = "";
                MatchCollection results = Regex.Matches(source, pattern);
                foreach (var v in results)
                {
                    strRet += v.ToString();
                }
                return strRet;
            }
            //}
        }

        public void OutputMod()
        {
            ComboBox[] slots ={
                comboBox_slot1,
                comboBox_slot2,
                comboBox_slot3,
                comboBox_slot4,
                comboBox_slot5,
                comboBox_slot6,
                comboBox_slot7,
                comboBox_slot8,
                comboBox_slot9,
                comboBox_slot10};

            TextBox[] angles ={
                textBox_angle1,
                textBox_angle2,
                textBox_angle3,
                textBox_angle4,
                textBox_angle5,
                textBox_angle6,
                textBox_angle7,
                textBox_angle8,
                textBox_angle9,
                textBox_angle10};





            /////////////////////////写入txt部分///////////////////////
            string path = System.Windows.Forms.Application.StartupPath + @"\radial_quickinventory.txt";
            //创建StreamWriter 类的实例
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine("\"settings\"");
            streamWriter.WriteLine("{"); //START
            streamWriter.WriteLine("\t" + "starting_angle" + "\t" + "\t" + textBox_startAngle.Text);//起始角度

            //写入各槽位名称及角度

            //                  9;
            for (int i = 0; i <= slots.Length - 1; i++)
            {
                if (slots[i].Enabled == true && Convert.ToString(slots[i].Tag) == "slots")
                {
                    streamWriter.WriteLine("\t" + GetNumberAlpha(slots[i].Text) + "\t" + "\t" + angles[i].Text);
                }
            }

            /*
            foreach (ComboBox i in slots)
            {
                if (Convert.ToString(i.Tag) == "slots" && i.Enabled == true)
                {
                    streamWriter.WriteLine("\t" + GetNumberAlpha(i.Text) + "\t" + "\t" + i.Text);slots.
                }
            }
            */

            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot1.Text) + "\t" + "\t" + textBox_angle1.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot2.Text) + "\t" + "\t" + textBox_angle2.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot3.Text) + "\t" + "\t" + textBox_angle3.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot4.Text) + "\t" + "\t" + textBox_angle4.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot5.Text) + "\t" + "\t" + textBox_angle5.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot6.Text) + "\t" + "\t" + textBox_angle6.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot7.Text) + "\t" + "\t" + textBox_angle7.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot8.Text) + "\t" + "\t" + textBox_angle8.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot9.Text) + "\t" + "\t" + textBox_angle9.Text);
            //streamWriter.WriteLine("\t" + GetNumberAlpha(comboBox_slot10.Text) + "\t" + "\t" + textBox_angle10.Text);
            streamWriter.WriteLine("}"); //END
            //刷新缓存
            streamWriter.Flush();
            //关闭流
            streamWriter.Close();
        }

        ////////////////////导出检查模块START///////////////////////

        public bool CheckMod_isSoltNotEmpty()
        {
            ComboBox[] slots ={
                comboBox_slot1,
                comboBox_slot2,
                comboBox_slot3,
                comboBox_slot4,
                comboBox_slot5,
                comboBox_slot6,
                comboBox_slot7,
                comboBox_slot8,
                comboBox_slot9,
                comboBox_slot10};

            foreach (ComboBox i in slots)
            {
                if (i.Enabled == true && (i.Text == "" | i.Text == "----投掷物----" | i.Text == "----大类----"))
                    return false;

            }
            return true;
        }
        public bool CheckMod_is360()
        {
            double angleSum = 0;
            TextBox[] angles ={
                textBox_angle1,
                textBox_angle2,
                textBox_angle3,
                textBox_angle4,
                textBox_angle5,
                textBox_angle6,
                textBox_angle7,
                textBox_angle8,
                textBox_angle9,
                textBox_angle10};

            
            foreach (TextBox i in angles)
            {
                if (Convert.ToString(i.Tag) == "angle" && i.Enabled == true)
                {
                    angleSum = angleSum + double.Parse(i.Text);
                }
            };

            common.angleSum_ = angleSum;


            
            if (angleSum == 360)

                return true;

            else

                return false;
        }
        ////////////////////导出检查模块End///////////////////////
        private void ToolStripMenuItem_导出按钮_Click(object sender, EventArgs e)
        {
            //检查错误
            if (CheckMod_is360())
            {
                if (CheckMod_isSoltNotEmpty())
                {
                    OutputMod();
                    MessageBox.Show("导出成功！已在同一目录下生成radial_quickinventory.txt" +
                        "\n将该文件替换Counter-Strike Global Offensive\\csgo中同名文件以使用你的自定义轮盘！");
                }
                else
                {
                    MessageBox.Show("导出失败！\n存在开启但未选择物品的槽位");
                }
            }
            else
            {
                MessageBox.Show("导出失败！\n所有槽位角度总和必须为360！\n当前角度和为" + common.angleSum_);
            }

        }


        ////////////////////////////////////////////////导入模块//////////////////////////////////////////////////////

        //未完成

        //文件分析模块
        //public int txtAnalyser()
        //{

        //}


        //导入按钮
        private void ToolStripMenuItem_导入按钮_Click(object sender, EventArgs e)
        {
            //选择文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "请选择文件夹";
            dialog.Filter = "文本文件(*.txt)|*.txt";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
            }
        }




        //////////////////////////////////////////////////绘图模块////////////////////////////////////////////

        //先在中间画一个圆  :  内圆
        public void drawInnerCircle()
        {

            //获取panel1宽高
            double width = panel1.Width;
            double height = panel1.Height;
            //大圆半径=0.5宽
            double halfWidth = 0.5 * width;
            //内圆半径为大圆半径0.52倍
            double innerRadius = 0.52 * halfWidth;
            double innerCircleLocation = halfWidth - innerRadius;

            //内圆矩形数据
            Rectangle innerRect = new Rectangle(Convert.ToInt32(innerCircleLocation),
                                    Convert.ToInt32(innerCircleLocation),
                                    Convert.ToInt32(innerRadius * 2),
                                    Convert.ToInt32(innerRadius * 2));


            Graphics graphics = panel1.CreateGraphics(); // 创建当前窗体的Graphics对象 
            Pen pen = new Pen(Color.Gray, 2);  // 创建灰色 粗细为2的画笔对象 
            SolidBrush solidBrushGray = new SolidBrush(Color.Gray); // 内圆填充笔刷  灰色


            //graphics.DrawRectangle(pen, 0,0,Convert.ToSingle(width), Convert.ToSingle(height));//临时用 画边界

            graphics.DrawEllipse(pen, innerRect);//画内圆边框

            //graphics.FillEllipse(solidBrushGray,innerRect);//画填充内圆    //暂时先不填充了



        }
        ///////////////再在边上画扇形



        //扇形类
        public class fan
        {
            //float fan_radius;
            public float startAngle;
            public float sweepAngle;
            public float midAngle;
        }

        /*
        public class soltPicture
        {
            
        }
        */
        //获取扇形数据

        public void drawFans()
        {


            Graphics graphicsFans = panel1.CreateGraphics();//初始化


            //graphicsFans.Clear(Color.Empty);//不太好用  必须要用一种颜色覆盖

            //画外侧圆环步骤：
            //DrawArc(Pen pen, Rectangle rect, float startAngle, float sweepAngle); 
            //1.定义笔
            //2.定义圆环内圆、外圆的外切矩形
            //3.计算圆环起始角度、经过角度
            //4,分别画出外、内侧两弧
            //5.计算端点坐标,画两条直线将其连起来
            //5.扇形完成，考虑在这一步将武器图片放上。
            //ps.有个问题，这样画的不知道应该如何填充内部颜色。

            //笔的定义挪到角度超出360判断中去了
            //Pen pen = new Pen(Color.Gray, 2);

            //计算环形内外径

            float circleOuterRadius = Convert.ToSingle(panel1.Width * 0.5);//外径为绘图区宽度 *0.5
            float circleInnerRadius = Convert.ToSingle(0.54) * circleOuterRadius;//内径为0.54倍外径

            Rectangle circleOuterRect = new Rectangle(0,
                                                    0,
                                                    Convert.ToInt32(panel1.Width),
                                                    Convert.ToInt32(panel1.Width));
            Rectangle circleInnerRect = new Rectangle(Convert.ToInt32(circleOuterRadius - circleInnerRadius),
                                                    Convert.ToInt32(circleOuterRadius - circleInnerRadius),
                                                    Convert.ToInt32(circleInnerRadius * 2),
                                                    Convert.ToInt32(circleInnerRadius * 2));
            //画矩形以test
            //result:successful!

            //graphicsFans.DrawRectangle(pen, circleInnerRect);
            //graphicsFans.DrawRectangle(pen, circleOuterRect);

            //分析扇形数据
            //angleSum为帮助计算起始角度的变量
            float angleSum = Convert.ToSingle(textBox_startAngle.Text) - 90;


            TextBox[] angles ={
                textBox_angle1,
                textBox_angle2,
                textBox_angle3,
                textBox_angle4,
                textBox_angle5,
                textBox_angle6,
                textBox_angle7,
                textBox_angle8,
                textBox_angle9,
                textBox_angle10};

            ComboBox[] slots ={
                comboBox_slot1,
                comboBox_slot2,
                comboBox_slot3,
                comboBox_slot4,
                comboBox_slot5,
                comboBox_slot6,
                comboBox_slot7,
                comboBox_slot8,
                comboBox_slot9,
                comboBox_slot10};

            float center = Convert.ToSingle(panel1.Width * 0.5);


            for (int i = 0; i <= slots.Length - 1; i++)
            {
                if (Convert.ToString(angles[i].Tag) == "angle" && angles[i].Enabled == true && angles[i].Text != "")
                {
                    fan fan1 = new fan();

                    fan1.startAngle = angleSum;
                    fan1.sweepAngle = float.Parse(angles[i].Text);
                    fan1.midAngle = fan1.startAngle + Convert.ToSingle(0.5) * fan1.sweepAngle;

                    Pen pen = new Pen(Color.Gray, 2);
                    //判断最终弧度是否超过360度
                    //是则笔改为红色
                    if (Config.isWarningRed == true)
                    {
                        if (fan1.startAngle + fan1.sweepAngle - (Convert.ToSingle(textBox_startAngle.Text) - 90) > 360)
                        {
                            pen.Color = Color.Red;
                            pen.Width = 5;
                        }
                    }

                    graphicsFans.DrawArc(pen, circleInnerRect, fan1.startAngle, fan1.sweepAngle);//画内弧
                    graphicsFans.DrawArc(pen, circleOuterRect, fan1.startAngle, fan1.sweepAngle);//画外弧



                    //计算端点坐标
                    float pointOutStartX = center + Convert.ToSingle(circleOuterRadius * Math.Cos(Math.PI / 180 * (fan1.startAngle)));
                    float pointOutStartY = center + Convert.ToSingle(circleOuterRadius * Math.Sin(Math.PI / 180 * (fan1.startAngle)));
                    float pointOutEndX = center + Convert.ToSingle(circleOuterRadius * Math.Cos(Math.PI / 180 * (fan1.startAngle + fan1.sweepAngle)));
                    float pointOutEndY = center + Convert.ToSingle(circleOuterRadius * Math.Sin(Math.PI / 180 * (fan1.startAngle + fan1.sweepAngle)));
                    float pointInStartX = center + Convert.ToSingle(circleInnerRadius * Math.Cos(Math.PI / 180 * (fan1.startAngle)));
                    float pointInStartY = center + Convert.ToSingle(circleInnerRadius * Math.Sin(Math.PI / 180 * (fan1.startAngle)));
                    float pointInEndX = center + Convert.ToSingle(circleInnerRadius * Math.Cos(Math.PI / 180 * (fan1.startAngle + fan1.sweepAngle)));
                    float pointInEndY = center + Convert.ToSingle(circleInnerRadius * Math.Sin(Math.PI / 180 * (fan1.startAngle + fan1.sweepAngle)));

                    graphicsFans.DrawLine(pen, pointOutStartX, pointOutStartY, pointInStartX, pointInStartY);//画起线
                    graphicsFans.DrawLine(pen, pointOutEndX, pointOutEndY, pointInEndX, pointInEndY);//画终线


                    ////如果最终未满360，则用蓝色弧线填满剩余部分
                    //if(Config.angleWarning_Blue == true)
                    //{
     
                    //    if (fan1.startAngle + fan1.sweepAngle - (Convert.ToSingle(textBox_startAngle.Text) - 90) < 360)
                    
                    //    {
                        
                    //        pen.Color = Color.Blue;
                        
                    //        pen.Width = 5;
                    
                    //    }
                    //}


                    ////////////////////////////////////////////////////////////////在midAngle放图片///START///////////////////////////

                    //判断是否应该放图片
                    //fan1.sweepAngle=0 ;  槽位未选择物品   时不放图片

                    if ( fan1.sweepAngle != 0 && (slots[i].Text != "" && slots[i].Text != "----投掷物----" && slots[i].Text != "----大类----"))
                    {

                    

                    //计算中间位置坐标

                    float midPointX = center + Convert.ToSingle((circleOuterRadius + circleInnerRadius) * 0.5 * Math.Cos(Math.PI / 180 * (fan1.midAngle)));
                    float midPointY = center + Convert.ToSingle((circleOuterRadius + circleInnerRadius) * 0.5 * Math.Sin(Math.PI / 180 * (fan1.midAngle)));

                    //放置一个图片框

                    PictureBox[] pictureSolts = new PictureBox[slots.Length];//声明一个图片框数组

                    pictureSolts[i] = new PictureBox(); //在循环中产生新的图片框

                    //定义图片框的属性
                    pictureSolts[i].Width = Convert.ToInt32(0.7 * (circleOuterRadius - circleInnerRadius));//暂定图片最大宽度为0.7倍扇环母线
                    pictureSolts[i].Height = Convert.ToInt32(0.5 * pictureSolts[i].Width);     //高为宽的一半
                    pictureSolts[i].Location = new Point(Convert.ToInt32(midPointX) - pictureSolts[i].Width / 2, Convert.ToInt32(midPointY) - pictureSolts[i].Height / 2);
                    //pictureSolts[i].Location = new Point(Convert.ToInt32(midPointX), Convert.ToInt32(midPointY));
                    //pictureSolts[i].BackColor = Color.Black;//debug
                    pictureSolts[i].BackColor = Color.Transparent;
                    pictureSolts[i].SizeMode = PictureBoxSizeMode.Zoom;
                    panel1.Controls.Add(pictureSolts[i]);

                    //给图片框放上图片
                    //判断应放哪种图片

                    //从资源文件.resx中读取图片    //不用这么做
                    //ResXResourceReader resxReader = new ResXResourceReader(System.Windows.Forms.Application.StartupPath + @"\Form1.resx");

                    switch (GetNumberAlpha(slots[i].Text))
                    {
                        case "PISTOL":
                            pictureSolts[i].Image = Resource1.PISTOL;

                            break;
                        case "KNIFE":
                            pictureSolts[i].Image = Resource1.KNIFE;
                            break;
                        case "RIFLE":
                            pictureSolts[i].Image = Resource1.RIFLE;
                            break;
                        case "UTILITY":
                            //pictureSolts[i].Image = Resource1.UTILITY;         //手上没有这个
                            break;
                        case "BOOSTS":
                            pictureSolts[i].Image = Resource1.BOOSTS;           //用个盾图标凑合
                            break;
                        case "C4":
                            pictureSolts[i].Image = Resource1.C4;
                            break;
                        case "GRENADES":
                            //pictureSolts[i].Image = Resource1.GRENADES;       //手上没有这个
                            break;
                        case "FLASHBANG":
                            pictureSolts[i].Image = Resource1.FLASHBANG;
                            break;
                        case "HEGRENADE":
                            pictureSolts[i].Image = Resource1.HEGRENADE;
                            break;
                        case "SMOKEGRENADE":
                            pictureSolts[i].Image = Resource1.SMOKEGRENADE;
                            break;
                        case "DECOYGRENADE":
                            pictureSolts[i].Image = Resource1.DECOYGRENADE;
                            break;
                        case "MOLOTOV":
                            pictureSolts[i].Image = Resource1.MOLOTOV;
                            break;

                    
                    }

                
                    }













                    ////////////////////////////////////////////////////////////////////放图片END////////////////////////////



                    angleSum = fan1.startAngle + fan1.sweepAngle;

                    //        angleSum = angleSum + float.Parse(i.Text);
                }
            }
        }
        
        /////////////////////////////////外圈绘制部分结束/////////////////////////////////////////
        /// <summary>
        /// 动态绘制函数，本软件中用于绘制panel1中内容
        /// </summary>
        public void dynamicDrawing()
        {
            if (Config.isDynamicDrawing == true)
            {
            clearCanva(panel1);//先清空画布
            drawInnerCircle();//画内圈
            drawFans();//画外部圆环扇形
            }

        }
        //debug按钮
        private void button1_Click(object sender, EventArgs e)
        {
            drawInnerCircle();
        }
        //debug按钮
        private void button2_Click(object sender, EventArgs e)
        {
            drawFans();
        }
        
        //debug按钮
        private void button3_Click(object sender, EventArgs e)
        {
            //先清除之前画的
            clearCanva(panel1);
        }

        /// <summary>
        /// 清除画布的函数，后跟panel名
        /// </summary>
        /// <param name="panelName"></param>
        public void clearCanva(Panel panelName)
        {
            Graphics g = panelName.CreateGraphics();//这个方法也很有问题...?
            g.Clear(this.BackColor);

            //panelName.Invalidate();//不用这个方法了，非常容易引起问题
            //仅循环清除图片而不循环清除绘图区，防止瞎眼和图形错误
            clearPictures();
            void clearPictures(){
            foreach (Control c in panelName.Controls)
                {
                    //if(c is PictureBox)
                    //{
                        panelName.Controls.Remove(c);

                        clearPictures();//再来一次，防止清不干净
                    //}
                }
            }
        }

        private void comboBox_slot10_TextChanged(object sender, EventArgs e)
        {
            dynamicDrawing();
        }

        private void checkBox_isDynamicDrawing_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_isDynamicDrawing.Checked == true)
            {
                Config.isDynamicDrawing = true;
            }
            else
            {
                Config.isDynamicDrawing = false;
            }
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("感谢使用本软件！" +
                "\n本软件仅供本C#初学者练手，如引起使用过程中的不舒服，尽情谅解" +
                "\nCSGO-customWeaponWheelVisuableEditor Version 0.8.0" +               
                "\nWelcome to visit my website at httpS://blog.mofengfeng.com  !"+
                "\nCreated By: Mofeng");
        }

        private void HaveNotDone_Click(object sender, EventArgs e)
        {
            MessageBox.Show("还没有做好！\n或许并用不到这些功能......");
        }

        private void MouseHover_HelpOfStartAngle(object sender, EventArgs e)
        {
            ToolTip toolTip_startAngle = new ToolTip();
            toolTip_startAngle.AutoPopDelay = 5000;
            toolTip_startAngle.InitialDelay = 200;
            toolTip_startAngle.ReshowDelay = 200;
            toolTip_startAngle.SetToolTip(label_startAngle, "0为正北（上方），180为正南（下方），顺时针旋转");
        }

        private void checkBox_isWarningRed_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_isWarningRed.Checked == true)
            {
                Config.isWarningRed = true;
            }
            else
            {
                Config.isWarningRed = false;
            }
            dynamicDrawing();
        }

        private void checkBox_isRelative_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_isRelative.Checked == true)
            {
                Config.isRelative = true;
            }
            else
            {
                Config.isRelative = false;
            }
        }

        
        ////////////////////////////////////////////////////////////////
        
        public class FixedAngle
        {
            public ArrayList originalAngle = new ArrayList();
            public ArrayList fixedAngleSum = new ArrayList();
        }
        /// <summary>
        /// 生成一个FixedAngle类，储存各槽位原始角度以及所需角度和
        /// </summary>
        public void getFixedAngle()
        {
            FixedAngle fixed1 = new FixedAngle() //初始化所需类
            {

            };
            TextBox[] angleBoxes ={
                textBox_angle1,
                textBox_angle2,
                textBox_angle3,
                textBox_angle4,
                textBox_angle5,
                textBox_angle6,
                textBox_angle7,
                textBox_angle8,
                textBox_angle9,
                textBox_angle10};

            
            //int i = 0;
            int lastAngle = 0;
            foreach ( TextBox t in angleBoxes)
            {
                if (t.Enabled == true)
                {
                    fixed1.originalAngle.Add(Convert.ToInt32(t.Text));//保存每个槽位的原始角度

                    int currentAngle = Convert.ToInt32(t.Text);
                    fixed1.fixedAngleSum.Add(currentAngle + lastAngle);
                    lastAngle = currentAngle;
                    
                }
            }



            fixed1.fixedAngleSum.Remove(0);
            fixed1.fixedAngleSum.Add(fixed1.fixedAngleSum[fixed1.fixedAngleSum.Count - 1]);//新增最后一项，其值等于原来的最后一项

            
        }

        /// <summary>
        /// 令当前项角度与下一项角度 之和 不发生变化。若当前为最后一项，则与前一项进行调整。
        /// </summary>
        /// 
        public void keepAngleSumFixed()
        {
            TextBox[] angleBoxes ={
                textBox_angle1,
                textBox_angle2,
                textBox_angle3,
                textBox_angle4,
                textBox_angle5,
                textBox_angle6,
                textBox_angle7,
                textBox_angle8,
                textBox_angle9,
                textBox_angle10};

            ArrayList currentAngle = new ArrayList();

            foreach (TextBox t in angleBoxes)
            {
                if (t.Enabled == true)
                {
                    currentAngle.Add(Convert.ToInt32(t.Text));//保存每个槽位的现有角度

                    

                }
            }

            //对比角度是否发生变化

            for (int i = 0;i <= currentAngle.Count -1; i++ )
            {
                //if(currentAngle[i] != fixed1. )
                {

                }
            }


        }
    }
}


