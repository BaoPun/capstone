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

namespace KeyStrokes
{
    public partial class ButtonGridControl : UserControl
    {
        private List<Button> buttonList;
        private int col = 0;
        private int row = 0;
        private Button hold;
        private Point startPoint;

        public ButtonGridControl()
        {
            InitializeComponent();

            buttonList = new List<Button>();

            //addButton("+", Add_Click);
        }

        public void addButton(String content, Action<object, RoutedEventArgs> click)
        {
            var template = (ControlTemplate)buttonGrid.FindResource("button");
            Button b = new Button { Template = template };

            b.Content = content;
            b.Width = Double.NaN;
            b.MaxHeight = 50;
            b.MaxWidth = 100;

            // set the click handler
            b.Click += click.Invoke;

            // option to remove the button
            //newButton.RightTapped += async (s, en) =>
            b.MouseDown += async (s, en) =>
            {

                /* MessageBoxResult result = MessageBox.Show("Remove?", "", MessageBoxButton.YesRemoveNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:

                        // remove button
                        grid.Children.Remove(b);
                        buttonList.Clear();
                        foreach(Button buttonItem in grid.Children)
                        {
                            if (buttonItem is FrameworkElement)
                            {
                                buttonList.Add(buttonItem);
                            }
                        }
                        
                        break;
                }
                */
                btnMenu.Visibility = Visibility.Visible;
                hold = b;

            };

            
            b.MouseDoubleClick += async (s, en) =>
            {
                btnMenu.Visibility = Visibility.Visible;
                hold = b;
            };


            // change this to actaully set the col and row
            //Grid.SetColumn(b, col++);
            //Grid.SetRow(b, row);

            // add button to the list
            buttonList.Add(b);

            //grid.Children.Add(b);

            // re-adds the buttons from the list 
            grid.Children.Clear();
            foreach (Button buttonItem in buttonList)
            {
                grid.Children.Add(buttonItem);
            }
        }

        public void removeBtn(object sender, RoutedEventArgs e)
        {
            // remove button
            grid.Children.Remove(hold);
            buttonList.Clear();
            foreach (Button buttonItem in grid.Children)
            {
                if (buttonItem is FrameworkElement)
                {
                    buttonList.Add(buttonItem);
                }
            }

            btnMenu.Visibility = Visibility.Hidden;
        }

        /*
        public void okBtn(object sender, RoutedEventArgs e)
        {
            rName.Visibility = Visibility.Visible;
            rowIn.Visibility = Visibility.Visible;
            cName.Visibility = Visibility.Visible;
            colIn.Visibility = Visibility.Visible;
            but.Visibility = Visibility.Visible;
        }*/

        public void changeBtn(object sender, RoutedEventArgs e)
        {
            grid.Visibility = Visibility.Hidden;
            myG.Visibility = Visibility.Visible;


            myG.Rows = grid.Rows+1;
            myG.Columns = 2*grid.Columns;

            buttonList.Clear();
            foreach (Button buttonItem in grid.Children)
            {
                if (buttonItem is FrameworkElement)
                {
                    buttonList.Add(buttonItem);
                }
            }

            grid.Children.Clear();

            Button placeButton = new Button();
            placeButton.Content = "+";
            placeButton.Height = 20;
            placeButton.Width = 40;
            placeButton.Click += new RoutedEventHandler(btnClick);

            myG.Children.Add(placeButton);
            foreach (Button buttonItem in buttonList)
            {
                Button placeButtons = new Button();
                placeButtons.Content = "+";
                placeButtons.Height = 20;
                placeButtons.Width = 40;
                placeButtons.Click += new RoutedEventHandler(btnClick);


                myG.Children.Add(buttonItem);
                myG.Children.Add(placeButtons);
            }


        }
 
        public void cancelBtn(object sender, RoutedEventArgs e)
        {
            if (myG.Visibility == Visibility.Visible)
            {
                myG.Children.Clear();

                foreach(Button bI in buttonList)
                {
                    grid.Children.Add(bI);
                }

                myG.Visibility = Visibility.Hidden;
                grid.Visibility = Visibility.Visible;
            }

            btnMenu.Visibility = Visibility.Hidden;
        }

        public void btnClick(object sender, RoutedEventArgs e)
        {
            // change color of button
            Button targetButton = (Button)sender;
            targetButton.Content = "clicked";

            // go through grid moving elements to list 
            buttonList.Clear();
            foreach(Button buttonItem in myG.Children)
            {
                // if button.Content == "clicked", make that the hold
                if (buttonItem == targetButton)
                {
                    buttonList.Add(hold);
                }
                // if button == "hold", remove
                else if (buttonItem == hold)
                {
                    ;
                }
                // if button.Content == "+", skip
                else if (buttonItem.Content.ToString() == "+")
                {
                    ;
                }
                else
                {
                    buttonList.Add(buttonItem);
                }
            }
            // clear out spare grid
            myG.Children.Clear();

            // move list to real grid
            foreach (Button buttonItem in buttonList)
            {
                grid.Children.Add(buttonItem);
            }

            // reverse the grids back 
            grid.Visibility = Visibility.Visible;
            myG.Visibility = Visibility.Hidden;
            btnMenu.Visibility = Visibility.Hidden;

        }


        // this opens the new window for adding new buttons
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //AddButtonWindow addButton = new AddButtonWindow();
            //addButton.InitializeComponent();
            //addButton.Show();
        }


        public void set_grid(int row, int col)
        {
            grid.Rows = row;
            grid.Columns = col;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}