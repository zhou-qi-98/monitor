using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;//线程引用集
using System.IO.Ports;//串口引用集

using SharpExModule.Modbus;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Timers;


namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 声明
        public SerialPort ModbusRTU = new SerialPort();
        Thread UPDATA_TIME_THREAD;//线程函数
        string strTime = "[" + System.DateTime.Now.ToString("HH:mm:ss") + "] ";
        string TEMP_DATA, HUM_DATA, AQI_DATA;

        #endregion
        long _start;
        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;//控制弹窗在正中间

            /*时间*/
            DispatcherTimer timer = new DispatcherTimer();
            _start = Environment.TickCount;
            timer.Tick += timer_Tick;
            timer.IsEnabled = true;

        }


        #region 串口接收
        private void Serial_Received(object sender, SerialDataReceivedEventArgs e)
        {
            string redata;
            redata = ModbusRTU.ReadLine();
            Thread.Sleep(10);//线程休眠
            /*解析数据*/
            /*---数据样例：20，40，51*/
            List<string> list = new List<string>(redata.Split(','));//以逗号分隔字符串
            /*解析数据 并取出数据到变量当中*/
            TEMP_DATA = list[0];
            HUM_DATA = list[1];
            AQI_DATA = list[2];      

        }
        #endregion

        #region 串口打开
        private void UI_port_btn_Click(object sender, RoutedEventArgs e)
        {
            if (UI_port_btn.Content.ToString() == "打开")
            {
                try//尝试执行，避免软件崩溃
                {
                    if (UI_port_com.SelectedIndex != -1)//下拉列表未选中串口
                    {
                        if (!ModbusRTU.IsOpen)
                        {                            
                            ModbusRTU.PortName = UI_port_com.SelectedValue.ToString();
                            ModbusRTU.BaudRate = int.Parse("115200");//波特率
                            ModbusRTU.DataBits = int.Parse("8");//数据位
                            ModbusRTU.StopBits = StopBits.One;//停止位
                            ModbusRTU.Parity = Parity.None;//校验位
                            ModbusRTU.ReadTimeout = -1;//超时读取时间
                            ModbusRTU.Open();
                            ModbusRTU.DataReceived += new SerialDataReceivedEventHandler(Serial_Received);
                            UI_port_btn.Content = "关闭";
                         // EXCHANGE_DATA.Text = "";
                            ModbusRTU.WriteLine("REA,VERSION,1,END");
                        }
                    }
                    else
                    {
                        MessageBoxResult result = MessageBox.Show("未选择正确的串口号，请检查后重试", "提示信息", MessageBoxButton.YesNo);
                    }
                }
                catch (Exception exc)//异常处理
                {
                    MessageBoxResult result = MessageBox.Show("当前串口可能被占用，或未选择正确的串口号，请检查后重试", "错误信息", MessageBoxButton.YesNo);
                }
            }
            else
            {
                UI_port_btn.Content = "打开";
                ModbusRTU.Close();
                //EXCHANGE_DATA.Text = "";
            }
        }
        #endregion

        #region 扫描串口
        private void UI_port_com_DropDownOpened(object sender, EventArgs e)
        {
            //刷新串口
            UI_port_com.Items.Clear();
            string[] arrpotr = SerialPort.GetPortNames();//加载电脑串口复制给数组arrport
            foreach (string item in arrpotr)   //遍历数据  添加到下拉列表
            {
                UI_port_com.Items.Add(item);
            }
            UI_port_com.SelectedIndex = 0;//设置该下拉框默认选中第一项
        }
        #endregion

        #region 显示时间
        void timer_Tick(object sender, EventArgs e)
        {
            UI_Time.Text = DateTime.Now.ToString("yyyy-dd-MM HH:mm:ss");
        }
        #endregion

        #region 引用控件
        private void UI_port_com_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        #endregion
        #region 窗口初始化事件
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            string[] arrpotr = SerialPort.GetPortNames();//加载电脑串口复制给数组arrport
            foreach (string item in arrpotr)   //遍历数据  添加到下拉列表
            {
                UI_port_com.Items.Add(item);
            }
            UI_port_com.SelectedIndex = 0;//设置该下拉框默认选中第一项
        }
        #endregion

        



        

        #region 页面切换按钮
        private void UI_BTN_MIAN_Click(object sender, RoutedEventArgs e)
        {
          //  Frame1.Navigate(new Uri("/Main.xaml", UriKind.RelativeOrAbsolute));
        }
        #endregion
    }
}
