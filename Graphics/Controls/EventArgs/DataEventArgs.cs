namespace Charts.Controls.EventArgs
{
    public class DataEventArgs : System.EventArgs
    {
        public IReadOnlyDictionary<double, DateTime> AllData { get; set; }
    }
}
