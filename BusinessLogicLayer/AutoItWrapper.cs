using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using BusinessLogicLayer.OCR;

namespace BusinessLogicLayer
{
    public class AutoItX
    {
        //These are used in a few functions to return multiple values(GetWinPos, MouseGetPos) 

        public struct Position2D
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
        }

        public struct PointXY
        {
            public int X;
            public int Y;
        }

        ///<summary> 
        /// Disable/enable the mouse and keyboard. /// 
        /// </summary>  /// 
        /// <param name="flag">1 = Disable user input /// 0 = Enable user input</param>  /// 
        /// <returns>Success: Returns 1. /// Failure: Returns 0. Already Enable or #requireAdmin not used.</returns>  
        public static void BlockInput(int flag)
        {
            AU3_BlockInput(flag);
        }

        /// <summary>  
        ///  Sends a mouse click command to a given control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <param name="vsButton">[optional] The button to click, "left", "right", "middle", "main", "menu", "primary", "secondary". Default is the left button.</param>  
        ///  <param name="vsNumClicks">[optional] The number of times to click the mouse. Default is 1.</param>  
        ///  <param name="vsX">[optional] The x position to click within the control. Default is center.</param>  
        ///  <param name="vsY">[optional] The y position to click within the control. Default is center.</param>   
        /// <returns>Success: Returns 1. /// Failure: Returns 0.</returns>  
        public static int ControlClick(string vsTitle, string vsText, string vsControl, string vsButton, int vsNumClicks, int vsX, int vsY)
        {
            return AU3_ControlClick(vsTitle, vsText, vsControl, vsButton, vsNumClicks, vsX, vsY);
        }

        /// <summary>  
        /// Sends a mouse click command to a given control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0.</returns> 
        public static int ControlClick(string vsTitle, string vsControl)
        {
            return AU3_ControlClick(vsTitle, "", vsControl, "", 1, 0, 0);
        }
        /// <summary>  
        /// Enables a "grayed-out" control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0.</returns>  
        public static int ControlDisable(string vsTitle, string vsText, string vsControl)
        {
            return AU3_ControlDisable(vsTitle, vsText, vsControl);
        }

        /// <summary>  
        ///  Disables a "grayed-out" control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0.</returns>  
        public static int ControlEnable(string vsTitle, string vsText, string vsControl)
        {
            return AU3_ControlEnable(vsTitle, vsText, vsControl);
        }

        /// <summary>  
        ///  Sets input focus to a given control on a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0.</returns>  
        public static int ControlFocus(string vsTitle, string vsText, string vsControl)
        {
            return AU3_ControlFocus(vsTitle, vsText, vsControl);
        }

        /// <summary>  
        ///  Sets input focus to a given control on a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0.</returns>  
        public static int ControlFocus(string vsTitle, string vsControl)
        {
            return AU3_ControlFocus(vsTitle, "", vsControl);
        }

        /// <summary>  
        ///  Retrieves the internal handle of a control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        public static string ControlGetHandle(string vsTitle, string vsText, string vsControl)
        {
            // A number big enought to hold result, trailing bytes will be 0 
            StringBuilder retText = new StringBuilder(50);
            AU3_ControlGetHandle(vsTitle, vsText, vsControl, retText, 50);

            // May need to convert back to a string or int 
            return retText.ToString().Trim();
        }

        /// <summary>  
        ///  Retrieves the position and size of a control relative to it's window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Returns a ControlPosition struct containing the size and the control's position relative to it's client window: /// ControlPosition { int X, int Y, int Width, int Height }</returns>  
        public static Position2D ControlGetPos(string vsTitle, string vsText, string vsControl)
        {
            Position2D pos;
            pos.X = AU3_ControlGetPosX(vsTitle, vsText, vsControl);
            pos.Y = AU3_ControlGetPosY(vsTitle, vsText, vsControl);
            pos.Height = AU3_ControlGetPosHeight(vsTitle, vsText, vsControl);
            pos.Width = AU3_ControlGetPosWidth(vsTitle, vsText, vsControl);
            return pos;
        }

        /// <summary>  
        ///  Hides a control. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0 if window/control is not found.</returns>  
        public static int ControlHide(string vsTitle, string vsText, string vsControl)
        {
            return AU3_ControlHide(vsTitle, vsText, vsControl);
        }

        /// <summary>  
        ///  Moves a control within a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <param name="vsX">X coordinate to move to relative to the window client area.</param>  
        ///  <param name="vsY">Y coordinate to move to relative to the window client area.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0 if window/control is not found.</returns>  
        public static int ControlMove(string vsTitle, string vsText, string vsControl, int vsX, int vsY)
        {
            int height = ControlGetPos(vsTitle, vsText, vsControl).Height;
            int width = ControlGetPos(vsTitle, vsText, vsControl).Width;
            return AU3_ControlMove(vsTitle, vsText, vsControl, vsX, vsY, height, width);
        }

        /// <summary>  
        /// Moves a control within a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <param name="vsX">X coordinate to move to relative to the window client area.</param>  
        ///  <param name="vsY">Y coordinate to move to relative to the window client area.</param>  
        ///  <param name="vsHeight">New width of the window.</param>  /// <param name="vsWidth">New height of the window.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0 if window/control is not found.</returns>  
        public static int ControlMove(string vsTitle, string vsText, string vsControl, int vsX, int vsY, int vsHeight, int vsWidth)
        {
            return AU3_ControlMove(vsTitle, vsText, vsControl, vsX, vsY, vsHeight, vsWidth);
        }

        /// <summary>  
        ///  Sends a string of characters to a control 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <param name="vsSendText">String of characters to send to the control.</param>  
        ///  <param name="vsMode">  
        ///  [optional] Changes how "keys" is processed: 
        ///  flag = 0 (default), Text contains special characters like + to indicate 
        ///  SHIFT and {LEFT} to indicate left arrow. 
        ///  flag = 1, keys are sent raw. 
        ///  </param>  
        ///  <returns>Success: Returns 1. 
        ///  Failure: Returns 0 if window/control is not found.
        /// </returns> 
        public static int ControlSend(string vsTitle, string vsText, string vsControl, string vsSendText, int vsMode)
        {
            return AU3_ControlSend(vsTitle, vsText, vsControl, vsSendText, vsMode);
        }

        /// <summary>  
        ///  Shows a control that was hidden. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to access.</param>  
        ///  <param name="vsText">The text of the window to access.</param>  
        ///  <param name="vsControl">The control to interact with. See Controls.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0 if window/control is not found.</returns>  
        public static int ControlShow(string vsTitle, string vsText, string vsControl)
        {
            return AU3_ControlShow(vsTitle, vsText, vsControl);
        }

        /// <summary>  
        ///  Perform a mouse click operation. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <param name="viX">[optional] The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used (default).</param>  
        ///  <param name="viY">[optional] The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used (default).</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns>  
        public static int MouseClick(string vsButton, int viX, int viY)
        {
            return MouseClick(vsButton, viX, viY, 1, 1);
        }

        public static int MouseClick(Point point)
        {
            return MouseClick("left", point.X, point.Y, 1, 1);
        }

        public static int DoubleClick(Point point)
        {
            return MouseClick("left", point.X, point.Y, 2, 1);
        }

        /// <summary>  
        ///  Perform a mouse click operation. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns>  
        public static int MouseClick(string vsButton)
        {
            int mousex = AU3_MouseGetPosX();
            int mousey = AU3_MouseGetPosY();
            return MouseClick(vsButton, mousex, mousey);
        }

        /// <summary>  
        ///  Perform a mouse click operation. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <param name="viX">[optional] The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used (default).</param>  
        ///  <param name="viY">[optional] The x/y coordinates to move the mouse to. If no x and y coords are given, the current position is used (default).</param>  
        ///  <param name="viClicks">[optional] The number of times to click the mouse. Default is 1.</param>  
        ///  <param name="viSpeed">[optional] the speed to move the mouse in the range 1 (fastest) to 100 (slowest). A speed of 0 will move the mouse instantly. Default speed is 10.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns> 
        public static int MouseClick(string vsButton, int viX, int viY, int viClicks, int viSpeed)
        {
            // MouseClick wasn't working with out first MouseMove call  
            AU3_MouseMove(viX, viY, 2);
            return AU3_MouseClick(vsButton, viX, viY, viClicks, viSpeed);
        }

        /// <summary>  
        ///  Perform a mouse click and drag operation. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <param name="viX1">The x/y coords to start the drag operation from.</param>  
        ///  <param name="viY1">The x/y coords to start the drag operation from.</param>  
        ///  <param name="viX2">The x/y coords to end the drag operation at.</param>  
        ///  <param name="viY2">The x/y coords to end the drag operation at.</param>  
        ///  <param name="viSpeed">[optional] the speed to move the mouse in the range 1 (fastest) to 100 (slowest). A speed of 0 will move the mouse instantly. Default speed is 10.</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns>  
        public static int MouseClickDrag(string vsButton, int viX1, int viY1, int viX2, int viY2, int viSpeed)
        {
            return AU3_MouseClickDrag(vsButton, viX1, viY1, viX2, viY2, viSpeed);
        }

        /// <summary>  
        ///  Perform a mouse down event at the current mouse position. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns>  
        public static void MouseDown(string vsButton)
        {
            AU3_MouseDown(vsButton);
        }

        /// <summary>  
        ///  Retrieves the current position of the mouse cursor. 
        ///  </summary>  
        ///  <returns>PointXY struct with x, y values.</returns>  
        public static PointXY MouseGetPos()
        {
            PointXY point;
            point.X = AU3_MouseGetPosX();
            point.Y = AU3_MouseGetPosY();
            return point;
        }

        /// <summary>  
        ///  Moves the mouse pointer. 
        ///  </summary>  
        ///  <param name="point">The screen coordinates to move the mouse to.</param>  
        public static void MouseMove(Point point)
        {
            MouseMove(point.X, point.Y, 1);
        }

        /// <summary>  
        ///  Moves the mouse pointer. 
        ///  </summary>  
        ///  <param name="viX">The screen x coordinate to move the mouse to.</param>  
        ///  <param name="viY">The screen y coordinate to move the mouse to.</param>  
        public static void MouseMove(int viX, int viY)
        {
            MouseMove(viX, viY, 1);
        }

        /// <summary>  
        ///  Moves the mouse pointer. 
        ///  </summary>  
        ///  <param name="viX">The screen x coordinate to move the mouse to.</param>  
        ///  <param name="viY">The screen y coordinate to move the mouse to.</param>  
        ///  <param name="viSpeed">[optional] the speed to move the mouse in the range 1 (fastest) to 100 (slowest). A speed of 0 will move the mouse instantly. Default speed is 10.</param>  
        public static void MouseMove(int viX, int viY, int viSpeed)
        {
            AU3_MouseMove(viX, viY, viSpeed);
        }

        /// <summary>  
        ///  Perform a mouse up event at the current mouse position. 
        ///  </summary>  
        ///  <param name="vsButton">The button to click: "left", "right", "middle", "main", "menu", "primary", "secondary".</param>  
        ///  <returns>Success: Returns 1. /// Failure: Returns 0, the button is not in the list.</returns>  
        public static void MouseUp(string vsButton)
        {
            AU3_MouseUp(vsButton);
        }

        /// <summary>  
        ///  Generates a checksum for a region of pixels. 
        ///  </summary>  
        ///  <param name="left">left coordinate of rectangle.</param>  
        ///  <param name="top">top coordinate of rectangle.</param>  
        ///  <param name="right">right coordinate of rectangle.</param>  
        ///  <param name="bottom">bottom coordinate of rectangle.</param>  
        ///  <param name="step">[optional] Instead of checksumming each pixel use a value larger than 1 to skip pixels (for speed). 
        /// E.g. A value of 2 will only check every other pixel. Default is 1. It is not recommended to use a step value greater than 1.</param>  
        ///  <returns>Returns the checksum value of the region.</returns>  
        public static int PixelChecksum(int left, int top, int right, int bottom, int step)
        {
            int sum = AU3_PixelChecksum(left, top, right, bottom, step);
            return sum;

            /*
             * int sum = 0; 
            for (int viX = left; viX <= right; viX += step)
            {
                for (int viY = top; viY <= bottom; viY += step)
                {
                    sum += AU3_PixelGetColor(viX, viY);
                }
            }
            return sum;
             * */
        }

        /// <summary>  
        ///  Generates a checksum for a region of pixels. 
        ///  </summary>  
        ///  <param name="topLeft">Top Left coordinate of the rectangle.</param>  
        ///  <param name="bottomRight">Bottom Right coordinate of the rectangle.</param>  
        ///  <returns>Returns the checksum value of the region.</returns> 
        public static int PixelChecksum(Point topLeft, Point bottomRight)
        {
            return PixelChecksum(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y, 1);
        }

        public static int PixelChecksum(Square area)
        {
            return PixelChecksum(area.LeftEdge, area.TopEdge, area.RightEdge, area.BottomEdge, 1);
        }

        /// <summary>  
        ///  Generates a checksum for a region of pixels. 
        ///  </summary>  
        ///  <param name="left">left coordinate of rectangle.</param>  
        ///  <param name="top">top coordinate of rectangle.</param>  
        ///  <param name="right">right coordinate of rectangle.</param>  
        ///  <param name="bottom">bottom coordinate of rectangle.</param>  
        ///  <returns>Returns the checksum value of the region.</returns> 
        public static int PixelChecksum(int left, int top, int right, int bottom)
        {
            return PixelChecksum(left, top, right, bottom, 10);
        }

        /// <summary>
        /// Returns a pixel color according to x,y pixel coordinates. 
        /// </summary>
        /// <param name="point">Point coordinates of the pixel.</param>
        /// <returns>Integer value of the color at the point.</returns>
        public static int PixelGetColor(Point point)
        {
            return PixelGetColor(point.X, point.Y);
        }

        /// <summary>  
        ///  Returns a pixel color according to x,y pixel coordinates. 
        ///  </summary>  
        ///  <param name="viX">x coordinate of pixel.</param>  
        ///  <param name="viY">y coordinate of pixel.</param>  
        ///  <returns></returns> 
        public static int PixelGetColor(int viX, int viY)
        {
            return AU3_PixelGetColor(viX, viY);
        }

        /// <summary>  
        ///  Searches a rectangle of pixels for the pixel color provided. ( NOT IMPLIMENTED ) 
        ///  </summary>  
        ///  <param name="left">left coordinate of rectangle.</param>  
        ///  <param name="top">top coordinate of rectangle.</param>  
        ///  <param name="right">right coordinate of rectangle.</param>  
        ///  <param name="bottom">bottom coordinate of rectangle.</param>  
        ///  <param name="color">Color value of pixel to find (in decimal or hex).</param>  
        ///  <param name="shade">[optional] A number between 0 and 255 to indicate the allowed number of shades of variation of the red, green, and blue components of the colour. Default is 0 (exact match).</param>  
        ///  <param name="step">[optional] Instead of searching each pixel use a value larger than 1 to skip pixels (for speed). E.g. A value of 2 will only check every other pixel. Default is 1.</param>  
        ///  <returns>  
        ///  an array [0]=x [1]=y if the pixel is found. returns null otherwise 
        ///  </returns>  
        public static int[] PixelSearch(int left, int top, int right, int bottom, int color, int shade, int step)
        {
            //int[] result = {0,0};
            //try
            //{
            // AU3_PixelSearch(0, 0, 800, 000,0xFFFFFF, 0, 1, result);
            //}
            //catch { }
            //It will crash if the color is not found, have not been able to determin why jcd
            //The AutoItX3Lib.AutoItX3Class version has similar problems and is the only function to return an object
            //so contortions are needed to get the data from it ie:
            //int[] result = {0,0};
            //object resultObj;
            //AutoItX3Lib.AutoItX3Class autoit = new AutoItX3Lib.AutoItX3Class();
            //resultObj = autoit.PixelSearch(0, 0, 800, 600, 0xFFFF00,0,1);
            //Type t = resultObj.GetType();
            //if(t == typeof(object[]))
            //{
            //object[] obj = (object[])resultObj;
            //result[0] = (int)obj[0];
            //result[1] = (int)obj[1];
            //}
            //When it fails it returns an object = 1 but when it succeeds it is object[X,Y]
            
            int[] result = { 0, 0 };
            AU3_PixelSearch(left, top, right, bottom, color, shade, step, result);
            //Here we check to see if it found the pixel or not. It always returns a 1 in C# if it did not. 
            //if (result.ToString() != "1")
            //{
            //    //We have to turn "object coord" into a useable array since it contains the coordinates we need. 
            //    object[] pixelCoord = (object[])coord;  //Now we cast the object array to integers so that we can use the data inside. 
            //    return new int[] { (int)pixelCoord[0], (int)pixelCoord[1] };
            //}
            //return null;
            return result;
        }

        /// <summary>  
        ///  Terminates a named process. 
        ///  </summary>  
        ///  <param name="process">The title or PID of the process to terminate.</param>  
        public static void ProcessClose(string process)
        {
            AU3_ProcessClose(process);
        }

        /// <summary>  
        ///  Checks to see if a specified process exists. 
        ///  </summary>  
        ///  <param name="process">The name or PID of the process to check. </param>  
        ///  <returns>true if it exist, false otherwise</returns>  
        public static bool ProcessExists(string process)
        {
            return AU3_ProcessExists(process) != 0;
        }

        /// <summary>  
        ///  Pauses script execution until a given process exists. 
        ///  </summary>  
        ///  <param name="process">The name of the process to check.</param>  
        ///  <param name="timeout">[optional] Specifies how long to wait (in seconds). Default is to wait indefinitely.</param> 
        public static void ProcessWait(string process, int timeout)
        {
            AU3_ProcessWait(process, timeout);
        }

        /// <summary>  
        ///  Pauses script execution until a given process does not exist. 
        ///  </summary>  
        ///  <param name="process">The name or PID of the process to check.</param>  
        ///  <param name="timeout">[optional] Specifies how long to wait (in seconds). Default is to wait indefinitely.</param> 
        public static void ProcessWaitClose(string process, int timeout)
        {
            AU3_ProcessWaitClose(process, timeout);
        }

        /// <summary>  
        ///  Runs an external program. 
        ///  </summary>  
        ///  <param name="process">The name of the executable (EXE, BAT, COM, or PIF) to run.</param>  
        public static void Run(string process)
        {
            Run(process, "");
        }

        /// <summary>  
        ///  Runs an external program. 
        ///  </summary>  
        ///  <param name="process">The name of the executable (EXE, BAT, COM, or PIF) to run.</param>  
        ///  <param name="dir">[optional] The working directory.</param>  
        public static void Run(string process, string dir)
        {
            Run(process, dir, SW_SHOWMAXIMIZED);
        }

        /// <summary>  
        ///  Runs an external program. 
        ///  </summary>  
        ///  <param name="process">The name of the executable (EXE, BAT, COM, or PIF) to run.</param>  
        ///  <param name="dir">[optional] The working directory.</param>  
        ///  <param name="showflag">  
        ///  [optional] The "show" flag of the executed program: 
        ///  @SW_HIDE = Hidden window (or Default keyword) 
        ///  @SW_MINIMIZE = Minimized window 
        ///  @SW_MAXIMIZE = Maximized window 
        ///  </param> 
        public static void Run(string process, string dir, int showflag)
        {
            AU3_Run(process, dir, showflag);
        }

        /// <summary>  
        ///  Sends simulated keystrokes to the active window. 
        ///  </summary>  
        ///  <param name="text">The sequence of keys to send.</param>  
        ///  <defaults>KeyDownDelay and KeySendDelay are default 5 ms.</defaults> 
        public static void Send(string text)
        {
            Send(text, 5, 5);
        }

        /// <summary>  
        ///  Sends simulated keystrokes to the active window. 
        ///  </summary>  
        ///  <param name="text">The sequence of keys to send.</param>  
        ///  <param name="viSpeed">  
        ///  Alters the the length of the brief pause in between sent keystrokes. 
        ///  Time in milliseconds to pause (default=5). Sometimes a value of 0 does 
        ///  not work; use 1 instead. 
        ///  </param>  
        public static void Send(string text, int viSpeed)
        {
            Send(text, viSpeed, 5);
        }

        /// <summary>  
        ///  Sends simulated keystrokes to the active window. 
        ///  </summary>  
        ///  <param name="text">The sequence of keys to send.</param>  
        ///  <param name="viSpeed">  
        ///  Alters the the length of the brief pause in between sent keystrokes. 
        ///  Time in milliseconds to pause (default=5). Sometimes a value of 0 does 
        ///  not work; use 1 instead. 
        ///  </param>  
        ///  <param name="downLen">  
        ///  Alters the length of time a key is held down before being released during a keystroke. 
        ///  For applications that take a while to register keypresses (and many games) you may need 
        ///  to raise this value from the default. 
        ///  Time in milliseconds to pause (default=5). 
        ///  </param>  
        public static void Send(string text, int viSpeed, int downLen)
        {
            SetOptions("AuXWrapper.SendKeyDelay", viSpeed);
            SetOptions("AuXWrapper.SendKeyDownDelay", downLen);
            Send(0, text);
        }

        /// <summary>  
        ///  Sends simulated keystrokes to the active window. 
        ///  </summary>  
        ///  <param name="viMode">[optional] Changes how "keys" is processed: 
        ///  flag = 0 (default), Text contains special characters like + and ! to indicate SHIFT and ALT key-presses. 
        ///  flag = 1, keys are sent raw. 
        ///  </param>  
        ///  <param name="vsText">The sequence of keys to send.</param>  
        public static void Send(int viMode, string vsText)
        {
            AU3_Send(vsText, viMode);
        }

        /// <summary>  
        ///  Sends simulated keystrokes to the specified control 
        ///  </summary>  
        ///  <param name="vsTitle">The s window title.</param>  
        ///  <param name="vsControl">The s control.</param>  
        ///  <param name="vsText">The s text.</param>  
        public static void Send(string vsText, string vsTitle, string vsControl)
        {
            // Set control focus and then send text to that control  
            ControlFocus(vsTitle, vsControl);
            Send(vsText);
        }

        /// <summary>  
        ///  Set recommended default AutoIt functions/parameters 
        ///  </summary>  public static void SetOptions() { SetOptions(true, 250); } 
        ///  <summary>  
        ///  Changes the operation of various AutoIt functions/parameters. 
        ///  </summary>  
        ///  <param name="sOption">The option to change. See Remarks.</param>  
        ///  <param name="iValue">  
        ///  [optional] The value to assign to the option. The type and meaning 
        ///  vary by option. The keyword Default can be used for the parameter 
        ///  to reset the option to its default value. 
        ///  </param>  
        public static void SetOptions(string sOption, int iValue)
        {
            AU3_AutoItSetOption(sOption, iValue);
        }

        /// <summary>  
        ///  Set recommended default AutoIt functions/parameters 
        ///  </summary>  
        public static void SetOptions(bool vbWindowTitleExactMatch, int viSendKeyDelay)
        {
            // WinTitleMatchMode 
            // Alters the method that is used to match window titles during search operations. 
            // 1 = Match the title from the start (default) 
            // 2 = Match any substring in the title 
            // 3 = Exact title match 
            // 4 = Advanced mode, see Window Titles & Text (Advanced) 
            // -1 to -4 = force lower case match according to other type of match.  

            if (vbWindowTitleExactMatch)
            {
                // Match exact title when looking for windows  
                SetOptions("WinTitleMatchMode", 1);
            }
            else
            {

                // Match any substring in the title when looking for windows  
                SetOptions("WinTitleMatchMode", 2);
            }

            // SendKeyDelay 
            // Alters the the length of the brief pause in between sent keystrokes. 
            // Time in milliseconds to pause (default=5). Sometimes a value of 0 
            // does not work; use 1 instead.  
            SetOptions("SendKeyDelay", viSendKeyDelay);

            // WinWaitDelay 
            // Alters how long a script should briefly pause after a successful 
            // window-related operation. Time in milliseconds to pause (default=250). 
            SetOptions("WinWaitDelay", 250);

            // WinDetectHiddenText 
            // Specifies if hidden window text can be "seen" by the window matching functions. 
            // 0 = Do not detect hidden text (default) 
            // 1 = Detect hidden text  
            SetOptions("WinDetectHiddenText", 1);

            // CaretCoordMode 
            // Sets the way coords are used in the caret functions, either absolute 
            // coords or coords relative to the current active window: 
            // 0 = relative coords to the active window 
            // 1 = absolute screen coordinates (default) 
            // 2 = relative coords to the client area of the active window  
            SetOptions("CaretCoordMode", 2);

            // PixelCoordMode 
            // Sets the way coords are used in the pixel functions, either absolute 
            // coords or coords relative to the window defined by hwnd (default active window): 
            // 0 = relative coords to the defined window 
            // 1 = absolute screen coordinates (default) 
            // 2 = relative coords to the client area of the defined window  
            SetOptions("PixelCoordMode", 2);

            // MouseCoordMode 
            // Sets the way coords are used in the mouse functions, either absolute 
            // coords or coords relative to the current active window: 
            // 0 = relative coords to the active window 
            // 1 = absolute screen coordinates (default) 
            // 2 = relative coords to the client area of the active window  
            SetOptions("MouseCoordMode", 2);
        }

        /// <summary>  
        ///  Pause script execution. 
        ///  </summary>  
        ///  <param name="iMilliseconds">Amount of time to pause (in milliseconds).</param>  
        public static void Sleep(int iMilliseconds)
        {
            AU3_Sleep(iMilliseconds);
        }

        /// <summary>  
        ///  Creates a tooltip anywhere on the screen. 
        ///  </summary>  
        ///  <param name="sTip">The text of the tooltip. (An empty string clears a displaying tooltip)</param>  
        ///  <param name="viX">[optional] The x,y position of the tooltip.</param>  
        ///  <param name="viY">[optional] The x,y position of the tooltip.</param>  
        public static void ToolTip(string sTip, int viX, int viY)
        {
            AU3_ToolTip(sTip, viX, viY);
        }

        /// <summary>  
        ///  Creates a padded tooltip in the top-left part of the screen. 
        ///  </summary>  
        ///  <param name="sMessage">The text of the tooltip. (An empty string clears a displaying tooltip)</param>  
        public static void ToolTip(string sMessage)
        {
            // Pad the message being displayed  
            if (!string.IsNullOrEmpty(sMessage))
            {
                sMessage = string.Format("\r\n {0} \r\n ", sMessage);
            }
            // Set the tooltip to display the message  
            ToolTip(sMessage, 0, 0);
        }

        /// <summary>  
        /// Activate a window 
        ///  </summary>  
        ///  <param name="vsTitle">the complete title of the window to activate, case sensitive</param>  
        public static void WinActivate(string vsTitle)
        {
            AU3_WinActivate(vsTitle, "");
        }

        /// <summary>  
        ///  Activates (gives focus to) a window. ( incorporates WinWaitActive ) 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to activate.</param>  
        ///  <param name="waitactivetimeout">[optional] Timeout in seconds</param>  
        public static void WinActivate(string vsTitle, int waitactivetimeout)
        {
            AU3_WinActivate(vsTitle, "");
            AU3_WinWaitActive(vsTitle, "", waitactivetimeout);
            System.Threading.Thread.Sleep(1000);
        }

        /// <summary>  
        ///  Activates (gives focus to) a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to activate.</param>  
        ///  <param name="vsText">[optional] The text of the window to activate.</param>  
        public static void WinActivate(string vsTitle, string vsText)
        {
            AU3_WinActivate(vsTitle, vsText);
        }

        /// <summary>  
        ///  Checks to see if a specified window exists. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <returns>  
        ///  Success: Returns 1 if the window exists. 
        ///  Failure: Returns 0 otherwise. 
        ///  </returns>  
        public static int WinExists(string vsTitle)
        {
            return AU3_WinExists(vsTitle, "");
        }

        /// <summary>  
        ///  Checks to see if a specified window exists. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <param name="vsText">[optional] The text of the window to check.</param>  
        ///  <returns>  
        ///  Success: Returns 1 if the window exists. 
        ///  Failure: Returns 0 otherwise. 
        ///  </returns>  
        public static int WinExists(string vsTitle, string vsText)
        {
            return AU3_WinExists(vsTitle, vsText);
        }

        /// <summary>  
        ///  Retrieves the internal handle of a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to read.</param>  
        ///  <returns></returns>  
        public static string WinGetHandle(string vsTitle)
        {
            // A number big enought to hold result, trailing bytes will be 0  
            StringBuilder retText = new StringBuilder(50);
            AU3_WinGetHandle(vsTitle, "", retText, retText.Length);
            // May need to convert back to a string or int  
            return retText.ToString().Trim();
        }

        /// <summary>  
        ///  Retrieves the text from a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to read.</param>  
        ///  <param name="vsText">[optional] The text of the window to read.</param>  
        ///  <returns></returns>  
        public static string WinGetText(string vsTitle, string vsText)
        {
            // A number big enought to hold result, trailing bytes will be 0  
            StringBuilder retText = new StringBuilder(10000);
            AU3_WinGetText(vsTitle, vsText, retText, 10000);
            // May need to convert back to a string or int  
            return retText.ToString().TrimEnd('\0');
        }

        /// <summary>  
        ///  Retrieves the text from a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to read.</param>  
        ///  <returns></returns>  
        public static string WinGetText(string vsTitle) { return WinGetText(vsTitle, ""); }

        public Position2D WinGetPos(string vsTitle, string vsText)
        {
            Position2D pos;
            pos.X = AU3_WinGetPosX(vsTitle, vsText);
            pos.Y = AU3_WinGetPosY(vsTitle, vsText);
            pos.Height = AU3_WinGetPosHeight(vsTitle, vsText);
            pos.Width = AU3_WinGetPosWidth(vsTitle, vsText);
            return pos;
        }

        /// <summary>  
        ///  Moves and/or resizes a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to move/resize.</param>  
        ///  <param name="viX">X coordinate to move to.</param>  
        ///  <param name="viY">Y coordinate to move to.</param>  
        public static void WinMove(string vsTitle, int viX, int viY)
        {
            int xLen = AU3_WinGetPosWidth(vsTitle, "");
            int yLen = AU3_WinGetPosHeight(vsTitle, "");
            WinMove(vsTitle, viX, viY, xLen, yLen);
        }

        /// <summary>  
        ///  Moves and/or resizes a window. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to move/resize.</param>  
        ///  <param name="viX">X coordinate to move to.</param>  
        ///  <param name="viY">Y coordinate to move to.</param>  
        ///  <param name="viWidth">[optional] New width of the window.</param>  
        ///  <param name="viHeight">[optional] New height of the window.</param>  
        public static void WinMove(string vsTitle, int viX, int viY, int viWidth, int viHeight)
        {
            AU3_WinMove(vsTitle, "", viX, viY, viWidth, viHeight);
        }

        /// <summary>  
        ///  Pauses execution of the script until the requested window exists. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <returns></returns>  public static int WinWait(string vsTitle) { return AU3_WinWait(vsTitle, "", 3000); } 
        ///  <summary>  
        ///  Pauses execution of the script until the requested window is active. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <param name="vsText">[optional] The text of the window to check.</param>  
        ///  <param name="iTimeout">[optional] Timeout in seconds</param>  
        ///  <returns>  
        ///  Success: Returns 1. /// Failure: Returns 0 if timeout occurred. 
        ///  </returns>  
        public static int WinWaitActive(string vsTitle, string vsText, int iTimeout)
        {
            return AU3_WinWaitActive(vsTitle, vsText, iTimeout);
        }

        /// <summary>  
        ///  Pauses execution of the script until the requested window is active. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <param name="iTimeout">[optional] Timeout in seconds</param>  
        ///  <returns>  
        ///  Success: Returns 1. 
        ///  Failure: Returns 0 if timeout occurred. 
        ///  </returns>  
        public static int WinWaitActive(string vsTitle, int iTimeout)
        {
            return AU3_WinWaitActive(vsTitle, "", iTimeout);
        }

        /// <summary>  
        ///  Pauses execution of the script until the requested window is active. 
        ///  </summary>  
        ///  <param name="vsTitle">The title of the window to check.</param>  
        ///  <returns>  
        ///  Success: Returns 1. 
        ///  Failure: Returns 0 if timeout occurred. 
        ///  </returns>  
        public static int WinWaitActive(string vsTitle)
        {
            return AU3_WinWaitActive(vsTitle, "", 3000);
        }

        public static void WinSetTopmost(string title, int flag = 1)
        {
            AU3_WinSetOnTop(title, "", flag);
        }

        public static void MaximizeWindow(string title)
        {
            AU3_WinSetState(title, "", SW_MAXIMIZE);
        }

        public static bool IsWindowMaximized(string title)
        {
            var state = AU3_WinGetState(title, "");
            return state == 39;
        }

        //NOTE: This is based on AutoItX v3.3.0.0 which is a Unicode version
        //NOTE: My comments usually have "jcd" appended where I am uncertain.
        //NOTE: Optional parameters are not supported in C# yet so fill in all fields even if just "".
        //NOTE: Be prepared to play around a bit with which fields need values and what those value are.
        //NOTE: In previous versions we used byte[] to return values like this:
        //byte[] returnclip = new byte[200]; //at least twice as long as the text string expected +2 for null, (Unicode is 2 bytes)
        //AutoItX3Declarations.AU3_ClipGet(returnclip, returnclip.Length);
        //clipdata = System.Text.Encoding.Unicode.GetString(returnclip).TrimEnd('\0');

        //Now we are returning Unicode we can use StringBuilder instead like this:
        //StringBuilder clip = new StringBuilder(); //passing a parameter here will not work, we must asign a length
        //clip.Length = 200; //the number of chars expected plus 1 for the terminating null
        //AutoItX3Declarations.AU3_ClipGet(clip,clip.Length);
        //MessageBox.Show(clip.ToString());
        //NOTE: The big advantage of using AutoItX3 like this is that you don't have to register
        //the dll with windows and more importantly you get away from the many issues involved in
        //publishing the application and the binding to the dll required.

        //The below constants were found by Registering AutoItX3.dll in Windows
        //, adding AutoItX3Lib to References in IDE
        //,declaring an instance of it like this:
        // AutoItX3Lib.AutoItX3 autoit;
        // static AutoItX3Lib.AutoItX3Class autoit;
        //,right clicking on the AutoItX3Class and then Goto Definitions
        //and seeing the equivalent of the below in the MetaData window.
        //So far it is working

        //NOTE: easier way is to use "DLL Export Viewer" utility and get it to list Properties also
        //"DLL Export Viewer" is from http://www.nirsoft.net
        // Definitions
        public const int AU3_INTDEFAULT = -2147483647; // "Default" value for _some_ int parameters (largest negative number)
        public const int error = 1;
        public const int SW_HIDE = 2;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 4;
        public const int SW_RESTORE = 5;
        public const int SW_SHOW = 6;
        public const int SW_SHOWDEFAULT = 7;
        public const int SW_SHOWMAXIMIZED = 8;
        public const int SW_SHOWMINIMIZED = 9;
        public const int SW_SHOWMINNOACTIVE = 10;
        public const int SW_SHOWNA = 11;
        public const int SW_SHOWNOACTIVATE = 12;
        public const int SW_SHOWNORMAL = 13;
        public const int version = 110; //was 109 if previous non-unicode version

        /////////////////////////////////////////////////////////////////////////////////
        //// Exported functions of AutoItXC.dll
        /////////////////////////////////////////////////////////////////////////////////

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        //AU3_API void WINAPI AU3_Init(void);
        //Uncertain if this is needed jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Init();

        //AU3_API long AU3_error(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_error();

        //AU3_API long WINAPI AU3_AutoItSetOption(LPCWSTR szOption, long nValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_AutoItSetOption([MarshalAs(UnmanagedType.LPWStr)] string Option, int Value);

        //AU3_API void WINAPI AU3_BlockInput(long nFlag);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_BlockInput(int Flag);

        //AU3_API long WINAPI AU3_CDTray(LPCWSTR szDrive, LPCWSTR szAction);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_CDTray([MarshalAs(UnmanagedType.LPWStr)] string Drive
        , [MarshalAs(UnmanagedType.LPWStr)] string Action);

        //AU3_API void WINAPI AU3_ClipGet(LPWSTR szClip, int nBufSize);
        //Use like this:
        //StringBuilder clip = new StringBuilder();
        //clip.Length = 4;
        //AutoItX3Declarations.AU3_ClipGet(clip,clip.Length);
        //MessageBox.Show(clip.ToString());
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ClipGet([MarshalAs(UnmanagedType.LPWStr)]StringBuilder Clip, int BufSize);

        //AU3_API void WINAPI AU3_ClipPut(LPCWSTR szClip);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ClipPut([MarshalAs(UnmanagedType.LPWStr)] string Clip);

        //AU3_API long WINAPI AU3_ControlClick(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szButton, long nNumClicks, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlClick([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Button, int NumClicks, int X, int Y);

        //AU3_API void WINAPI AU3_ControlCommand(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlCommand([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extra
        , [MarshalAs(UnmanagedType.LPWStr)] StringBuilder Result, int BufSize);

        //AU3_API void WINAPI AU3_ControlListView(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra1, LPCWSTR szExtra2, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlListView([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extral1
        , [MarshalAs(UnmanagedType.LPWStr)] string Extra2, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder Result
        , int BufSize);

        //AU3_API long WINAPI AU3_ControlDisable(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlDisable([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlEnable(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlEnable([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlFocus(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlFocus([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API void WINAPI AU3_ControlGetFocus(LPCWSTR szTitle, LPCWSTR szText, LPWSTR szControlWithFocus
        //, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetFocus([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] StringBuilder ControlWithFocus
        , int BufSize);

        //AU3_API void WINAPI AU3_ControlGetHandle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPCWSTR szControl, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetHandle([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_ControlGetPosX(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosX([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlGetPosY(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosY([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlGetPosHeight(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosHeight([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlGetPosWidth(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlGetPosWidth([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API void WINAPI AU3_ControlGetText(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPWSTR szControlText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlGetText([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)]StringBuilder ControlText, int BufSize);

        //AU3_API long WINAPI AU3_ControlHide(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlHide([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API long WINAPI AU3_ControlMove(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, long nX, long nY, /*[in,defaultvalue(-1)]*/long nWidth, /*[in,defaultvalue(-1)]*/long nHeight);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlMove([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , int X, int Y, int Width, int Height);

        //AU3_API long WINAPI AU3_ControlSend(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szSendText, /*[in,defaultvalue(0)]*/long nMode);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlSend([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string SendText, int Mode);

        //AU3_API long WINAPI AU3_ControlSetText(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szControlText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlSetText([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string ControlText);

        //AU3_API long WINAPI AU3_ControlShow(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ControlShow([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control);

        //AU3_API void WINAPI AU3_ControlTreeView(LPCWSTR szTitle, LPCWSTR szText, LPCWSTR szControl
        //, LPCWSTR szCommand, LPCWSTR szExtra1, LPCWSTR szExtra2, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ControlTreeView([MarshalAs(UnmanagedType.LPWStr)] string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Control
        , [MarshalAs(UnmanagedType.LPWStr)] string Command, [MarshalAs(UnmanagedType.LPWStr)] string Extra1
        , [MarshalAs(UnmanagedType.LPWStr)] string Extra2, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);

        //AU3_API void WINAPI AU3_DriveMapAdd(LPCWSTR szDevice, LPCWSTR szShare, long nFlags
        //, /*[in,defaultvalue("")]*/LPCWSTR szUser, /*[in,defaultvalue("")]*/LPCWSTR szPwd
        //, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_DriveMapAdd([MarshalAs(UnmanagedType.LPWStr)] string Device
        , [MarshalAs(UnmanagedType.LPWStr)] string Share, int Flags, [MarshalAs(UnmanagedType.LPWStr)] string User
        , [MarshalAs(UnmanagedType.LPWStr)] string Pwd, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);

        //AU3_API long WINAPI AU3_DriveMapDel(LPCWSTR szDevice);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_DriveMapDel([MarshalAs(UnmanagedType.LPWStr)] string Device);

        //AU3_API void WINAPI AU3_DriveMapGet(LPCWSTR szDevice, LPWSTR szMapping, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_DriveMapGet([MarshalAs(UnmanagedType.LPWStr)] string Device
        , [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Mapping, int BufSize);

        //AU3_API long WINAPI AU3_IniDelete(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IniDelete([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key);

        //AU3_API void WINAPI AU3_IniRead(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey
        //, LPCWSTR szDefault, LPWSTR szValue, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_IniRead([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key
        , [MarshalAs(UnmanagedType.LPWStr)] string Default, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Value, int BufSize);

        //AU3_API long WINAPI AU3_IniWrite(LPCWSTR szFilename, LPCWSTR szSection, LPCWSTR szKey
        //, LPCWSTR szValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IniWrite([MarshalAs(UnmanagedType.LPWStr)] string Filename
        , [MarshalAs(UnmanagedType.LPWStr)] string Section, [MarshalAs(UnmanagedType.LPWStr)] string Key
        , [MarshalAs(UnmanagedType.LPWStr)] string Value);

        //AU3_API long WINAPI AU3_IsAdmin(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_IsAdmin();

        //AU3_API long WINAPI AU3_MouseClick(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY
        //, /*[in,defaultvalue(1)]*/long nClicks, /*[in,defaultvalue(-1)]*/long nSpeed);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseClick([MarshalAs(UnmanagedType.LPWStr)] string Button, int x, int y
        , int clicks, int speed);

        //AU3_API long WINAPI AU3_MouseClickDrag(LPCWSTR szButton, long nX1, long nY1, long nX2, long nY2
        //, /*[in,defaultvalue(-1)]*/long nSpeed);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseClickDrag([MarshalAs(UnmanagedType.LPWStr)] string Button
        , int X1, int Y1, int X2, int Y2, int Speed);

        //AU3_API void WINAPI AU3_MouseDown(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseDown([MarshalAs(UnmanagedType.LPWStr)] string Button);

        //AU3_API long WINAPI AU3_MouseGetCursor(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetCursor();

        //AU3_API long WINAPI AU3_MouseGetPosX(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetPosX();

        //AU3_API long WINAPI AU3_MouseGetPosY(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseGetPosY();

        //AU3_API long WINAPI AU3_MouseMove(long nX, long nY, /*[in,defaultvalue(-1)]*/long nSpeed);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_MouseMove(int X, int Y, int Speed);

        //AU3_API void WINAPI AU3_MouseUp(/*[in,defaultvalue("LEFT")]*/LPCWSTR szButton);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseUp([MarshalAs(UnmanagedType.LPWStr)] string Button);

        //AU3_API void WINAPI AU3_MouseWheel(LPCWSTR szDirection, long nClicks);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_MouseWheel([MarshalAs(UnmanagedType.LPWStr)] string Direction, int Clicks);

        //AU3_API long WINAPI AU3_Opt(LPCWSTR szOption, long nValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Opt([MarshalAs(UnmanagedType.LPWStr)] string Option, int Value);

        //AU3_API unsigned long WINAPI AU3_PixelChecksum(long nLeft, long nTop, long nRight, long nBottom
        //, /*[in,defaultvalue(1)]*/long nStep);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_PixelChecksum(int Left, int Top, int Right, int Bottom, int Step);

        //AU3_API long WINAPI AU3_PixelGetColor(long nX, long nY);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_PixelGetColor(int X, int Y);

        //AU3_API void WINAPI AU3_PixelSearch(long nLeft, long nTop, long nRight, long nBottom, long nCol
        //, /*default 0*/long nVar, /*default 1*/long nStep, LPPOINT pPointResult);
        //Use like this:
        //int[] result = {0,0};
        //try
        //{
        // AutoItX3Declarations.AU3_PixelSearch(0, 0, 800, 000,0xFFFFFF, 0, 1, result);
        //}
        //catch { }
        //It will crash if the color is not found, have not been able to determin why jcd
        //The AutoItX3Lib.AutoItX3Class version has similar problems and is the only function to return an object
        //so contortions are needed to get the data from it ie:
        //int[] result = {0,0};
        //object resultObj;
        //AutoItX3Lib.AutoItX3Class autoit = new AutoItX3Lib.AutoItX3Class();
        //resultObj = autoit.PixelSearch(0, 0, 800, 600, 0xFFFF00,0,1);
        //Type t = resultObj.GetType();
        //if(t == typeof(object[]))
        //{
        //object[] obj = (object[])resultObj;
        //result[0] = (int)obj[0];
        //result[1] = (int)obj[1];
        //}
        //When it fails it returns an object = 1 but when it succeeds it is object[X,Y]
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_PixelSearch(int Left, int Top, int Right, int Bottom, int Col, int Var
        , int Step, int[] PointResult);

        //AU3_API long WINAPI AU3_ProcessClose(LPCWSTR szProcess);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessClose([MarshalAs(UnmanagedType.LPWStr)]string Process);

        //AU3_API long WINAPI AU3_ProcessExists(LPCWSTR szProcess);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessExists([MarshalAs(UnmanagedType.LPWStr)]string Process);

        //AU3_API long WINAPI AU3_ProcessSetPriority(LPCWSTR szProcess, long nPriority);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessSetPriority([MarshalAs(UnmanagedType.LPWStr)]string Process, int Priority);

        //AU3_API long WINAPI AU3_ProcessWait(LPCWSTR szProcess, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessWait([MarshalAs(UnmanagedType.LPWStr)]string Process, int Timeout);

        //AU3_API long WINAPI AU3_ProcessWaitClose(LPCWSTR szProcess, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_ProcessWaitClose([MarshalAs(UnmanagedType.LPWStr)]string Process, int Timeout);

        //AU3_API long WINAPI AU3_RegDeleteKey(LPCWSTR szKeyname);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegDeleteKey([MarshalAs(UnmanagedType.LPWStr)]string Keyname);

        //AU3_API long WINAPI AU3_RegDeleteVal(LPCWSTR szKeyname, LPCWSTR szValuename);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegDeleteVal([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string ValueName);

        //AU3_API void WINAPI AU3_RegEnumKey(LPCWSTR szKeyname, long nInstance, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegEnumKey([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , int Instance, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BusSize);

        //AU3_API void WINAPI AU3_RegEnumVal(LPCWSTR szKeyname, long nInstance, LPWSTR szResult, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegEnumVal([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , int Instance, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder Result, int BufSize);

        //AU3_API void WINAPI AU3_RegRead(LPCWSTR szKeyname, LPCWSTR szValuename, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_RegRead([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string Valuename, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_RegWrite(LPCWSTR szKeyname, LPCWSTR szValuename, LPCWSTR szType
        //, LPCWSTR szValue);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RegWrite([MarshalAs(UnmanagedType.LPWStr)]string Keyname
        , [MarshalAs(UnmanagedType.LPWStr)]string Valuename, [MarshalAs(UnmanagedType.LPWStr)]string Type
        , [MarshalAs(UnmanagedType.LPWStr)]string Value);

        //AU3_API long WINAPI AU3_Run(LPCWSTR szRun, /*[in,defaultvalue("")]*/LPCWSTR szDir
        //, /*[in,defaultvalue(1)]*/long nShowFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Run([MarshalAs(UnmanagedType.LPWStr)]string Run
        , [MarshalAs(UnmanagedType.LPWStr)]string Dir, int ShowFlags);

        //AU3_API long WINAPI AU3_RunAsSet(LPCWSTR szUser, LPCWSTR szDomain, LPCWSTR szPassword, int nOptions);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RunAsSet([MarshalAs(UnmanagedType.LPWStr)]string User
        , [MarshalAs(UnmanagedType.LPWStr)]string Domain, [MarshalAs(UnmanagedType.LPWStr)]string Password
        , int Options);

        //AU3_API long WINAPI AU3_RunWait(LPCWSTR szRun, /*[in,defaultvalue("")]*/LPCWSTR szDir
        //, /*[in,defaultvalue(1)]*/long nShowFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_RunWait([MarshalAs(UnmanagedType.LPWStr)]string Run
        , [MarshalAs(UnmanagedType.LPWStr)]string Dir, int ShowFlags);

        //AU3_API void WINAPI AU3_Send(LPCWSTR szSendText, /*[in,defaultvalue("")]*/long nMode);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Send([MarshalAs(UnmanagedType.LPWStr)] string SendText, int Mode);

        //AU3_API void WINAPI AU3_SendA(LPCSTR szSendText, /*[in,defaultvalue("")]*/long nMode);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_SendA([MarshalAs(UnmanagedType.LPStr)] string SendText, int Mode);

        //AU3_API long WINAPI AU3_Shutdown(long nFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_Shutdown(int Flags);

        //AU3_API void WINAPI AU3_Sleep(long nMilliseconds);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_Sleep(int Milliseconds);

        //AU3_API void WINAPI AU3_StatusbarGetText(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(1)]*/long nPart, LPWSTR szStatusText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_StatusbarGetText([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, int Part, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder StatusText, int BufSize);

        //AU3_API void WINAPI AU3_ToolTip(LPCWSTR szTip, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nX
        //, /*[in,defaultvalue(AU3_INTDEFAULT)]*/long nY);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_ToolTip([MarshalAs(UnmanagedType.LPWStr)]string Tip, int X, int Y);

        //AU3_API void WINAPI AU3_WinActivate(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinActivate([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);

        //AU3_API long WINAPI AU3_WinActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);

        //AU3_API long WINAPI AU3_WinClose(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinClose([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API long WINAPI AU3_WinExists(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinExists([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);

        //AU3_API long WINAPI AU3_WinGetCaretPosX(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetCaretPosX();

        //AU3_API long WINAPI AU3_WinGetCaretPosY(void);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetCaretPosY();

        //AU3_API void WINAPI AU3_WinGetClassList(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetClassList([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_WinGetClientSizeHeight(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetClientSizeHeight([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);

        //AU3_API long WINAPI AU3_WinGetClientSizeWidth(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetClientSizeWidth([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text);

        //AU3_API void WINAPI AU3_WinGetHandle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetHandle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)]string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_WinGetPosX(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosX([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API long WINAPI AU3_WinGetPosY(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosY([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API long WINAPI AU3_WinGetPosHeight(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosHeight([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API long WINAPI AU3_WinGetPosWidth(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetPosWidth([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API void WINAPI AU3_WinGetProcess(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetProcess([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_WinGetState(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinGetState([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API void WINAPI AU3_WinGetText(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetText([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API void WINAPI AU3_WinGetTitle(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPWSTR szRetText, int nBufSize);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinGetTitle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)]StringBuilder RetText, int BufSize);

        //AU3_API long WINAPI AU3_WinKill(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinKill([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text);

        //AU3_API long WINAPI AU3_WinMenuSelectItem(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, LPCWSTR szItem1, LPCWSTR szItem2, LPCWSTR szItem3, LPCWSTR szItem4, LPCWSTR szItem5, LPCWSTR szItem6
        //, LPCWSTR szItem7, LPCWSTR szItem8);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinMenuSelectItem([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string Item1
        , [MarshalAs(UnmanagedType.LPWStr)] string Item2, [MarshalAs(UnmanagedType.LPWStr)] string Item3
        , [MarshalAs(UnmanagedType.LPWStr)] string Item4, [MarshalAs(UnmanagedType.LPWStr)] string Item5
        , [MarshalAs(UnmanagedType.LPWStr)] string Item6, [MarshalAs(UnmanagedType.LPWStr)] string Item7
        , [MarshalAs(UnmanagedType.LPWStr)] string Item8);

        //AU3_API void WINAPI AU3_WinMinimizeAll();
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinMinimizeAll();

        //AU3_API void WINAPI AU3_WinMinimizeAllUndo();
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern void AU3_WinMinimizeAllUndo();

        //AU3_API long WINAPI AU3_WinMove(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, long nX, long nY, /*[in,defaultvalue(-1)]*/long nWidth, /*[in,defaultvalue(-1)]*/long nHeight);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinMove([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int X, int Y, int Width, int Height);

        //AU3_API long WINAPI AU3_WinSetOnTop(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nFlag);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetOnTop([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Flags);

        //AU3_API long WINAPI AU3_WinSetState(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nFlags);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetState([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Flags);

        //AU3_API long WINAPI AU3_WinSetTitle(LPCWSTR szTitle,/*[in,defaultvalue("")]*/ LPCWSTR szText
        //, LPCWSTR szNewTitle);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetTitle([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, [MarshalAs(UnmanagedType.LPWStr)] string NewTitle);

        //AU3_API long WINAPI AU3_WinSetTrans(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText, long nTrans);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinSetTrans([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Trans);

        //AU3_API long WINAPI AU3_WinWait(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWait([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitActiveA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitActiveA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitClose(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitClose([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitCloseA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitCloseA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitNotActive(LPCWSTR szTitle, /*[in,defaultvalue("")]*/LPCWSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitNotActive([MarshalAs(UnmanagedType.LPWStr)]string Title
        , [MarshalAs(UnmanagedType.LPWStr)] string Text, int Timeout);

        //AU3_API long WINAPI AU3_WinWaitNotActiveA(LPCSTR szTitle, /*[in,defaultvalue("")]*/LPCSTR szText
        //, /*[in,defaultvalue(0)]*/long nTimeout);
        //Not checked jcd
        [DllImport("AutoItX3.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static public extern int AU3_WinWaitNotActiveA([MarshalAs(UnmanagedType.LPStr)]string Title
        , [MarshalAs(UnmanagedType.LPStr)] string Text, int Timeout);
    }
}