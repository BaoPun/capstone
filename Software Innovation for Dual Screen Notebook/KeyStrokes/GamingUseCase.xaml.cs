﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace KeyStrokes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class GamingUseCase: Window
    {
        public static Boolean finished;
        private const int WM_MOUSEACTIVATE = 0x0021;
        private const int MA_NOACTIVATE = 3;
        private const int WS_EX_NOACTIVE = 0x08000000;
        private const int GWL_EXSTYLE = -20;

        // this is used to get the hWnd for imported functions
        // hWnd is a way to identify windows used in win32 framework
        private WindowInteropHelper helper;
        private List<VirtualKeyShort.Key> shortcut;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        //Store list of hotkeys (in char) used atm (Cannot use the same hotkey in more than one button)
        private static List<char> hotkeyCharList = new List<char>();

        //Also store list of hotkeys (in Key) used atm
        private static List<Key> hotKeyList = new List<Key>();

        //Also store list of locations (mainly used for the dynamic button portion)
        private static List<string> locations = new List<string>();

        //Also store list of images
        private static List<string> imageList = new List<string>();

        /*
        [DllImport("user32.dll")]
        static extern short VkKeyScan(char ch);

        static public Key ResolveKey(char charToResolve)
        {
            return KeyInterop.KeyFromVirtualKey(VkKeyScan(charToResolve));
        }*/

        // Retrieve an icon
        public static ImageSource GetIcon(string fileName)
        {
            Icon icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
            return Imaging.CreateBitmapSourceFromHIcon(
                        icon.Handle,
                        new Int32Rect(0, 0, icon.Width, icon.Height),
                        BitmapSizeOptions.FromEmptyOptions());
        }


        public GamingUseCase()
        {
            InitializeComponent();

            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine("The current window element: " + window.Content);
            }

            //Your boy did it, he managed to KEKW the capstone project
            this.Icon = BitmapFrame.Create(new Uri("../../Images/kekw.jpg", UriKind.RelativeOrAbsolute));

            //When loading multiple single instances, the lists are not being cleared for w/e reason.  Fix this issue by clearing the arrays beforehand.
            hotkeyCharList.Clear();
            hotKeyList.Clear();
            locations.Clear();
            imageList.Clear();

            //Store the initial set of hotkeys (1 initially)
            hotkeyCharList.Add('+');

            hotKeyList.Add(Key.OemPlus);


            //Also add the applications
            LoadApplicationsFromFile("../../SavedApplications.txt");

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            shortcut = new List<VirtualKeyShort.Key>();

            // sets the window so that a click does not bring it into focus
            helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVE);

            // this lets WndProc be overriden so that we can get the click massage
            HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
            source.AddHook(WndProc);
        }

        // this gets the click message so that 
        // it can still sends the click to the app
        // even though it is out of focus
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle messages...
            switch (msg)
            {
                case WM_MOUSEACTIVATE:
                    return (IntPtr)MA_NOACTIVATE;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        //Add the locations via a file
        public static void LoadApplicationsFromFile(string file)
        {
            using (StreamReader reader = new StreamReader(file))
            {
                //Read until the file is empty 

                //Step 1: read the application itself
                string application;
                while ((application = reader.ReadLine()) != null)
                {
                    //4 lines per application:
                    /*
                     *  1. Application name
                     *  2. Button height, width, and margins 
                     *  3. Image location (unless n), height, width, and transformation origins
                     *  4. Text margins
                     */

                    //If we encounter an empty string, stop
                    if (application == "")
                        break;

                    // For steps 2-4, we need a tokenizer
                    char[] separator = { ' ' };

                    // Step 2 (requires tokenizing)
                    string[] buttonInfo = reader.ReadLine().Split(separator);
                    int buttonHeight = Int32.Parse(buttonInfo[0]);
                    int buttonWidth = Int32.Parse(buttonInfo[1]);
                    int buttonMarginOne = Int32.Parse(buttonInfo[2]);
                    int buttonMarginTwo = Int32.Parse(buttonInfo[3]);

                    // Step 3 (requires tokenizing)
                    string[] imageInfo = reader.ReadLine().Split(separator);
                    string imageLocation = imageInfo[0];
                    if (imageLocation == "n")
                        imageLocation = "";
                    int imageHeight = Int32.Parse(imageInfo[1]);
                    int imageWidth = Int32.Parse(imageInfo[2]);
                    double originOne = Double.Parse(imageInfo[3]);
                    double originTwo = Double.Parse(imageInfo[4]);

                    // Step 4 (requires parsing)
                    string[] textInfo = reader.ReadLine().Split(separator);
                    int textMarginOne = Int32.Parse(textInfo[0]);
                    int textMarginTwo = Int32.Parse(textInfo[1]);
                    int textMarginThree = Int32.Parse(textInfo[2]);
                    int textMarginFour = Int32.Parse(textInfo[3]);
                    char hotkey = textInfo[4][0];

                    // Then load that button dynamically
                    AddApplication(application,
                                    buttonHeight, buttonWidth, buttonMarginOne, buttonMarginTwo,
                                        imageLocation, imageHeight, imageWidth, originOne, originTwo,
                                            textMarginOne, textMarginTwo, textMarginThree, textMarginFour, hotkey);
                }
            }
        }

        // Actually add the application
        private static void AddApplication(string appLocation,
                                        int buttonHeight, int buttonWidth, int buttonMarginOne, int buttonMarginTwo,
                                            string imageLocation, int imageHeight, int imageWidth, double originOne, double originTwo,
                                                int textMarginOne, int textMarginTwo, int textMarginThree, int textMarginFour, char hotkey)
        {
            //Adds the margin (Left, Top, Right, Bottom)
            Thickness buttonMargin = new Thickness(buttonMarginOne, 0, buttonMarginTwo, 0);
            Thickness textMargin = new Thickness(textMarginOne, textMarginTwo, textMarginThree, textMarginFour);

            //Adds an image to the button (WIP)
            //If no specified image was provided, then use the exe's icon instead.
            //Otherwise, use that specific image link
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            BitmapSource source;
            ImageSource sourceEmpty;
            if (imageLocation != "")
            {
                source = new BitmapImage(new Uri(imageLocation, UriKind.RelativeOrAbsolute));
                image.Source = source;
            }
            else
            {
                sourceEmpty = GetIcon(appLocation);
                image.Source = sourceEmpty;
            }
            //BitmapSource source = new BitmapImage(new Uri(appImage, UriKind.RelativeOrAbsolute));
            //image.Source = source;
            image.Height = imageHeight;
            image.Width = imageWidth;
            System.Windows.Point point = new System.Windows.Point(originOne, originTwo);
            image.RenderTransformOrigin = point;

            //Add a text block
            TextBlock text = new TextBlock();
            text.VerticalAlignment = VerticalAlignment.Bottom;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.Margin = textMargin;
            text.Text = hotkey + "";

            //Create a dock panel that holds both the Image and the TextBlock children
            DockPanel dock = new DockPanel();
            dock.Children.Add(image);
            dock.Children.Add(text);

            //Create a new button and give it keydown and click events
            Button newButton = new Button()
            {
                Height = buttonHeight,
                Width = buttonWidth,
                Margin = buttonMargin,
            };
            newButton.KeyDown += DynamicButton_KeyDown;
            newButton.Click += (sender, e) => DynamicButton_Click(sender, e, appLocation);

            //Contents of the button is simply whatever the dock is
            newButton.Content = dock;


            //Finally, the button is a part of the stack panel, which is a content of the scrollviewer
            //The only two controls are the ScrollViewer and the Grid.  Since we want the Grid, use index 1.

            Window content;
            foreach (Window window in Application.Current.Windows)
            {
                Console.WriteLine("The current window element: " + window.Content);
                content = window;
                if (content.Content is System.Windows.Controls.Grid)
                {
                    ScrollViewer scroll = content.FindName("ButtonViewholder") as ScrollViewer;
                    StackPanel stack = content.FindName("MyStack") as StackPanel;

                    stack.Children.Add(newButton);
                    scroll.Content = stack;
                    break;
                }
            }
            


            //Determine which key was pressed and add it to the Key enum list

            //Is it a number?
            if (Char.IsDigit(hotkey))
            {
                if (hotkey == '1')
                    hotKeyList.Add(Key.D1);
                else if (hotkey == '2')
                    hotKeyList.Add(Key.D2);
                else if (hotkey == '3')
                    hotKeyList.Add(Key.D3);
                else if (hotkey == '4')
                    hotKeyList.Add(Key.D4);
                else if (hotkey == '5')
                    hotKeyList.Add(Key.D5);
                else if (hotkey == '6')
                    hotKeyList.Add(Key.D6);
                else if (hotkey == '7')
                    hotKeyList.Add(Key.D7);
                else if (hotkey == '8')
                    hotKeyList.Add(Key.D8);
                else if (hotkey == '9')
                    hotKeyList.Add(Key.D9);
                else
                    hotKeyList.Add(Key.D0);
            }

            //It must be a letter if it's not a number
            else
            {
                if (Char.ToUpper(hotkey) == 'A')
                    hotKeyList.Add(Key.A);
                else if (Char.ToUpper(hotkey) == 'B')
                    hotKeyList.Add(Key.B);
                else if (Char.ToUpper(hotkey) == 'C')
                    hotKeyList.Add(Key.C);
                else if (Char.ToUpper(hotkey) == 'D')
                    hotKeyList.Add(Key.D);
                else if (Char.ToUpper(hotkey) == 'E')
                    hotKeyList.Add(Key.E);
                else if (Char.ToUpper(hotkey) == 'F')
                    hotKeyList.Add(Key.F);
                else if (Char.ToUpper(hotkey) == 'G')
                    hotKeyList.Add(Key.G);
                else if (Char.ToUpper(hotkey) == 'H')
                    hotKeyList.Add(Key.H);
                else if (Char.ToUpper(hotkey) == 'I')
                    hotKeyList.Add(Key.I);
                else if (Char.ToUpper(hotkey) == 'J')
                    hotKeyList.Add(Key.J);
                else if (Char.ToUpper(hotkey) == 'K')
                    hotKeyList.Add(Key.K);
                else if (Char.ToUpper(hotkey) == 'L')
                    hotKeyList.Add(Key.L);
                else if (Char.ToUpper(hotkey) == 'M')
                    hotKeyList.Add(Key.M);
                else if (Char.ToUpper(hotkey) == 'N')
                    hotKeyList.Add(Key.N);
                else if (Char.ToUpper(hotkey) == 'O')
                    hotKeyList.Add(Key.O);
                else if (Char.ToUpper(hotkey) == 'P')
                    hotKeyList.Add(Key.P);
                else if (Char.ToUpper(hotkey) == 'Q')
                    hotKeyList.Add(Key.Q);
                else if (Char.ToUpper(hotkey) == 'R')
                    hotKeyList.Add(Key.R);
                else if (Char.ToUpper(hotkey) == 'S')
                    hotKeyList.Add(Key.S);
                else if (Char.ToUpper(hotkey) == 'T')
                    hotKeyList.Add(Key.T);
                else if (Char.ToUpper(hotkey) == 'U')
                    hotKeyList.Add(Key.U);
                else if (Char.ToUpper(hotkey) == 'V')
                    hotKeyList.Add(Key.V);
                else if (Char.ToUpper(hotkey) == 'W')
                    hotKeyList.Add(Key.W);
                else if (Char.ToUpper(hotkey) == 'X')
                    hotKeyList.Add(Key.X);
                else if (Char.ToUpper(hotkey) == 'Y')
                    hotKeyList.Add(Key.Y);
                else
                    hotKeyList.Add(Key.Z);

                hotkey = Char.ToUpper(hotkey);
            }

            //Before finishing up, add the hotkey to the list
            //and the location and the image
            hotkeyCharList.Add(hotkey);
            locations.Add(appLocation);
            imageList.Add(imageLocation);
        }


        //Keys register on window.
        private void KeyInteractor(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.OemPlus)
            {
                e.Handled = true; //prevent the action from happening twice.
                Add_KeyDown(sender, e);
            }
            //For the dynamic
            else
            {
                e.Handled = true;
                DynamicButton_KeyDown(sender, e);
            }
        }


        // Save all current applications on the Window to the text file
        private void CloseWindow(object sender, CancelEventArgs e)
        {
            // If user chooses to quit, then save all applications onto the text file before leaving
            MessageBoxResult confirm = MessageBox.Show("Are you sure you want to quit?", "Confirm Closure", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (confirm == MessageBoxResult.No)
                e.Cancel = true;
            else
            {
                MainWindow.currentInstance = false;
                SaveApplications("../../SavedApplications.txt");
                e.Cancel = false;
            }
        }


        private void SaveApplications(string file)
        {
            using (StreamWriter writer = new StreamWriter(file))
            {
                // Total # of apps is based on the # of locations
                for (int i = 0; i < locations.Count; i++)
                {
                    // First line: app location
                    writer.WriteLine(locations[i]);

                    // Second line: button details
                    writer.WriteLine("75 90 11 10");

                    // Third line: image details
                    if (imageList[i] == "")
                        imageList[i] = "n";
                    writer.WriteLine(imageList[i] + " 72 75 0.455 -0.263");

                    // Fourth line: text details (offset by 1 since first hotkey is +)
                    writer.WriteLine("-80 0 -5 -20 " + hotkeyCharList[i + 1]);
                }
            }
        }


        // this opens the help window purely for showing how to work this application
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            /*
            AddButtonWindow addButton = new AddButtonWindow();
            addButton.InitializeComponent();
            addButton.Show();
            */
            if (finished)
            {
                MessageBox.Show("What are you doing?  You have an instance of this window open already!", "Already opened");
                return;
            }
            finished = true;
            AddApplication form1 = new AddApplication();
            form1.Show();
        }

        // Press the + button (shift =) to call via keydown
        private void Add_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.OemPlus)
                Add_Click(sender, e);
        }

        //Takes in the form inputs from Form1.cs and dynamically adds a new button by appending it to the ScrollViewer
        public static bool processFormInputs(string appLocation, string appImage, string appHotKey)
        {

            //First go through the current list of hotkeys. 
            //If at least one hotkey matches, then return false
            for (int i = 0; i < hotkeyCharList.Count; i++)
            {
                if (Char.ToUpper(appHotKey[0]) == hotkeyCharList[i])
                {
                    MessageBox.Show("Please use a different hotkey, it needs to be unique", "NonUnique Hotkey");
                    return false;
                }
            }

            //Otherwise form is valid and hotkey is unique, add the form.



            //Just displaying what will be added
            MessageBox.Show("Your application was successfully added!", "Button successfully created!");

            //Dynamically add button details
            AddApplication(appLocation,
                                75, 90, 11, 10,
                                    appImage, 72, 75, 0.455, -0.263,
                                        -80, 0, -5, -20, appHotKey[0]);



            return true;


        }

        //Dynamic button's KeyDown event
        private static void DynamicButton_KeyDown(object sender, KeyEventArgs e)
        {
            //Search for which hotkey was pressed (not including + since there's already an event for it)
            for (int i = 1; i < hotKeyList.Count; i++)
            {
                if (hotKeyList[i] == e.Key)
                {
                    //Start the application at the 1st offset index and then stop searching
                    Process.Start(locations[i - 1]);
                    break;
                }
            }
        }

        //Dynamic button's Click event
        private static void DynamicButton_Click(object sender, RoutedEventArgs e, string location)
        {
            Process.Start(location);
        }

        //Only have 1 instance of this
        private void Check_Load(object sender, RoutedEventArgs e)
        {

        }
    }
}
