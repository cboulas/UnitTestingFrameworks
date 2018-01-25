using System;

namespace UnitTestingFrameworks
{
    public class InvoiceBeforeSavingEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}