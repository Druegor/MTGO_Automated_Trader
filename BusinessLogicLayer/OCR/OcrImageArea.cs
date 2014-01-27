using System.Drawing;
using Framework;
using Framework.Logging;

namespace BusinessLogicLayer.OCR
{
    /// <summary>
    /// Object containing the screen recognition code.
    /// </summary>
    public class OcrImageArea
    {
        private ILogger _logger = IoC.GetLoggerFor<OcrImageArea>();

        private Square _area;
        private string _matchingText;
        private Point _button;

        private OcrImageArea() { }

        /// <summary>
        /// Get the database values for the area of the screen for a given resolution and OS.
        /// </summary>
        public static OcrImageArea GetAreaFor(Point topLeft, Point bottomRight, string matchingText, Point buttonLocation)
        {
            return new OcrImageArea()
                       {
                           _area = Square.Initialize(topLeft, bottomRight),
                           _matchingText = matchingText,
                           _button = buttonLocation
                       };
        }

        public static OcrImageArea GetAreaFor(Square area, string matchingText, Point buttonLocation)
        {
            return new OcrImageArea()
            {
                _area = area,
                _matchingText = matchingText,
                _button = buttonLocation
            };
        }

        /// <summary>
        /// Determines if the screen contains the Window or Control.  If it does and the control has a button it is pressed prior to returning true.
        /// </summary>
        /// <returns>Bool which is the result of looking for the Window / Control.</returns>
        public bool CheckForOCRValue(bool pressButton = true)
        {
            var ocrValue = _area.GetOCRValue();
            _logger.Trace("Ocr Value: " + ocrValue);
            if (ocrValue.ToLower().Contains(_matchingText.ToLower()))
            {
                if (pressButton && !this._button.IsEmpty)
                {
                    AutoItX.MouseMove(this._button);
                    AutoItX.Sleep(100);
                    AutoItX.MouseClick(this._button);
                    AutoItX.Sleep(100);
                }

                return true;
            }

            return false;
        }
    }

    public class Square   
    {
        private ILogger _logger = IoC.GetLoggerFor<Square>();

        private Point _topLeft;
        private Point _bottomRight;

        private Square(){}

        public static Square Initialize(int top, int left, int bottom, int right)
        {
            return Initialize(new Point(left, top), new Point(right, bottom));
        }

        public static Square Initialize(Point topLeft, Point bottomRight)
        {
            return new Square()
                       {
                           _topLeft = topLeft,
                           _bottomRight = bottomRight
                       };
        }

        public string GetOCRValue()
        {
            Ocr ocr = new Ocr();
            var value = ocr.ExtractTextFromScreen(_topLeft, _bottomRight).Trim();
            // _logger.TraceFormat("Ocr for {0},{1} -> {2},{3} is : {4}", LeftEdge, TopEdge, RightEdge, BottomEdge, value);
            return value;
        }

        public int LeftEdge
        {
            get { return _topLeft.X; }
        }

        public int TopEdge
        {
            get { return _topLeft.Y; }
        }

        public int RightEdge
        {
            get { return _bottomRight.X; }
        }

        public int BottomEdge
        {
            get { return _bottomRight.Y; }
        }

        public Point TopLeft
        {
            get { return _topLeft; }
        }

        public Point BottomRight
        {
            get { return _bottomRight; }
        }

        public Point MidPoint
        {
            get { return new Point((LeftEdge + RightEdge)/2, (TopEdge + BottomEdge)/2); }
        }

        /// <summary>
        /// Positive moves the square to the right
        /// </summary>
        public Square MoveAlongXAxis(int x)
        {
            _topLeft.X += x;
            _bottomRight.X += x;
            return this;
        }

        /// <summary>
        /// Positive moves the square downwards
        /// </summary>
        public Square MoveAlongYAxis(int y)
        {
            _topLeft.Y += y;
            _bottomRight.Y+= y;
            return this;
        }

        public Square MoveTopEdgeTo(int newY)
        {
            _topLeft.Y = newY;
            return this;
        }

        public Square MoveBottomEdgeTo(int newY)
        {
            _bottomRight.Y = newY;
            return this;
        }

        public Square Copy()
        {
            return new Square
                       {
                           _topLeft = this.TopLeft,
                           _bottomRight = this.BottomRight
                       };

        }

        public override string ToString()
        {
            return string.Format("Top: {0}, Left: {1}, Bottom: {2}, Right: {3}", this.TopEdge, this.LeftEdge,
                                 this.BottomEdge, this.RightEdge);
        }
    }
}
