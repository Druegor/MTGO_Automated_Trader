using System.Drawing;
using BusinessLogicLayer;

namespace AutoItBot.Updater
{
    public static class Selector
    {
        private const string DownArrow = "{DOWN}";
        private const string UpArrow = "{UP}";

        public static int PickFilterOption(int currentValue, int nextValue, Point selectorPosition)
        {
            if (currentValue == nextValue)
            {
                return currentValue;
            }

            AutoItX.MouseClick(selectorPosition);
            AutoItX.Sleep(1000);
            if (nextValue > currentValue)
            {
                for (int i = currentValue; i < nextValue; i++)
                {
                    AutoItX.Send(DownArrow);
                    AutoItX.Sleep(500);
                }
            }
            else
            {
                for (int i = currentValue; i > nextValue; i--)
                {
                    AutoItX.Send(UpArrow);
                    AutoItX.Sleep(500);
                }
            }

            AutoItX.Sleep(1000);
            return nextValue;
        } 
    }
}