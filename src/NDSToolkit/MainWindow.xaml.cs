﻿using System;
using System.IO;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Win32;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace NDS_Toolkit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        long Length1 = 0, Length2 = 0;
        string nLine = Environment.NewLine;

        //File Dialogs
        OpenFileDialog openFileOne = new OpenFileDialog();
        OpenFileDialog openFileTwo = new OpenFileDialog();

        //Code types
        string D2 = "D2000000 00000000";
        string DC = "DC000000 ", C0 = "C0000000 ";
        string D4 = "D4000000 ", D5 = "D5000000 ";
        string D6 = "D6000000 ", D7 = "D7000000 ";
        string D8 = "D8000000 "; 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CodeAdd.IsChecked = true; 
            Positive.IsChecked = true;
            int DefaultOffset = 0x8000;
            MaxOffset.Text = DefaultOffset.ToString("X8");
        }

        private void ExitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); 
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            AboutBox1 box = new AboutBox1();
            box.ShowDialog();
        }

        private void CheatMenu_Click(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
        }

        #region GlobalFunctions
        private bool Valid_Code(String Code, String Pattern)
        {
            if (Regex.IsMatch(Code, Pattern, RegexOptions.IgnoreCase))
                return true;
            else return false;
        }

        private void HexOnly_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.F ||
                e.Key >= Key.D0 && e.Key <= Key.D9 ||
                e.Key >= Key.NumPad0 || e.Key <= Key.NumPad9)
                e.Handled = false;
            else e.Handled = true;
        }

        private void DecOnly_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.D0 && e.Key <= Key.D9 ||
                e.Key >= Key.NumPad0 || e.Key <= Key.NumPad9)
                e.Handled = false;
            else e.Handled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoopBase.Text.Length == 8) //Loop Code Generator 
            {
                LoopBase.Text += " ";
                LoopBase.SelectionStart = 10;
            }

            if (FileOneRead.Text != "" && FileTwoRead.Text != "") //Helder's requested Pointer feature
            {
                string ReadToAddy1 = FileOneRead.Text.Substring(FileOneRead.Text.IndexOf(".bin") - 8, 8);
                string ReadToAddy2 = FileTwoRead.Text.Substring(FileTwoRead.Text.IndexOf(".bin") - 8, 8);

                if (Valid_Code(ReadToAddy1, "[0-F]{8}") && Valid_Code(ReadToAddy2, "[0-F]{8}"))
                {
                    AddressOne.Text = ReadToAddy1;
                    AddressTwo.Text = ReadToAddy2;
                }
                else
                {
                    AddressOne.Text = "";
                    AddressTwo.Text = "";
                }
            }

            //CodePorter; elixirdream's requested feature 
            if (CodeOutput.Text != "")
                CodeOutput.IsReadOnly = false;
            else CodeOutput.IsReadOnly = true; 
        }
        #endregion

        #region ButtonGenerator
        private void ButtonGen_Click(object sender, RoutedEventArgs e)
        {
            int GBATotal = 0, NDSTotal = 0;
            bool GBA = false, NDS = false;

            if (chkA.IsChecked == true)
                GBATotal += 0x0001;
            if (chkB.IsChecked == true)
                GBATotal += 0x0002;
            if (chkS.IsChecked == true)
                GBATotal += 0x0004;
            if (chkT.IsChecked == true)
                GBATotal += 0x0008;
            if (chkI.IsChecked == true)
                GBATotal += 0x0010;
            if (chkE.IsChecked == true)
                GBATotal += 0x0020;
            if (chkU.IsChecked == true)
                GBATotal += 0x0040;
            if (chkD.IsChecked == true)
                GBATotal += 0x0080;
            if (chkR.IsChecked == true)
                GBATotal += 0x0100;
            if (chkL.IsChecked == true)
                GBATotal += 0x0200;
            if (chkX.IsChecked == true)
                NDSTotal += 0x0400;
            if (chkY.IsChecked == true)
                NDSTotal += 0x0800;
            if (chkG.IsChecked == true)
                NDSTotal += 0x2000;
            if (chkF.IsChecked == true)
                NDSTotal += 0x8000;

            string GBAConverted = (0xFFFF - GBATotal).ToString("X") + "0000";
            string NDSConverted = (0xFFFF - NDSTotal).ToString("X") + "0000";

            if (GBATotal != 0)
                GBA = true;
            if (NDSTotal != 0)
                NDS = true;

            if (GBA == true)
            {
                ButtonOutput.Text = "94000130 " + GBAConverted + nLine + ButtonInput.Text + nLine + D2;
                GBATst.Text = "GBA tst Value: 0x" + GBATotal.ToString("X4");
            }
            else GBATst.Text = "";

            if (NDS == true)
            {
                ButtonOutput.Text = "927FFFA8 " + NDSConverted + nLine + ButtonInput.Text + nLine + D2;
                NDSTst.Text = "NDS tst Value: 0x" + NDSTotal.ToString("X4");
            }
            else NDSTst.Text = "";

            if (GBA == true && NDS == true)
            {
                ButtonOutput.Text = "927FFFA8 " + NDSConverted + nLine + "94000130 " + 
                    GBAConverted + nLine + ButtonInput.Text + nLine + D2;
            }
            else if (GBA == false && NDS == false)
            {
                ButtonOutput.Text = "";
                GBATst.Text = "";
                NDSTst.Text = "";
            }
        }

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement elementOne in grid1.Children)
            {
                if (elementOne is TextBox)
                {
                    TextBox buttonCodeIO = (TextBox)elementOne;
                    buttonCodeIO.Text = "";
                }
                if (elementOne is GroupBox)
                {
                    foreach (UIElement elementTwo in chkGrid.Children)
                    {
                        if (elementTwo is CheckBox)
                        {
                            CheckBox buttonChk = (CheckBox)elementTwo;
                            buttonChk.IsChecked = false;
                        }
                    }
                }
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ButtonOutput.Text);
        }

        private void ButtonPaste_Click(object sender, RoutedEventArgs e)
        {
            ButtonInput.Paste();
        }
        #endregion

        #region PointerSearcher
        private void FileOne_Click(object sender, RoutedEventArgs e)
        {
            openFileOne.Filter = "Binary file (*.bin)|*.bin";

            if ((openFileOne.ShowDialog() == true) && (openFileOne.OpenFile() != null))
            {
                FileOneRead.Text = System.IO.Path.GetFileName(openFileOne.FileName);
                Length1 = openFileOne.OpenFile().Length;
            }
        }

        private void FileTwo_Click(object sender, RoutedEventArgs e)
        {
            openFileTwo.Filter = "Binary file (*.bin)|*.bin";

            if ((openFileTwo.ShowDialog() == true) && (openFileTwo.OpenFile() != null))
            {
                FileTwoRead.Text = System.IO.Path.GetFileName(openFileTwo.FileName);
                Length2 = openFileTwo.OpenFile().Length;
            }
        }

        private void PointerSearch_Click(object sender, RoutedEventArgs e)
        {
            //Check if both files have been opened
            if (Length1 == 0 || Length2 == 0 || Length1 != Length2)
            {
                MessageBox.Show(this, "Please upload two files of the same size.");
                return;
            }

            //compare the length of the files (in bytes/size)
            if (openFileOne.OpenFile().Length != openFileTwo.OpenFile().Length)
            {
                MessageBox.Show(this, "The files you uploaded are different sizes!");
                return;
            }

            //Check if the addresses are the same
            if (AddressOne.Text == AddressTwo.Text)
            {
                MessageBox.Show(this, "Please input two different addresses.");
                return;
            }

            //Check if the user entered an address with less than 7 chars
            if (AddressOne.Text.Length < 7 || AddressTwo.Text.Length < 7)
            {
                MessageBox.Show(this, "Please check to see if both addresses are 7-8 characters long.");
                return;
            }

            PtARDS.Text = "";
            PointerResults.Text = "";

            StringBuilder ptrCode = new StringBuilder();
            StringBuilder ptrOutput = new StringBuilder();

            int HexCheck = int.Parse(HexValue.Text, NumberStyles.AllowHexSpecifier);
            int MaxOffsetTest = int.Parse(MaxOffset.Text, NumberStyles.HexNumber);

            //get the code type based on the Hex Value input
            int CodeType = HexCheck >= 0 && HexCheck <= 255
                           ? 2
                           : HexCheck > 255 && HexCheck <= 65535
                             ? 1
                             : 0;

            int DesiredOffset = Positive.IsChecked == true
                                ? Math.Abs(MaxOffsetTest)
                                : -Math.Abs(MaxOffsetTest);

            //read the contents of the files
            BinaryReader File1 = new BinaryReader(openFileOne.OpenFile());
            BinaryReader File2 = new BinaryReader(openFileTwo.OpenFile());

            //parse the text to an integer
            int Addy1 = int.Parse(AddressOne.Text, NumberStyles.HexNumber);
            int Addy2 = int.Parse(AddressTwo.Text, NumberStyles.HexNumber);

            //loop through the file, from 0 to file size in bytes (32-bit aligned)
            for (int i = 0; i < Length1; i += sizeof(int))
            {
                int Offset1 = Addy1 - File1.ReadInt32();
                int Offset2 = Addy2 - File2.ReadInt32();

                /*Based on the checkbox that was checked:
                 *when positive is checked, it'll check to see if the offset is than or equal to
                 *the positive max offset the user entered and it will then check if the offset
                 *is actually positive (greater than 0).
                 *
                 *when negative is checked, it'll check to see if the offset is greater than or
                 *equal to the negative max offset the user entered and it will then check if the
                 *offset is actually negative (less than 0)
                 */

                if ((Offset1 == Offset2) &&
                    ((Offset1 <= DesiredOffset && Offset1 > 0) ||
                     (Offset1 >= DesiredOffset && Offset1 < 0)))
                    ptrOutput.Append(String.Format("0x{0:X8} : 0x{1:X8} :: 0x{2:X8}\n",
                                                   i + 0x02000000, Addy1 - Offset1, Offset1));
            }

            PointerResults.Text = ptrOutput.ToString().Trim();

            /*Smallest = smallest value, 
             *SmallestAddy1 = smallest address 1
             */

            int Smallest = 0;
            string SmallestAddy1 = "";

            foreach (string Address in PointerResults.Text.Split('\n'))
            {
                //make sure we didn't grab a blank
                if (Address.Trim() == "")
                    continue;

                //parse the offset value
                int SmallCheck = int.Parse(Address.Substring(29, 8),
                                           NumberStyles.AllowHexSpecifier);

                //if the result is the best so far, save some data about it
                if (Smallest == 0 || SmallCheck < Smallest)
                {
                    Smallest = SmallCheck;
                    SmallestAddy1 = Address.Substring(3, 7);
                }
            }

            ptrCode.Append(String.Format("6{0} 00000000\nB{0} 00000000\n",
                                         SmallestAddy1));
            ptrCode.Append(
                Smallest < 0
                //negative offset
                ? String.Format(
                    "DC000000 {0:X8}\n" +
                    "{1:X}8000000 {2:X8}\n" +
                    "D2000000 00000000",
                    Smallest - 0x08000000,
                    CodeType,
                    HexCheck
                 )
                 : String.Format(
                     "{0:X}{1:X7} {2:X8}\n" +
                     "D2000000 00000000",
                     CodeType,
                     Smallest,
                     HexCheck
                 )
            );

            PtARDS.Text = ptrCode.ToString();
        }
        #endregion

        #region CodePorter
        private void CodePort_Click(object sender, RoutedEventArgs e)
        {
            string Ported = "";
            CodeOutput.Text = "";
            StringBuilder cb = new StringBuilder();

            foreach (string line in CodeInput.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (Valid_Code(line, @"[0-F]{8}\s[0-F]{8}") &&
                    !Valid_Code(line, @"[CD][0-2C4-5]0{6}\s[0-F]{8}|[3-A]4[0-F]{6}\s[0-F]{8}|927[0-F]{5}\s[0-F]{8}"))
                {
                    string AddyOnly = line.Substring(0, 8);
                    string ValyOnly = line.Substring(9, 8);
                    int AddyConvert = int.Parse(AddyOnly, NumberStyles.AllowHexSpecifier);
                    int ValyConvert = int.Parse(ValyOnly, NumberStyles.AllowHexSpecifier);
                    int OffyConvert = int.Parse(CodeOffset.Text, NumberStyles.AllowHexSpecifier);

                    if (Valid_Code(line, @"D[36-B]0{6}\s[0-F]{8}")) //Dx Lines
                    {
                        Ported = CodeAdd.IsChecked == true ? (ValyConvert + OffyConvert).ToString("X8") :
                            (ValyConvert - OffyConvert).ToString("X8");
                        cb.Append(AddyOnly.ToUpper() + " " + Ported.ToUpper() + nLine);
                    }
                    else //non-Dx lines
                    {
                        Ported = CodeAdd.IsChecked == true ? (AddyConvert + OffyConvert).ToString("X8") :
                            (AddyConvert - OffyConvert).ToString("X8");
                        cb.Append(Ported.ToUpper() + " " + ValyOnly.ToUpper() + nLine);
                    }
                }
                else cb.Append(line + nLine); //everything else
            }
            CodeOutput.Text = cb.ToString();
        }

        private void CodeCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CodeOutput.Text);
        }

        private void CodePaste_Click(object sender, RoutedEventArgs e)
        {
            CodeInput.Paste();
        }

        private void CodeClear_Click(object sender, RoutedEventArgs e)
        {
            CodeInput.Text = "";
            CodeOutput.Text = "";
            CodeOffset.Text = "";
        }
        #endregion

        #region PatchCodeBuilder
        private void PatchBuild_Click(object sender, RoutedEventArgs e)
        {
            PatchOutput.Text = "";
            PatchInput.Text = PatchInput.Text.Trim();
            StringBuilder pb = new StringBuilder();
            String[] PatchCode = PatchInput.Text.Split(new String[] { Environment.NewLine }, StringSplitOptions.None);

            int CodeOffset = PatchInput.LineCount; //get the number of lines
            int CodeCheck = CodeOffset;
            CodeOffset *= 4; //multiply by 4 to get the offset

            string CodeAddress = PatchCode[0].Substring(1, 8);
            pb.Append("E" + CodeAddress + CodeOffset.ToString("X8") + nLine);

            for (int y = 0; y < PatchCode.Length; y++)
            {
                string CodeValues = PatchCode[y].Substring(9, 8);

                if (y % 2 == 0)
                    pb.Append(CodeValues + " ");
                else
                    pb.Append(CodeValues + nLine);
            }

            if (CodeCheck % 2 != 0)
                pb.Append("00000000");

            PatchOutput.Text = pb.ToString();
        }

        private void PatchPaste_Click(object sender, RoutedEventArgs e)
        {
            PatchInput.Paste();
        }

        private void PatchCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(PatchOutput.Text);
        }

        private void PatchClear_Click(object sender, RoutedEventArgs e)
        {
            PatchInput.Text = "";
            PatchOutput.Text = "";
        }
        #endregion

        #region LoopCodeGenerator
        private void LoopGen_Click(object sender, RoutedEventArgs e)
        {
            bool run = true;
            string check = "";

            string TempCount = Convert.ToInt32(LoopCount.Text).ToString("X8");
            int HalfOffset = int.Parse(LoopOffset.Text, NumberStyles.AllowHexSpecifier);
            string FullOffset = HalfOffset.ToString("X8");

            //get the correct block offset by subtracting 1 and convert it to hex
            int nBlock = int.Parse(TempCount, NumberStyles.AllowHexSpecifier); nBlock -= 1;
            string ConvCount = nBlock.ToString("X8");

            //check offset
            int TempOffset = int.Parse(LoopOffset.Text, NumberStyles.AllowHexSpecifier);

            if (TempOffset == 1) check = D8;
            else if (TempOffset == 2) check = D7;
            else if (TempOffset == 4) check = D6;
            else run = false;

            //Did the user enter a full code?
            if (LoopBase.Text.Length != 17)
            {
                LoopOutput.Text = "";
                MessageBox.Show(this, "Please enter a full code (XXXXXXXX YYYYYYYY).",
                    "Base Code Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (LoopBase.Text.Length == 17 && run)
            {
                string BaseAddy = LoopBase.Text.Substring(0, 8); //grab the address
                string BaseValy = LoopBase.Text.Substring(9, 8); //grab the value

                if (LoopBase.Text[0] == '0')
                {
                    LoopOutput.Text = D5 + BaseValy + nLine + C0 + ConvCount + nLine + check + BaseAddy + nLine;

                    if (LoopInc.Text != "") //if there's text in the increment textbox, add the value increment.
                    {
                        int HalfInc = int.Parse(LoopInc.Text, NumberStyles.AllowHexSpecifier);
                        string FullInc = HalfInc.ToString("X8");
                        LoopOutput.Text += D4 + FullInc + nLine + D2;
                    }
                    else LoopOutput.Text += D2; //else, don't add the value increment.
                }
                else
                {
                    LoopOutput.Text = "";
                    MessageBox.Show(this, "Please check to see if your base code starts with a '0' and if you've entered an offset increment of 1, 2, or 4.",
                        "Value Increment Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (LoopBase.Text.Length == 17 && !run)
            {
                if (LoopBase.Text[0] >= '0' && LoopBase.Text[0] < '3')
                    LoopOutput.Text = C0 + ConvCount + nLine + LoopBase.Text + nLine + DC + FullOffset + nLine + D2;
                else MessageBox.Show(this, "Invalid Data! Please start your code off with a 0, 1, or 2.", "Data Input Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoopCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(LoopOutput.Text);
        }

        private void LoopClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (UIElement element in loopGrid.Children)
            {
                if (element is TextBox)
                {
                    TextBox txtFields = (TextBox)element;
                    txtFields.Text = "";
                }
            }
        }
        #endregion
    }
}
