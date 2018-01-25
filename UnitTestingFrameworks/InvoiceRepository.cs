using System;
using System.Data;
using System.Threading.Tasks;

namespace UnitTestingFrameworks
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IDbConnection _db;
        public event EventHandler<InvoiceBeforeSavingEventArgs> BeforeSaving;
        public event EventHandler Saved;

        public InvoiceRepository(IDbConnection db)
        {
            _db = db;
        }

        protected virtual void OnBeforeSaving(InvoiceBeforeSavingEventArgs e)
        {
            BeforeSaving?.Invoke(this, e);
        }

        protected virtual void OnSaved(EventArgs e)
        {
            Saved?.Invoke(this, e);
        }

        public async Task<bool> StoreInvoice(Invoice invoice)
        {
            if (invoice.InvoiceNumber <= 0)
                throw new InvalidInvoiceNumberException();

            var beforeSavingEventArgs = new InvoiceBeforeSavingEventArgs();
            OnBeforeSaving(beforeSavingEventArgs);

            if (beforeSavingEventArgs.Cancel) return false;

            // Process of saving to a remote database would be here...
            await Task.Delay(500);
            
            OnSaved(EventArgs.Empty);

            return true;
        }
    }
}