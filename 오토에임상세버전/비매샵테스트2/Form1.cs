using Hook;
using Microsoft.Diagnostics.Tracing;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;
using Microsoft.Win32;
using NetLimiter.Service;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Media;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace 비매샵테스트
{
    public partial class Form1 : Form
    {
        Object lockObj = new object();
        Point totalResultPoint = new Point(int.MinValue , -1);

        public static IntPtr Form1Handle = IntPtr.Zero;

        public static string grants = "none";
        public static string settings = "none";
        public static string id = null, password = null;
        public static string suddenRootpath = null;
        public static string fullpath = "";
       
        public static int printWindowFlag = 0x3;

        private Point firstPoint = new Point();

        public static int autoaim_Xaxis = 60;
        public static int autoaim_Yaxis = 20;
        public static int autoaim_colorRange = 30;
        public static int autoaim_Xoff = 3;
        public static int autoaim_Yoff = 3;

        public static int autoaim_mouseButton = 0;
        public static int dance_bug = 0;
        public static int total_dance_number = 0;
        

        

        public static bool autoaim_YaxisOff = false;
        public static int autoaim_mouseSpeed분자 = 1;
        public static int autoaim_mouseSpeed분모 = 2;
        public static double autoaim_mouseSpeed = 0.5;
        public static int autoaim_YmouseSpeed분자 = 1;
        public static int autoaim_YmouseSpeed분모 = 2;
        public static double autoaim_YmouseSpeed = 0.5;
        
        public static int mouseStep = 1;
        public static int mouseSpeed = 15;
        public static int mouseType = 0;
        public static int screenCaptureType = 0;
        public static double gammaValue = 0.2;
        public static int colorType = 0;

        public int teamNumber = 0;

        
        bool logging = false;

        bool bool_ingame = false;

        public static string dance_name = "";
        
        

        public static List<RGBDATA> red_RGB = new List<RGBDATA>();
        public static List<RGBDATA> blue_RGB = new List<RGBDATA>();
        public static List<HSVDATA> red_HSV = new List<HSVDATA>();
        public static List<HSVDATA> blue_HSV = new List<HSVDATA>();

        bool bool_rightMouse = false;

                [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

         [System.Runtime.InteropServices.DllImport("User32", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, Input[] pInputs, int cbSize);
        

        [DllImport("user32.dll")]
        private static extern IntPtr GetMessageExtraInfo();

        private delegate void del_findEnemy();
        private delegate bool del_isColor(byte r, byte g, byte b);
        private delegate void del_mouse(int x,int y);
        del_isColor isColor;
        del_findEnemy findEnemy;
        del_mouse mousemoveTest;
        public struct Input
        {
            public int type;
            public InputUnion u;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)] public MouseInput mi;
            [FieldOffset(0)] public KeyboardInput ki;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseInput
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardInput
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }
        static int cbSize = Marshal.SizeOf(typeof(Input));


        TraceEventSession KernelSession;
        ETWTraceEventSource KernelSource;
        KernelTraceEventParser KernelParser;


        Thread thread화면따기;
        Thread thread_메인 = null;
        

        static Input[] mouseMove = new Input[]
       {
            new Input
            {
                type = 0,
                u = new InputUnion
                {
                    mi = new MouseInput
                    {
                        dx = 0,
                        dy = 0,
                        dwFlags = (uint)(1),        // none = 0 , mousemove = 1
                        dwExtraInfo = GetMessageExtraInfo()
                    }
                }
            }
       };

        
            
      





        public Form1()
        {
            InitializeComponent();
            

            notifyIcon1.Visible = false;

            

            findEnemy = findEnemy_CopyFromScreen;
            mousemoveTest = simpleMouseMove;



            _mevent = new MoveInfo
		    {
			    MovementSettings = (InternCaseMoveSettings)8196,
			    xAmount = 0,
			    yAmount = 0,
		    };

            OpenCvSharp.Cv2.Normalize(Blurkernel, Blurkernel, 1,0);
            sharpkernel = new OpenCvSharp.Mat(3 , 3 , OpenCvSharp.MatType.CV_32F,sharpdata);
        }

        static Random random = new Random();

        static void smoothMouseMove(int x, int y) 
        {
            
            double randomSpeed = Math.Max((random.Next(mouseSpeed) / 2.0 + mouseSpeed) / 10.0, 0.1);

            WindMouse(4000, 3000, 4000+x, 3000+y, 9.0, 3.0, 10.0 / randomSpeed,15.0 / randomSpeed, 10.0 * randomSpeed, 10.0 * randomSpeed); 
        }

         static void WindMouse(double xs, double ys, double xe, double ye,
        double gravity, double wind, double minWait, double maxWait,
        double maxStep, double targetArea)
                {

            double dist, windX = 0, windY = 0, veloX = 0, veloY = 0, randomDist, veloMag, step;
            int oldX, oldY, newX = (int)Math.Round(xs), newY = (int)Math.Round(ys);

            double waitDiff = maxWait - minWait;
            double sqrt2 = Math.Sqrt(2.0);
            double sqrt3 = Math.Sqrt(3.0);
            double sqrt5 = Math.Sqrt(5.0);

            dist = Hypot(xe - xs, ye - ys);

            while (dist > 1.0) {

                wind = Math.Min(wind, dist);

                if (dist >= targetArea) {
                    int w = random.Next((int)Math.Round(wind) * 2 + 1);
                    windX = windX / sqrt3 + (w - wind) / sqrt5;
                    windY = windY / sqrt3 + (w - wind) / sqrt5;
                }
                else {
                    windX = windX / sqrt2;
                    windY = windY / sqrt2;
                    if (maxStep < 3)
                        maxStep = random.Next(3) + 3.0;
                    else
                        maxStep = maxStep / sqrt5;
                }

                veloX += windX;
                veloY += windY;
                veloX = veloX + gravity * (xe - xs) / dist;
                veloY = veloY + gravity * (ye - ys) / dist;

                if (Hypot(veloX, veloY) > maxStep) {
                    randomDist = maxStep / 2.0 + random.Next((int)Math.Round(maxStep) / 2);
                    veloMag = Hypot(veloX, veloY);
                    veloX = (veloX / veloMag) * randomDist;
                    veloY = (veloY / veloMag) * randomDist;
                }

                oldX = (int)Math.Round(xs);
                oldY = (int)Math.Round(ys);
                xs += veloX;
                ys += veloY;
                dist = Hypot(xe - xs, ye - ys);
                newX = (int)Math.Round(xs);
                newY = (int)Math.Round(ys);

                if (oldX != newX || oldY != newY)
                { 
                    mouseMove[0].u.mi.dx = newX - oldX;
                    mouseMove[0].u.mi.dy = newY - oldY;
                    SendInput(1, mouseMove, cbSize);
   
                }

                step = Hypot(xs - oldX, ys - oldY);
                int wait = (int)Math.Round(waitDiff * (step / maxStep) + minWait);
                Thread.Sleep(wait);
            }

            int endX = (int)Math.Round(xe);
            int endY = (int)Math.Round(ye);
            if (endX != newX || endY != newY)
            { 
                   mouseMove[0].u.mi.dx = endX - newX;
                   mouseMove[0].u.mi.dy = endY - newY;
                   SendInput(1, mouseMove, cbSize);
            }
        }

        static double Hypot(double dx, double dy) {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public void toSimpleMouseMove()
        {
            mousemoveTest = simpleMouseMove;
        }
        public void toSmoothMouseMove()
        {
            mousemoveTest = smoothMouseMove;
        }
        public void toInjectMouseMove()
        {
            mousemoveTest = move3;
        }



            public struct MoveInfo
	{
		public int xAmount;
		public int yAmount;
		public uint MovePack;
		public InternCaseMoveSettings MovementSettings;
		public uint Waittimems;
		public IntPtr Information;
	}

	public enum InternCaseMoveSettings
	{
		LeftDown = 2,
		LeftUp = 4,
		Move = 1,
		MoveNoCoalesce = 8192
	}

	public static class CaseExecute
	{
		public static void ExecuteMovementCase(MoveInfo input)
		{
			CaseExecute.ExecuteMovementCase(new MoveInfo[]
			{
				input
			});
		}

		public static void ExecuteMovementCase(MoveInfo[] inputs)
		{
            Execute.InjectMouseInput(inputs, inputs.Length);
		}
	}


    private MoveInfo _mevent;

    public void move3(int x, int y)
        {
             if(mouseStep == 1) { 

                                Move(x,y);
                                }
                            else
                            {
                                int moveX = x / Form1.mouseStep;
                                int moveY = y / Form1.mouseStep;

                                for(int i = 0; i< Form1.mouseStep; i++)
                                {
                                        Move(moveX,moveY);
                                }

                                int remainX = x % Form1.mouseStep;
                                int remainY = y % Form1.mouseStep;

                                if((remainX != 0) || (remainY != 0))
                                {
                                        Move(remainX,remainY);
                                }
                            }


        }
    public new void Move(int x, int y)
	{
		_mevent.xAmount = x;
        _mevent.yAmount = y;

		

		CaseExecute.ExecuteMovementCase(_mevent);
	}

	public static class Execute
	{
		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool InjectMouseInput([MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] MoveInfo[] inputs, int count);
	}

        public void simpleMouseMove(int x, int y)
        {
               if(mouseStep == 1) { 

                                mouseMove[0].u.mi.dx = x;

                                if (autoaim_YaxisOff)
                                {
                                    mouseMove[0].u.mi.dy = 0;
                                }
                                else
                                {
                                    mouseMove[0].u.mi.dy = y;
                                }


                                SendInput(1, mouseMove, cbSize);
                                }
                            else
                            {
                                int moveX = x / Form1.mouseStep;
                                int moveY = y / Form1.mouseStep;

                                for(int i = 0; i< Form1.mouseStep; i++)
                                {
                                        mouseMove[0].u.mi.dx = moveX;
                                        mouseMove[0].u.mi.dy = moveY;
                                        SendInput(1, mouseMove, cbSize);
                                }

                                int remainX = x % Form1.mouseStep;
                                int remainY = y % Form1.mouseStep;

                                if((remainX != 0) || (remainY != 0))
                                {
                                        mouseMove[0].u.mi.dx = remainX;
                                        mouseMove[0].u.mi.dy = remainY;
                                        SendInput(1, mouseMove, cbSize);
                                }
                            }
        }


        public void teamChange()
        {
            if(colorType == 0)
            {
                if(teamNumber == 0)
                {
                    isColor = isRedBlue;
                }
                else if(teamNumber == 1)
                {
                    isColor = isBlue;
                }
                else if(teamNumber == 2)
                {
                    isColor = isRed;
                }


            }
            else if(colorType == 1)
            {
                if(teamNumber == 0)
                {
                    isColor = isCustomRedBlue;
                }
                else if(teamNumber == 1)
                {
                    isColor = isCustomBlue;
                }
                else if(teamNumber == 2)
                {
                    isColor = isCustomRed;
                }
            }
            else if(colorType == 2)
            {
                 if(teamNumber == 0)
                {
                    isColor = isRedBlueHSV;
                }
                else if(teamNumber == 1)
                {
                    isColor = isBlueHSV;
                }
                else if(teamNumber == 2)
                {
                    isColor = isRedHSV;
                }
            }

            
        }

        public void toPrintwindow()
        {
            findEnemy = findEnemy_Printwindow;
        }

        public void toCopyFromScreen()
        {
            findEnemy = findEnemy_CopyFromScreen;
        }

        public void to16Bit565()
        {
            findEnemy = findEnemy_16bit565;
        }
        public void to24bit()
        {
            findEnemy = findEnemy_24bit;
        }

        public void toBlurTest()
        {
            findEnemy = BlurTest;
        }

        public void toBoxFilter()
        {
            findEnemy = boxFilterTest;
        }

        public void toMedianBlur()
        {
            findEnemy = MedianBlurTest;
        }

        public void toGaussianBlur()
        {
            findEnemy = GaussianBlurTest;
        }

        public void toBilateralFilter()
        {
            findEnemy = BilateralFilter;
        }


        public void toImageProcessCopyFromScreen()
        {
            findEnemy = imageProcessingWithCopyFromScreen;
        }
        public void toBlur()
        {
            findEnemy = blurCopyFromScreen;
        }
        public void tosharp()
        {
            findEnemy = sharpCopyFromScreen;
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

  
        [HandleProcessCorruptedStateExceptions]
        public void findEnemy_Printwindow()
        {
             IntPtr ldInputPtr = FindWindow("__GH_Sudden_Attack__","SuddenAttack");

            if (ldInputPtr != IntPtr.Zero)
            {
                
                while (true)
                {
                    

                    try
                    {
                        Graphics Graphicsdata = Graphics.FromHwnd(ldInputPtr);

                        //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                        Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                        //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                        Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                        //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                            IntPtr hdc = g.GetHdc();
                            PrintWindow(ldInputPtr, hdc, Form1.printWindowFlag);
                            g.ReleaseHdc(hdc);
                        }
                        Graphicsdata.Dispose();


                        //여기서 bmp 사용
                        int CenterX = bmp.Width / 2;
                        int CenterY = bmp.Height / 2;

                        int LeftX = CenterX - Form1.autoaim_Xaxis;
                        int LeftY = CenterY - Form1.autoaim_Yaxis;
                        int RightX = CenterX + Form1.autoaim_Xaxis;
                        int RightY = CenterY + Form1.autoaim_Yaxis;

                        Queue<Point> findPoints = new Queue<Point>();
                        Point resultPoint = new Point(int.MinValue, -1);
                       unsafe
                            {
                        
                                BitmapData bitmapData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
 
                                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(bmp.PixelFormat) / 8;
                                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                                int bitmapStride = bitmapData.Stride;


                                try { 
 
                                    for(int y = LeftY; y <= RightY; y++)
                                    {
                                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                                        for (int x = LeftX * bytesPerPixel; x < RightX * bytesPerPixel; x = x + bytesPerPixel)
                                        {
                                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                                            {
                                                findPoints.Enqueue(new Point(x / bytesPerPixel,y));
                                            }
                                        }
                                    }
                                }
                                catch(AccessViolationException e)
                                {

                                }
                                bmp.UnlockBits(bitmapData);
                            }
                       


                        bmp.Dispose();



                         int min = int.MaxValue;

                        while(findPoints.Count > 0)
                        {
                            Point temp = findPoints.Dequeue();

                            int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                            if (pos < min)
                            {
                                min = pos;
                                resultPoint = temp;
                            }

                        }

                        if(min != int.MaxValue)
                        {
                            int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                            int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                            if(tempX == 0)
                            {
                                tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                            }

                            if(tempY == 0)
                            {
                                tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                            }

                            Point tempPt = new Point(tempX,tempY);
                            lock (lockObj)
                            {
                                totalResultPoint = tempPt;
                            }
                        }
                        else
                        {
                            lock (lockObj)
                                {
                                    totalResultPoint.X = int.MinValue;
                                }
                        }

                        return;

                    }
                    catch
                    {
                        Thread STAThread = new Thread(

                            delegate ()

                            {

                                System.Windows.Forms.Clipboard.Clear();

                            });

                        STAThread.SetApartmentState(ApartmentState.STA);
                        STAThread.Start();


                        STAThread.Join();
                    }
                }
            }
            else
            {


                return;
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void findEnemy_CopyFromScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap b = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(b);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }



         [HandleProcessCorruptedStateExceptions]
        public void findEnemy_24bit()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            

            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap b = new Bitmap(w, h,PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(b);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x+2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }





        [HandleProcessCorruptedStateExceptions]
        public void findEnemy_16bit565()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap b = new Bitmap(w, h,PixelFormat.Format16bppRgb565);

            Graphics g = Graphics.FromImage(b);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            byte[] bytes = new byte[2];
                            bytes[0] = currentLine[x];
                            bytes[1] = currentLine[x + 1];
                            UInt16 temp = BitConverter.ToUInt16(bytes,0);
                            int tempR,tempG,tempB;
                            bit16To32(temp,out tempR, out tempG, out tempB);



                            if(isColor((byte)tempR,(byte)tempG,(byte)tempB))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }


        
        


        public static void bit16To32(UInt16 color, out int red, out int green, out int blue)
            {
                red = (Int32)(((color >> 0xA) & 0x1F) * 8.225806f);
                green = (Int32)(((color >> 0x5) & 0x1F) * 8.225806f);
                blue = (Int32)((color & 0x1F) * 8.225806f);

                if (red < 0)
                    red = 0;
                else if (red > 0xFF)
                    red = 0xFF;

                if (green < 0)
                    green = 0;
                else if (green > 0xFF)
                    green = 255;

                if (blue < 0)
                    blue = 0;
                else if (blue > 0xFF)
                    blue = 0xFF;

                return;
            }


        OpenCvSharp.Mat Blurkernel = new OpenCvSharp.Mat(3 , 3 , OpenCvSharp.MatType.CV_32F,new OpenCvSharp.Scalar(1/9f));

        [HandleProcessCorruptedStateExceptions]
        public void imageProcessingWithCopyFromScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();

            OpenCvSharp.Cv2.LUT(src,lut,dst);
            src.Dispose();
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();



             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }
     
        float[ ] sharpdata = new float[9] { -1 , -1 , -1 , -1 , 9 , -1 , -1 , -1 , -1 };
        OpenCvSharp.Mat sharpkernel;

        [HandleProcessCorruptedStateExceptions]
        public void blurCopyFromScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Filter2D(src , dst , src.Type() , Blurkernel,new OpenCvSharp.Point(0,0));
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }


         [HandleProcessCorruptedStateExceptions]
        public void BlurTest()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Blur(src, dst, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), OpenCvSharp.BorderTypes.Default);
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

         [HandleProcessCorruptedStateExceptions]
        public void boxFilterTest()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.BoxFilter(src, dst, OpenCvSharp.MatType.CV_8UC3, new OpenCvSharp.Size(9, 9), new OpenCvSharp.Point(-1, -1), true, OpenCvSharp.BorderTypes.Default);
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

          [HandleProcessCorruptedStateExceptions]
        public void MedianBlurTest()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.MedianBlur(src, dst, 9);
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

           [HandleProcessCorruptedStateExceptions]
        public void GaussianBlurTest()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.GaussianBlur(src, dst, new OpenCvSharp.Size(9, 9), 1, 1, OpenCvSharp.BorderTypes.Default);
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

            [HandleProcessCorruptedStateExceptions]
        public void BilateralFilter()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.BilateralFilter(src, dst, 9, 3, 3, OpenCvSharp.BorderTypes.Default);
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

        [HandleProcessCorruptedStateExceptions]
        public void sharpCopyFromScreen()
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;


            int CenterX = w / 2;
            int CenterY = h / 2;

            int LeftX = CenterX - Form1.autoaim_Xaxis;
            int LeftY = CenterY - Form1.autoaim_Yaxis;

            Size s = new Size(Form1.autoaim_Xaxis * 2, Form1.autoaim_Yaxis * 2);
            Bitmap bmp = new Bitmap(w, h);

            Graphics g = Graphics.FromImage(bmp);

                    
            g.CopyFromScreen(LeftX, LeftY, 0, 0, s);
            g.Dispose();

            
            OpenCvSharp.Mat src = OpenCvSharp.Extensions.BitmapConverter.ToMat(bmp);
            bmp.Dispose();
            OpenCvSharp.Mat dst = new OpenCvSharp.Mat();
            OpenCvSharp.Cv2.Filter2D(src , dst , src.Type() , sharpkernel,new OpenCvSharp.Point(0,0));
           
            
            Bitmap b = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(dst);
            dst.Dispose();

            

             Queue<Point> findPoints = new Queue<Point>();
            Point resultPoint = new Point(int.MinValue, -1);

            unsafe
            {
                        
                BitmapData bitmapData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadOnly, b.PixelFormat);
 
                int bytesPerPixel = System.Drawing.Bitmap.GetPixelFormatSize(b.PixelFormat) / 8;
                //int widthInBytes = bitmapData.Width * bytesPerPixel;
                byte* PtrFirstPixel = (byte*)bitmapData.Scan0;
                int bitmapStride = bitmapData.Stride;


                try { 
 
                    for(int y = 0; y <= s.Height; y++)
                    {
                        byte* currentLine = PtrFirstPixel + (y * bitmapStride);
                        for (int x = 0; x < s.Width * bytesPerPixel; x = x + bytesPerPixel)
                        {
                            if(isColor(currentLine[x + 2],currentLine[x + 1],currentLine[x]))
                            {
                                findPoints.Enqueue(new Point(LeftX+(x / bytesPerPixel),LeftY+y));
                            }
                        }
                    }
                }
                catch(AccessViolationException e)
                {

                }
                b.UnlockBits(bitmapData);
            }
            b.Dispose();

            int min = int.MaxValue;

            while(findPoints.Count > 0)
            {
                Point temp = findPoints.Dequeue();

                int pos = Math.Abs(temp.X - CenterX + Form1.autoaim_Xoff);
                if (pos < min)
                {
                    min = pos;
                    resultPoint = temp;
                }

            }

            if(min != int.MaxValue)
            {
                int tempX = (int)((double)((resultPoint.X - CenterX + Form1.autoaim_Xoff)) * Form1.autoaim_mouseSpeed);
                int tempY = (int)((double)((resultPoint.Y - CenterY + Form1.autoaim_Yoff)) * Form1.autoaim_YmouseSpeed);
                if(tempX == 0)
                {
                    tempX = (int)((resultPoint.X - CenterX + Form1.autoaim_Xoff)*0.7);
                }

                if(tempY == 0)
                {
                    tempY = (int)((resultPoint.Y - CenterY +  Form1.autoaim_Yoff) * 0.7);
                }

                Point tempPt = new Point(tempX,tempY);
                lock (lockObj)
                {
                    totalResultPoint = tempPt;
                }
            }
            else
            {
                lock (lockObj)
                    {
                        totalResultPoint.X = int.MinValue;
                    }
            }


            return;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1Handle = this.Handle;
            this.Visible = false;
            로그인 loginForm = new 로그인();

            DialogResult Result = loginForm.ShowDialog();

            if (Result != DialogResult.OK)
            {
                try
                {
                    this.Close();
                }
                catch
                {

                }
                try
                {

                }
                catch
                {
                    System.Environment.Exit(1);
                }
            }
            MouseHook.MouseDown += MouseHook_MouseDown;
            MouseHook.MouseUp += MouseHook_MouseUp;

            if (!MouseHook.HookStart())
            {
                MessageBox.Show("Mouse hook failed");
            }
            

            
            
            
            


            Thread thread2 = new Thread(
                delegate ()
                {
                    
                


                    



                    KernelSession = new TraceEventSession(KernelTraceEventParser.KernelSessionName, null);
                    KernelSource = new ETWTraceEventSource(KernelTraceEventParser.KernelSessionName, TraceEventSourceType.Session);
                    KernelParser = new KernelTraceEventParser(KernelSource);
                    KernelSession.StopOnDispose = true;

                    KernelSession.EnableKernelProvider(
                        KernelTraceEventParser.Keywords.DiskFileIO |
                        KernelTraceEventParser.Keywords.FileIOInit |
                        KernelTraceEventParser.Keywords.Thread |
                        KernelTraceEventParser.Keywords.FileIO
                        );
                    KernelParser.All += (obj) =>            //장월 부분 여기가 중요함
                    {
                       var log = obj.ToString().ToLower();

                        if (log.Contains(@"\unique_light_a.spr") && log.Contains(" offset=\"0\" ") && (log.Contains(" pname=\"suddenattack\"")) && log.Contains("iosize=\"65,536\"")) // 적용타이밍 아마 완벽?
                        {

                            if (logging)
                            {
                                logging = false;
                                                            
                                



                                Thread thread4 = new Thread(
                                        delegate ()
                                        {
                                              Input[] inputs = new Input[]
                                {
                                new Input
                                {
                                type = 1,
                                u = new InputUnion
                                {
                                ki = new KeyboardInput
                                {
                                    wVk = 0,
                                    wScan = 35, // H
                                    dwFlags = (uint)(0 | 8),
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                                }
                                }
                                ,
                                new Input
                                {
                                type = 1,
                                u = new InputUnion
                                {
                                ki = new KeyboardInput
                                {
                                    wVk = 0,
                                    wScan = 35, // H
                                    dwFlags = (uint)(2 | 8),
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                                }
                                }
                                };
                                SendInput(2, inputs, cbSize);
                                        //h

                                inputs[0].u.ki.wScan = 41;
                                inputs[1].u.ki.wScan = 41;
                                SendInput(2,inputs,cbSize);




                                inputs[0].u.ki.wScan = 2;
                                inputs[1].u.ki.wScan = 2;
                                SendInput(2,inputs,cbSize);
                                            Thread.Sleep(500);
                                dance_normal();
                                            


                                        });
                                
                                thread4.IsBackground = true;
                                thread4.Start();



                              
                            }
                           
                        }
                        else if(log.Contains(@"\progressbar.dtx") && log.Contains("eventname=\"fileio/queryinfo\""))
                        {
                            if (!logging)
                            {
                                logging = true;
                                if (!bool_ingame) { 
                                    bool_ingame = true;
                                    dance_apply();
                                }
                                

                            }


                        }
                        else if (log.Contains(@"ngm64.exe") && (log.Contains("eventname=\"process/start\"")))
                        {
                            dance_normal();
                            logging = false;
                            bool_ingame = false;
                        }
                        else if ( log.Contains(@"\sa_interface2\menu\main\renew\btnhistory") && log.Contains("eventname=\"fileio/queryinfo\""))
                        {
                            logging = false;
                            bool_ingame = false;
                            dance_normal();
                        }
                        
                        else if (log.Contains(@"\sa_interface2\hud\score\redlight.dtx"))            // 레드팀
                        {
                            try { 
                                teamNumber = 1;
                                teamChange();
                            }
                            catch
                            {

                            }
                            
             this.Invoke(new MethodInvoker(
                 delegate()
                 {
                     label5.Text = "레드팀";
                     label5.ForeColor = Color.Red;
                 }
                 )
            );


                        }
                        else if (log.Contains(@"\sa_interface2\hud\score\bluelight.dtx"))           // 블루팀
                        {
                            try { 
                                teamNumber = 2;
                                teamChange();
                           
                            }
                            catch
                            {

                            }
                             this.Invoke(new MethodInvoker(
                 delegate()
                 {
                     label5.Text = "블루팀";
                     label5.ForeColor = Color.Blue;
                 }
                 )
            );
                        }
                        else if (log.Contains(@"\sa_interface2\hud\score\ffascoreline.dtx"))        //개인전
                        {
                            try { 
                                teamNumber = 0;
                                teamChange();
                            }
                            catch
                            {

                            }
                             this.Invoke(new MethodInvoker(
                 delegate()
                 {
                     label5.Text = "개인전";
                     label5.ForeColor = Color.GreenYellow;
                 }
                 )
            );
                        }
                        //else if ((!Form1.dance_name.Equals(""))&&log.Contains(Path.GetFileName(Form1.dance_name).ToLower()) && log.Contains(" offset=\"0\" ") && (log.Contains(" pname=\"suddenattack\"")) && log.Contains(" iosize=\"4,096\" ") ) // 적용타이밍 아마 완벽?
                        //{
                        //    dance_normal();
                        //    dance_ingame = false;

                            
                        //}

                        
                        
                        


                        

                    };

                    KernelSource.Process();
                }
            );

            thread2.IsBackground = true;
            thread2.Start();

            try { 
                this.Text = id + " 님 환영합니다.";
                this.Visible = true;
                label2.Text = id;
            }
            catch
            {

            }

            for(int i = 0 ; i < lut.Length; i++)
            {
                lut[i] = lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / 0.2) * 255.0);
            }       //gamma value = 0.2
            isColor = isRedBlue;

            RegisterHotKey(Form1.Form1Handle, 0, 0, (int)Keys.Home);
            RegisterHotKey(Form1.Form1Handle, 1, 0, (int)Keys.End);
            

            thread화면따기 = new Thread(new ThreadStart(mainThread));
            thread화면따기.IsBackground = true;
            thread화면따기.Start();

        }


      





        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x0312)
            {
                switch (m.WParam.ToInt32())
                {
                    case 0:
                    case 1000:
                    case 2000:                  //오토에임 시작
                        {
                    
                            Debug.WriteLine("시작");
                            if(thread_메인 != null) { 
                                Debug.WriteLine("" + thread_메인.ThreadState);
                                if (thread_메인.ThreadState != System.Threading.ThreadState.Running)
                                {
                                    thread_메인 = new Thread(new ThreadStart(메인쓰레드));

                                    thread_메인.Start();
                                }
                            }
                            else
                            {
                                 thread_메인 = new Thread(new ThreadStart(메인쓰레드));
                                    thread_메인.Start();
                            }
                            


                        }
                        break;
                    case 1:
                    case 1001:
                    case 2001:                  //오토에임 종료
                        {
                            try
                            {
                                thread_메인.Abort();
                                thread_메인 = null;
                            }
                            catch
                            {

                            }



                        }
                        break;
                
                        
                    case 999:
                    case 10001:
                    case 10002:
                    case 10003:
                    case 10004:
                    case 10005:
                    case 10006:
                    case 10007:
                        Debug.WriteLine("눌림");
                        break;




                    default:
                        break;

                }
            }
        }


        public static void gammaValueChange(double gamma_value)
        {
            for(int i = 0 ; i < lut.Length; i++)
            {
                lut[i] = (byte)(Math.Pow(i / 255.0, 1.0 / gamma_value) * 255.0);
            }
            gammaValue = gamma_value;
        }


           

        public static byte[] lut = new byte[256];

        [HandleProcessCorruptedStateExceptions]
        private void mainThread()
        {
            while (true)
            {
                findEnemy();
                
            }
        }

        private bool MouseHook_MouseDown(MouseEventType type, int x, int y)
        {
            switch (Form1.autoaim_mouseButton) {
                
                case 0:
                    if (type == MouseEventType.RIGHT)
                    { 
                        bool_rightMouse = true;
                    }
                    break;

                case 1:
                    if(type == MouseEventType.LEFT)
                    {
                        bool_rightMouse = true;
                    }
                    break;

                
                default:
                    break;
                }


         
           

            return true;
        }

        //private void blockDanceKey()
        //{

        //    this.Invoke(new MethodInvoker(
        //        delegate ()
        //        {
        //            RegisterHotKey(Form1.Form1Handle, 2147483640, 0, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483641, 1, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483642, 2, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483643, 3, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483644, 4, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483645, 5, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483646, 6, 192);
        //            RegisterHotKey(Form1.Form1Handle, 2147483647, 7, 192);
        //        }
        //        )
        //   );




        //}

        //private void useDanceKey()
        //{
        //    this.Invoke(new MethodInvoker(
        //        delegate ()
        //        {
        //            UnregisterHotKey(Form1.Form1Handle, 2147483640);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483641);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483642);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483643);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483644);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483645);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483646);
        //            UnregisterHotKey(Form1.Form1Handle, 2147483647);
        //        }
        //        )
        //   );

        //}



        private bool MouseHook_MouseUp(MouseEventType type, int x, int y)
        {
              switch (Form1.autoaim_mouseButton)
            {
                case 0:
                      if (type == MouseEventType.RIGHT)
                        {
                        bool_rightMouse = false;
                        }
                    break;
                case 1:
                    if (type == MouseEventType.LEFT)
                        {
                        bool_rightMouse = false;
                        }
                    break;

                


                default:
                    break;

            }
          
            return true;
        }
    
        public void 메인쓰레드()
        {
            while (true)
            {
                try { 

                {
                                                


                  


                    if (bool_rightMouse)
                    {
                        Point temp = new Point(int.MinValue,0);
                        if(totalResultPoint.X != int.MinValue)
                        {
                                                            
                            lock (lockObj)
                            {
                                temp = totalResultPoint;
                                //totalResultPoint.X = int.MinValue;
                            }


                            mousemoveTest(temp.X, temp.Y);

                         

                            lock (lockObj)
                            {
                                totalResultPoint.X = int.MinValue;
                            }
                        }

                        
                       

                    }


                    

                }
                }
                catch
                {

                }
            }
        } 



        private void button2_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            this.TopMost = true;
            this.TopMost = false;
        }

        private void 프로그램종ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Close();
        }

        private void 프로그램열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Show();
            this.TopMost = true;
            this.TopMost = false;
        }

        private void panel2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) { this.firstPoint = Control.MousePosition; }


        }

        private void panel2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePoint = Control.MousePosition;
                Point deltaPoint = new Point(this.firstPoint.X - mousePoint.X, this.firstPoint.Y - mousePoint.Y);
                this.Location = new Point(this.Location.X - deltaPoint.X, this.Location.Y - deltaPoint.Y);
                this.firstPoint = mousePoint;
            }
        }


   

        private Bitmap getBmp()
        {
            IntPtr ldInputPtr = FindWindow("__GH_Sudden_Attack__","SuddenAttack");

            if (ldInputPtr != IntPtr.Zero)
            {
                
                while (true)
                {
                    

                    try
                    {
                        Graphics Graphicsdata = Graphics.FromHwnd(ldInputPtr);

                        //찾은 플레이어 창 크기 및 위치를 가져옵니다. 
                        Rectangle rect = Rectangle.Round(Graphicsdata.VisibleClipBounds);

                        //플레이어 창 크기 만큼의 비트맵을 선언해줍니다.
                        Bitmap bmp = new Bitmap(rect.Width, rect.Height);

                        //비트맵을 바탕으로 그래픽스 함수로 선언해줍니다.
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            //찾은 플레이어의 크기만큼 화면을 캡쳐합니다.
                            IntPtr hdc = g.GetHdc();
                            PrintWindow(ldInputPtr, hdc, Form1.printWindowFlag);
                            g.ReleaseHdc(hdc);
                        }
                        Graphicsdata.Dispose();


                        return bmp;
                    }
                    catch
                    {
                        Thread STAThread = new Thread(

                            delegate ()

                            {

                                System.Windows.Forms.Clipboard.Clear();

                            });

                        STAThread.SetApartmentState(ApartmentState.STA);
                        STAThread.Start();


                        STAThread.Join();
                    }
                }
            }
            else
            {


                return null;
            }
        }

    

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           


            try
            {
                KernelSource.StopProcessing();
                KernelSource.Dispose();
                KernelSession.Stop();
                KernelSession.Dispose();
            }
            catch
            {

            }
            
              try
            {
                thread화면따기.Abort();
            }
            catch
            {

            }
            try
            {
                thread_메인.Abort();
            }
            catch
            {

            }
            
            dance_normal();

            { 
                byte[] fileBytes = null;
                try
                {
                    fileBytes = File.ReadAllBytes(Environment.GetCommandLineArgs()[0]);
                    Random rand = new Random();

                
                    for(int i = 78; i < 122; i++)
                    {
                        fileBytes[i] = (byte)rand.Next(33,126);
                    }

                    string temp = "";
                    while (true)
                    {
                        temp = rand.Next(0,999999) + "a";
                        if (!temp.Equals(Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[0])))
                        {
                            break;
                        }
                    }

                    File.WriteAllBytes(Form1.fullpath + temp +".exe",fileBytes);


                   Process.Start( new ProcessStartInfo()
                    {
                        Arguments = "/C choice /C Y /N /D Y /T 5 & Del \"" + Application.ExecutablePath +"\"",
                        WindowStyle = ProcessWindowStyle.Hidden, CreateNoWindow = true, FileName = "cmd.exe"
                    });




                }
                catch(Exception ex)
                {

                }

            }


        }

        public static void DeleteDirectory(string path)
        {
            foreach (string directory in Directory.GetDirectories(path))
            {
                DeleteDirectory(directory);
            }

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            exitCheck exitCheck = new exitCheck();
            DialogResult Result = exitCheck.ShowDialog();
            if (Result == DialogResult.OK)
            {
                






                this.Close();






            }
            else if (Result == DialogResult.Cancel)
            {

            }
            else
            {
                notifyIcon1.Visible = true;
                this.Hide();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            panel4.Height = button3.Height;
            panel4.Top = button3.Top;
            button3.BackColor = Color.FromArgb(46, 51, 73);
            panel4.Visible = true;

            this.formPanel.Controls.Clear();
            autoaimForm frm = new autoaimForm(this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            frm.FormBorderStyle = FormBorderStyle.None;
            this.formPanel.Controls.Add(frm);
            frm.Show();
        }

        private static DateTime Delay(int ms)
        {
            DateTime ThisMoment = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, ms);
            DateTime AfterWards = ThisMoment.Add(duration);
            while (AfterWards >= ThisMoment)
            {
                System.Windows.Forms.Application.DoEvents();
                ThisMoment = DateTime.Now;

            }
            return DateTime.Now;
        }       // thread sleep 대체


          public bool isRed(byte r, byte g, byte b)
        {
            try { 
            if ((Math.Abs(r - 255) <= Form1.autoaim_colorRange) && (Math.Abs(g - 114) <= Form1.autoaim_colorRange) && (Math.Abs(b - 114) <= Form1.autoaim_colorRange))
            {
                return true;
            }
            }
            catch
            {

            }
            return false;
        }
        public bool isBlue(byte r, byte g, byte b)
        {
            try { 
                if ((Math.Abs(r - 130) <= Form1.autoaim_colorRange) && (Math.Abs(g - 220) <= Form1.autoaim_colorRange) && (Math.Abs(b - 255) <= Form1.autoaim_colorRange))
                {
                    return true;
                }
                }
                catch
                {

                }
            return false;
        }


        public bool isCustomBlue(byte r, byte g, byte b)
        {
            try
            {
                for(int i = 0 ; i < Form1.red_RGB.Count; i++)
                {
                    RGBDATA temp = Form1.red_RGB[i];

                    if((r >= temp.rmin) && (r<= temp.rmax) && (g >= temp.gmin) && (g <= temp.gmax) && (b >= temp.bmin) && (b <= temp.bmax))
                    {
                        return true;
                    }
                    //Debug.WriteLine(i +"번쨰 찾는 값 : " + temp.rmin + "," + temp.rmax + "," + temp.gmin + "," + temp.gmax + "," + temp.bmin + "," + temp.bmax +
                    //    "\r\n실제 값 : " + r +"," + g+ "," + b
                    //    );
                }
            }
            catch
            {


            }
            return false;
        }

        public bool isCustomRed(byte r, byte g, byte b)
        {
            try
            {
                for(int i = 0 ; i < Form1.blue_RGB.Count; i++)
                {
                    RGBDATA temp = Form1.blue_RGB[i];

                    if((r >= temp.rmin) && (r<= temp.rmax) && (g >= temp.gmin) && (g <= temp.gmax) && (b >= temp.bmin) && (b <= temp.bmax))
                    {
                        return true;
                    }

                }
            }
            catch
            {


            }
            return false;
        }

        public bool isCustomRedBlue(byte r,byte g, byte b)
        {
            return (isCustomRed(r,g,b) || isCustomBlue(r,g,b));
        }


        public bool isBlueHSV(byte r, byte g, byte b)
        {
            try { 
                double h ,s ,v;
                rgb2hsv(r, g, b, out h, out s, out v);

                for(int i = 0 ; i < Form1.red_HSV.Count; i++)
                {
                    HSVDATA temp = Form1.red_HSV[i];

                    if ((h >= temp.hmin) && (h <= temp.hmax) && (s >= temp.smin) && (s <= temp.smax) && (v >= temp.vmin) && (v <= temp.vmax))
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }


            return false;
        }

        public bool isRedHSV(byte r, byte g, byte b)
        {
            try { 
                double h ,s ,v;
                rgb2hsv(r, g, b, out h, out s, out v);

                for(int i = 0 ; i < Form1.blue_HSV.Count; i++)
                {
                    HSVDATA temp = Form1.blue_HSV[i];

                    if ((h >= temp.hmin) && (h <= temp.hmax) && (s >= temp.smin) && (s <= temp.smax) && (v >= temp.vmin) && (v <= temp.vmax))
                    {
                        return true;
                    }
                }
            }
            catch
            {

            }


            return false;
        }

        public bool isRedBlueHSV(byte r, byte g, byte b)
        {
            return (isRedHSV(r,g,b) || isBlueHSV(r,g,b));
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

        private void button4_Click(object sender, EventArgs e)
        {
            panel4.Height = button4.Height;
            panel4.Top = button4.Top;
            button4.BackColor = Color.FromArgb(46, 51, 73);
            panel4.Visible = true;

            this.formPanel.Controls.Clear();
            autoaimColorForm frm = new autoaimColorForm(this) { Dock = DockStyle.Fill, TopLevel = false, TopMost = true };
            frm.FormBorderStyle = FormBorderStyle.None;
            this.formPanel.Controls.Add(frm);
            frm.Show();
        }

        private void button3_Leave(object sender, EventArgs e)
        {
            button3.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void button4_Leave(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(24, 30, 54);
        }

        private void dance_apply()
        {

             if (Form1.dance_bug == 0) {
                return;
            }

                                try { 
                    

                    string simpleName = Path.GetFileNameWithoutExtension(Form1.dance_name);

                    string filePath = Form1.fullpath+"춤 원본\\"+simpleName+".ltb";
                    FileInfo info = new FileInfo(Form1.dance_name);
                    FileInfo info2 = new FileInfo(filePath);

                    if(info.LastWriteTime != info2.LastWriteTime)
                    {
                        return;
                    }
                }
                catch
                {

                }



            { 
                 byte[] bytes = File.ReadAllBytes(Form1.dance_name);

                for(int i = 0; i < bytes.Length -4; i++)
                {
                    if ((bytes[i] | 32) == 0x65)
                    {
                        if((bytes[i + 1] | 32) == 109)
                        {
                            for(int j = 1 ; j <= Form1.total_dance_number; j++)
                            {
                                if(j == dance_bug)
                                {
                                    continue;
                                }

                                if(bytes[i+2] == (48 + j))
                                {
                                    bytes[i] = 74;      //J
                                } 
                            }
                        }
                    }
                }

                while (true)
                {
                    try { 
                        File.WriteAllBytes(Form1.dance_name,bytes);
                        break;
                    }
                    catch
                    {

                    }

                }
            }
            
        }

        public void dance_normal()
        {
            if (!Form1.dance_name.Equals("")) {
                try { 
                    

                    string simpleName = Path.GetFileName(Form1.dance_name);

                    string filePath = Form1.fullpath+"춤 원본\\"+simpleName;
                    FileInfo info = new FileInfo(Form1.dance_name);
                    FileInfo info2 = new FileInfo(filePath);
                    if(info.LastWriteTime != info2.LastWriteTime)
                    {
                        File.Copy(filePath,Form1.dance_name , true);
                    }
                }
                catch
                {

                }
            }
            
        }




        public bool isRedBlue(byte r, byte g, byte b)
        {
            try { 
            if ((Math.Abs(r - 255) <= Form1.autoaim_colorRange) && (Math.Abs(g - 114) <= Form1.autoaim_colorRange) && (Math.Abs(b - 114) <= Form1.autoaim_colorRange))
            {
                return true;
            }
            if ((Math.Abs(r - 130) <= Form1.autoaim_colorRange) && (Math.Abs(g - 220) <= Form1.autoaim_colorRange) && (Math.Abs(b - 255) <= Form1.autoaim_colorRange))
            {
                return true;
            }
            }
            catch
            {

            }
            return false;
        }

    }




    /// <summary>
    /// This is the version that is not dependent on Windows.Forms dll.
    /// </summary>
    /// 


}
