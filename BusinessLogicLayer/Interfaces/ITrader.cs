namespace BusinessLogicLayer.Interfaces
{
    public interface ITrader
    {
        /// <summary>
        /// Accept button has already been pressed so now:
        /// - The MessageBox and Type Columns adjusted
        /// - The Filter Needs to be Set
        /// - The Credits for the Tradee displayed
        /// - The Collection needs to be read
        /// </summary>
        void StartTrade();
    }
}