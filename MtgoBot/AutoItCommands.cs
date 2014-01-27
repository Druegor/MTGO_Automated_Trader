using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using BusinessLogicLayer;
using BusinessLogicLayer.OCR;

namespace AutoItBot
{
    public partial class AutoItCommands : Form
    {
        private const string Checksum = "Get Pixel Checksum";
        private const string PixelColor = "Get Pixel Color";
        private const string PixelPosition = "Get Point";
        private const string OCR = "Ocr";

        private int State { get; set; }

        public AutoItCommands()
        {
            InitializeComponent();
        }

        private void AutoItCommands_Load(object sender, EventArgs e)
        {
            this.State = 0;
            this.autoItAction.DataSource = this.GetAutoItActions();
        }

        private List<string > GetAutoItActions()
        {
            List<string> actions = new List<string>();
            actions.Add(Checksum);
            actions.Add(PixelColor);
            actions.Add(PixelPosition);
            actions.Add(OCR);
            return actions;
        }

        private void DoAutoItActionClick(object sender, EventArgs e)
        {
            switch (this.autoItAction.SelectedValue.ToString())
            {
                case Checksum:
                    DoChecksum();
                    // Start mouse hooks so that I can catch where on the screen the mouse is clicked
                    break;
                case PixelColor:
                    DoPixelGetColor();
                    break;
                case PixelPosition:
                    DoGetPosition();
                    break;
                case OCR:
                    DoGetOCR();
                    break;
                default:
                    break;
            }
        }

        private void DoGetOCR()
        {
            switch (this.State++)
            {
                case 0:
                    this.values.Text = string.Empty;
                    this.instructions.Text =
                        "Move the mouse to the top left position of the area you want to checksum and press the spacebar.";
                    break;
                case 1:
                    var point = AutoItX.MouseGetPos();
                    Point checkSumTopLeft = new Point(point.X, point.Y);
                    this.CheckSumTopLeft = checkSumTopLeft;
                    this.instructions.Text = "Now move the mouse to the bottom right and press the spacebar again.";
                    break;
                case 2:
                    point = AutoItX.MouseGetPos();
                    this.CheckSumBottomRight = new Point(point.X, point.Y);
                    Ocr ocr = new Ocr();
                    string ocrText = ocr.ExtractTextFromScreen(this.CheckSumTopLeft, this.CheckSumBottomRight);
                    this.instructions.Text =
                        "Copy the code below and paste it into your constants or ini file.  After pasting into your file replace all instances of 'checkSum' with the variable name that this belongs to.";
                    this.values.Text =
                        string.Format(
                            "public static readonly Point checkSumTopLeft = new Point({0}, {1}); \r public static readonly Point checkSumBottomRight = new Point({2}, {3}); \r public static readonly string text = {4};",
                            this.CheckSumTopLeft.X, this.CheckSumTopLeft.Y, this.CheckSumBottomRight.X,
                            this.CheckSumBottomRight.Y, ocrText);
                    State = 0;
                    break;
            }
        }

        private void DoGetPosition()
        {
            switch (this.State++)
            {
                case 0:
                    this.values.Text = string.Empty;
                    this.instructions.Text =
                        "Move the mouse to the position you want and press the spacebar.";
                    break;
                case 1:
                    var point = AutoItX.MouseGetPos();
                    this.instructions.Text = "Copy the code below and paste it into your constants or ini file.  After pasting into your file rename the variable.";
                    this.values.Text =
                        string.Format(
                            "public static readonly Point pixelPosition = new Point({0}, {1}); ",
                            point.X, point.Y);
                    State = 0;
                    break;
            }
        }

        private void DoPixelGetColor()
        {
            switch (this.State++)
            {
                case 0:
                    this.values.Text = string.Empty;
                    this.instructions.Text =
                        "Move the mouse to the position of the area you want to get the color from and press the spacebar.";
                    break;
                case 1:
                    var point = AutoItX.MouseGetPos();
                    int color = AutoItX.PixelGetColor(point.X, point.Y);
                    this.instructions.Text = "Copy the code below and paste it into your constants or ini file.  After pasting into your file rename the variables.";
                    this.values.Text =
                        string.Format(
                            "public static readonly Point pixelPosition = new Point({0}, {1}); \r public static readonly int color = {2};",
                            point.X, point.Y, color);
                    State = 0;
                    break;
            }
        }

        private Point CheckSumTopLeft { get; set; }
        private Point CheckSumBottomRight { get; set; }

        private void DoChecksum()
        {
            switch (this.State++)
            {
                case 0:
                    this.values.Text = string.Empty;
                    this.instructions.Text =
                        "Move the mouse to the top left position of the area you want to checksum and press the spacebar.";
                    break;
                case 1:
                    var point = AutoItX.MouseGetPos();
                    Point checkSumTopLeft = new Point(point.X, point.Y);
                    this.CheckSumTopLeft = checkSumTopLeft;
                    this.instructions.Text = "Now move the mouse to the bottom right and press the spacebar again.";
                    break;
                case 2:
                    point = AutoItX.MouseGetPos();
                    Point checkSumBottomRight = new Point(point.X, point.Y);
                    this.CheckSumBottomRight = checkSumBottomRight;
                    int checksum = AutoItX.PixelChecksum(this.CheckSumTopLeft.X, this.CheckSumTopLeft.Y,
                                                         this.CheckSumBottomRight.X, this.CheckSumBottomRight.Y);
                    this.instructions.Text = "Copy the code below and paste it into your constants or ini file.  After pasting into your file replace all instances of 'checkSum' with the variable name that this belongs to.";
                    this.values.Text =
                        string.Format(
                            "public static readonly Point checkSumTopLeft = new Point({0}, {1}); \r public static readonly Point checkSumBottomRight = new Point({2}, {3}); \r public static readonly int checkSumChecksum = {4};",
                            this.CheckSumTopLeft.X, this.CheckSumTopLeft.Y, this.CheckSumBottomRight.X,
                            this.CheckSumBottomRight.Y, checksum);
                    State = 0;
                    break;
            }
        }


        private void GetChecksum(object sender, EventArgs e)
        {
            int checksum = AutoItX.PixelChecksum(this.CheckSumTopLeft.X, this.CheckSumTopLeft.Y,
                                                         this.CheckSumBottomRight.X, this.CheckSumBottomRight.Y);
            this.values.Text +=
                string.Format(
                    "\r public static readonly int checkSumChecksum = {0};",
                    checksum);
        }

        private void CheckKeyPress(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Space)
            {
                DoAutoItActionClick(sender, EventArgs.Empty);
            }
        }

        private void ActionChanged(object sender, EventArgs e)
        {
            this.State = 0;
        }

    }


}
