using System;
using System.Threading.Tasks;

namespace UnitTestingFrameworks
{
    public interface IInvoiceRepository
    {
        event EventHandler<InvoiceBeforeSavingEventArgs> BeforeSaving;
        event EventHandler Saved;
        Task<bool> StoreInvoice(Invoice invoice);
    }
}