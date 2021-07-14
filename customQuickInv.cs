using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
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
            public static bool isDynamicDrawing = true;     //动态绘制


            //public static bool angleWarning_Blue = true;  //没做好，可能不做了
            public static bool isWarningRed = true;         //超出360时红线警告
            public static bool isKeepAngleSumFixed = false; //相对调整


            //auto360模块
            public static bool isAuto360 = false;


            public static bool isAdjustEnd = false;         //末端调整
            public static bool isAverageAngles = false;     //角度均分

            //自动重绘事件

            public static bool isRePaint = true;




        }

        //存放其他全局变量
        public class common
        {

            //用于导出检查模块中
            public static double angleSum_ = 0;



            //用于相对调整中
            public static List<int> originalAngle = new List<int>();
            public static List<int> fixedAngleSum = new List<int>();




        }



        //槽位选项
        static void FillBoxList(ComboBox Boxname)
        {
            //Boxname.Items.Add("None");
            Boxname.Items.Add("----大类----");
            Boxname.Items.Add("RIFLE主武器");
            Boxname.Items.Add("PISTOL副武器");
            Boxname.Items.Add("KNIFE近战武器(包括电击枪)");
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
        /// <summary>
        /// 放置于TrackBar的功能下，令 所指定的 文本框内容 为 所指定的 滑动条的值所转换成的字符串
        /// </summary>
        /// <param name="TrackBarName"></param>
        /// <param name="TextBoxName"></param>
        public static void AngleMod_TrackBar(TrackBar TrackBarName, TextBox TextBoxName)
        {
            TextBoxName.Text = TrackBarName.Value.ToString();


        }
        //TextBox模块
        /// <summary>
        /// 放置于TextBox的功能下，令 所指定的 滑动条的值 为 经过处理后 的 所指定的 文本框内容所转换成的int32.
        /// </summary>
        /// <param name="TrackBarName"></param>
        /// <param name="TextBoxName"></param>
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



            /////////////////////////tooltips模块///////////////////////////////////
            ToolTip toolTip_startAngle = new ToolTip();
            toolTip_startAngle.AutoPopDelay = 10000;
            toolTip_startAngle.InitialDelay = 200;
            toolTip_startAngle.ReshowDelay = 200;
            //左侧
            toolTip_startAngle.SetToolTip(label_startAngle, "0为正北（上方），180为正南（下方），顺时针旋转");
            //控制区
            toolTip_startAngle.SetToolTip(checkBox_isRelative, "开启时禁用新增/删除槽位 以及 调整槽位最大角度功能" +
                "\n令当前项角度与下一项角度 之和 不发生变化。若当前为最后一项，则与前一项进行调整" +
                "\n若调整后，被动调整项数值将小于0，则本次主动调整无效，对于细小调节请手动输入数字");
            toolTip_startAngle.SetToolTip(checkBox_isDynamicDrawing, "每当角度、槽位选项改变时，重新绘制轮盘");
            toolTip_startAngle.SetToolTip(radioButton_AdjustEnd, "若角度和未满360，则增加最后槽位的角度，使和为360" +
                "\n若角度和超过360，仅保留第一个到达360的槽位，舍去其后面的所有槽位（令其角度为0）");
            toolTip_startAngle.SetToolTip(label_MaxAngle, "调整各槽位最大角度" +
                "\n当相对调整和自动调整360启用时，禁用本功能");
            toolTip_startAngle.SetToolTip(checkBox_AutoRePaint, "开启本项可防止轮盘图形意外消失，但会减慢绘图速度" +
                "\n关闭本项可增加绘图速度，但轮盘图形可能会意外消失");
            toolTip_startAngle.SetToolTip(checkBox_isAuto360, "开启时禁用调整槽位最大角度功能");
            //toolTip_startAngle.SetToolTip(label_startAngle, "0为正北（上方），180为正南（下方），顺时针旋转");


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
            AngleAssists();
            dynamicDrawing();
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            SoltEnableMod(checkBox2, comboBox_slot2, trackBar_angle2, textBox_angle2);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox3, comboBox_slot3, trackBar_angle3, textBox_angle3);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox4, comboBox_slot4, trackBar_angle4, textBox_angle4);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox5, comboBox_slot5, trackBar_angle5, textBox_angle5);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox6, comboBox_slot6, trackBar_angle6, textBox_angle6);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox7, comboBox_slot7, trackBar_angle7, textBox_angle7);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox8, comboBox_slot8, trackBar_angle8, textBox_angle8);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox9, comboBox_slot9, trackBar_angle9, textBox_angle9);
            AngleAssists();
            dynamicDrawing();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            SoltEnableMod(checkBox10, comboBox_slot10, trackBar_angle10, textBox_angle10);
            AngleAssists();
            dynamicDrawing();
        }

        ////////////////////////槽位是否启用模块END/////////////////////////////////


        ////////////////////////角度辅助调整各功能///////////////////////////////
        public void AngleAssists()
        {
            if(Config.isAdjustEnd == true)
            {
                Auto360_AdjustEnd();

            }
            if (Config.isAverageAngles == true)
            {
                Auto360_AverageAngles();

            }
            if (Config.isKeepAngleSumFixed == true)
            {
                keepAngleSumFixed();
            }
        }
        ///////////////////////////角度控件START////////////////////////////////

        private void trackBar_startAngle_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_startAngle, textBox_startAngle);
            dynamicDrawing();
           // AngleAssists();
        }

        private void textBox__startAngle_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_startAngle, textBox_startAngle);
            dynamicDrawing();
           // AngleAssists();
        }
        private void trackBar_angle1_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle1, textBox_angle1);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void AngleBox1_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle1, textBox_angle1);

            AngleAssists();
            dynamicDrawing();
            
        }

        private void trackBar_angle2_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle2, textBox_angle2);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void AngleBox2_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle2, textBox_angle2);
            AngleAssists();
            dynamicDrawing();

            
        }


        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle3, textBox_angle3);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle3, textBox_angle3);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle4_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle4, textBox_angle4);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle4_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle4, textBox_angle4);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle5_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle5, textBox_angle5);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle5_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle5, textBox_angle5);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle6_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle6, textBox_angle6);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle6_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle6, textBox_angle6);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle7_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle7, textBox_angle7);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle7_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle7, textBox_angle7);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle8_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle8, textBox_angle8);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle8_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle8, textBox_angle8);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle9_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle9, textBox_angle9);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle9_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle9, textBox_angle9);
            AngleAssists();
            dynamicDrawing();
        }

        private void trackBar_angle10_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_angle10, textBox_angle10);

            //AngleAssists();
            //dynamicDrawing();
        }

        private void textBox_angle10_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_angle10, textBox_angle10);
            AngleAssists();
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
            //4.分别画出外、内侧两弧
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

                    //void midPictures()
                    //{

                    
                    if (fan1.sweepAngle != 0 && (slots[i].Text != "" && slots[i].Text != "----投掷物----" && slots[i].Text != "----大类----"))
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
                                pictureSolts[i].Image = Resource1.UTILITY;         //手上没有这个  //有了
                                break;
                            case "BOOSTS":
                                pictureSolts[i].Image = Resource1.BOOSTS;           //用个盾图标凑合 //有了
                                break;
                            case "C4":
                                pictureSolts[i].Image = Resource1.C4;
                                break;
                            case "GRENADES":
                                pictureSolts[i].Image = Resource1.GRENADES;       //手上没有这个 //有了
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

                       // }
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
            void clearPictures()
            {
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
                dynamicDrawing();
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
                "\nCSGO-customWeaponWheelVisuableEditor Version 0.8.6" +
                "\nWelcome to visit my website at httpS://blog.mofengfeng.com  !" +
                "\nCreated By: Mofeng");
        }

        private void HaveNotDone_Click(object sender, EventArgs e)
        {
            MessageBox.Show("还没有做好！\n或许并用不到这些功能......");
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

                checkBox_isAuto360.Checked = false;//禁止与auto360同时使用
      
                Config.isKeepAngleSumFixed = true;

                getFixedAngle();//开启时储存当前各角度信息



            }
            else
            {
                Config.isKeepAngleSumFixed = false;

            }
            enOrDisSlots();
            enOrDisMaxAgnle();
        }
        void enOrDisSlots()
        {
            CheckBox[] checkbox_Slots =
               {
                checkBox1,
                checkBox2,
                checkBox3,
                checkBox4,
                checkBox5,
                checkBox6,
                checkBox7,
                checkBox8,
                checkBox9,
                checkBox10
            };
            foreach (CheckBox t in checkbox_Slots)
            {
                if (checkBox_isRelative.Checked == true)
                {
                    t.Enabled = false;
                }
                if (checkBox_isRelative.Checked == false)
                {
                    t.Enabled = true;
                }
            }            
        }

        /////////////////////////////////相对调整模块//START///////////////////////////////////////////////


        /// <summary>
        /// 生成一个FixedAngle类，储存各槽位原始角度以及所需角度和
        /// </summary>
        public void getFixedAngle()
        {
            common.originalAngle.Clear();
            common.fixedAngleSum.Clear();

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
            foreach (TextBox t in angleBoxes)
            {
                if (t.Enabled == true)
                {
                    common.originalAngle.Add(Convert.ToInt32(t.Text));//保存每个槽位的原始角度

                    int currentAngle = Convert.ToInt32(t.Text);
                    common.fixedAngleSum.Add(currentAngle + lastAngle);
                    lastAngle = currentAngle;

                }
            }



            common.fixedAngleSum.RemoveAt(0);
            common.fixedAngleSum.Add(common.fixedAngleSum[common.fixedAngleSum.Count - 1]);//新增最后一项，其值等于原来的最后一项
            

        }

        /// <summary>
        /// 令当前项角度与下一项角度 之和 不发生变化。若当前为最后一项，则与前一项进行调整。
        /// </summary>
        public void keepAngleSumFixed()
        {
            //开关挪到AngleAssists函数里了
            
            //if (Config.isKeepAngleSumFixed == true)
            //{


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

                //List<int> currentAngle = new List<int>();
                List<TextBox> activeAngleBoxes = new List<TextBox>();

                foreach (TextBox t in angleBoxes)
                {
                    if (t.Enabled == true)
                    {
                        //currentAngle.Add(Convert.ToInt32(t.Text));//保存每个槽位的现有角度

                        activeAngleBoxes.Add(t);//生成active角度文本框List


                    }
                }

            //对比角度是否发生变化

            for (int i = 0; i <= activeAngleBoxes.Count - 1; i++)
            {
                if (Convert.ToInt32(activeAngleBoxes[i].Text) != common.originalAngle[i])
                {
                    if (common.fixedAngleSum[i] - Convert.ToInt32(activeAngleBoxes[i].Text) < 0) //防止减出负数，影响相隔项
                    {
                        activeAngleBoxes[i].Text = Convert.ToString(common.fixedAngleSum[i]);//若减出负数，则令本项为最大值，且不可继续调整
                        break;
                    }
                    else
                    {

                    
                        if (i < activeAngleBoxes.Count - 1)
                        {
                            activeAngleBoxes[i + 1].Text = Convert.ToString(common.fixedAngleSum[i] - Convert.ToInt32(activeAngleBoxes[i].Text));
                            getFixedAngle();//重获各角度信息
                            break;
                        }
                        if (i == activeAngleBoxes.Count - 1)//末位则对前一项进行调整
                        {
                            activeAngleBoxes[i - 1].Text = Convert.ToString(common.fixedAngleSum[i] - Convert.ToInt32(activeAngleBoxes[i].Text));
                            getFixedAngle();//重获各角度信息
                            break;
                        }
                    }
                }

            }



            //}
        }


        /////////////////////////////////相对调整模块//END///////////////////////////////////////////////


        //自动调整360//
        private void checkBox_isAuto360_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_isAuto360.Checked == true)
            {
                checkBox_isRelative.Checked = false;//禁止与 相对调整 同时使用

                Config.isAuto360 = true;
                radioButton_AdjustEnd.Enabled = true;
                radioButton_AverageAngles.Enabled = true;
            }
            else
            {
                Config.isAuto360 = false;
                radioButton_AdjustEnd.Enabled = false;
                radioButton_AdjustEnd.Checked = false;
                radioButton_AverageAngles.Enabled = false;
                radioButton_AverageAngles.Checked = false;
            }
            enOrDisMaxAgnle();
        }

        private void radioButton_AdjustEnd_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_AdjustEnd.Checked == true)
            {
                Config.isAdjustEnd = true;
                Auto360_AdjustEnd();
            }
            else
            {
                Config.isAdjustEnd = false;
            }
        }

        private void radioButton_AverageAngles_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_AverageAngles.Checked == true)
            {
                Config.isAverageAngles = true;
                Auto360_AverageAngles();
            }
            else
            {
                Config.isAverageAngles = false;
            }
        }
        /////////////////////////末端调整/补全///////////////////////
        public void Auto360_AdjustEnd()
        {
            //首先获取角度和，
            //分两条路线
            //1.角度和 < 360
            //则对最后一项进行调整，增加其值
            //2.角度和 > 360
            //则找出超出360的第一个槽位，减少其值，然后令其后面的项 角度值=0

            //开关挪到AngleAssists函数里了
            //if (Config.isAdjustEnd == true)
            //{
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


                List<TextBox> enabledAngleBoxes = new List<TextBox>();
                List<int> angleSum = new List<int>();


                //获取angleSum
                /*
                for (int i=0; i<= angleBoxes - 1; i++)
                {

                }
                */
                int i = 1;
                angleSum.Add(0);
                foreach (TextBox t in angleBoxes)
                {
                    if (t.Enabled == true)
                    {
                        angleSum.Add(Convert.ToInt32(t.Text));//angleSum[i]
                        enabledAngleBoxes.Add(t);
                        angleSum[i] = angleSum[i - 1] + angleSum[i];
                        i++;
                        ;
                    }
                }
                angleSum.Remove(0);
                i = i - 2;//使i为最后一项的索引
                
                //已获取angleSum,对其进行分析
                if (angleSum[i] - 360 < 0)  //未满3660
                {
                    enabledAngleBoxes[i].Text = Convert.ToString(Convert.ToInt32(enabledAngleBoxes[i].Text) - (angleSum[i] - 360));
                }
                if (angleSum[i] - 360 > 0)  //超出360
                {
                    int k = angleSum.FindIndex((j) => { return j > 360; });//找到>360的第一项的索引位置
                    enabledAngleBoxes[k].Text = Convert.ToString(Convert.ToInt32(enabledAngleBoxes[k].Text) - (angleSum[k] - 360 ));
                    for (; k < enabledAngleBoxes.Count - 1; k++)
                    {
                        enabledAngleBoxes[k+1].Text = "0";
                    }
                }
                
            //}
        }

        /////////////////////////均分模式/////////////////////////////
        /// <summary>
        /// 若Config.isAverageAngles状态为开启，则使各槽位角度进行均分调整
        /// </summary>
        public void Auto360_AverageAngles()
        {

            //开关挪到AngleAssists函数里了
            //if (Config.isAverageAngles == true)
            //{

            
            //先获取有几个槽位是开着的，然后进行均分。
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
            List<TextBox> enabledAngleBoxes = new List<TextBox>();
            foreach (TextBox t in angleBoxes)
            {
                if (t.Enabled == true)
                {
                    enabledAngleBoxes.Add(t);
                }
            }
            //则总数为 list .count
            if (enabledAngleBoxes.Count != 7)
            {
                foreach (TextBox t in enabledAngleBoxes)
                {
                    t.Text = Convert.ToString(360 / enabledAngleBoxes.Count);
                }
            }
            else
            {
                    //7之外都没有小数，简单粗暴的直接分配，懒得写函数了
                    enabledAngleBoxes[0].Text = "51";
                    enabledAngleBoxes[1].Text = "52";
                    enabledAngleBoxes[2].Text = "51";
                    enabledAngleBoxes[3].Text = "52";
                    enabledAngleBoxes[4].Text = "51";
                    enabledAngleBoxes[5].Text = "52";
                    enabledAngleBoxes[6].Text = "51";
                    //预想中的简单算法：
                    //先取得商和余数，然后从头或从尾分别令余数项角度值 +1
                    //稍微进阶一点：
                    //先从头开始令奇数项+1，若还有剩余则从头开始令偶数项+1
                }

            //}
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (Config.isRePaint)
            {
            
                RePaint();//解决了外框消失的问题，但会二次放置图片，显著拖慢速度 ,需要把图片放置从drawFans函数中抽离
           
            }
            void RePaint()
            {
                drawFans();
                drawInnerCircle();
            }
        }

        private void trackBar_MaxAngle_Scroll(object sender, EventArgs e)
        {
            AngleMod_TrackBar(trackBar_MaxAngle, textBox_MaxAngle) ;
        }

        private void textBox_MaxAngle_TextChanged(object sender, EventArgs e)
        {
            AngleMod_TextBox(trackBar_MaxAngle, textBox_MaxAngle);
            TrackBar[] trackBars_Angle =
            {
                trackBar_angle1,
                trackBar_angle2,
                trackBar_angle3,
                trackBar_angle4,
                trackBar_angle5,
                trackBar_angle6,
                trackBar_angle7,
                trackBar_angle8,
                trackBar_angle9,
                trackBar_angle10
            };
            foreach (TrackBar t in trackBars_Angle)
            {
                t.Maximum = trackBar_MaxAngle.Value;
            }
            
        }
        public void enOrDisMaxAgnle()
        {
            if (checkBox_isAuto360.Checked || checkBox_isRelative.Checked)
            {
                trackBar_MaxAngle.Enabled = false;
                textBox_MaxAngle.Enabled = false;
            }
            else
            {
                trackBar_MaxAngle.Enabled = true;
                textBox_MaxAngle.Enabled = true;
            }
        }

        private void checkBox_AutoRePaint_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_AutoRePaint.Checked == true)
            {
                Config.isRePaint = true;
                dynamicDrawing();
            }
            else
            {
                Config.isRePaint = false;
            }
            
        }
    }
}


