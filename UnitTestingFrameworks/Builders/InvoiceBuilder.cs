using System;

namespace UnitTestingFrameworks.Builders
{
    public class InvoiceBuilder
    {
        private Invoice _invoice;

        public InvoiceBuilder()
        {
            _invoice = new Invoice();
        }

        public InvoiceBuilder WithInvoiceNumber(int invoiceNumber)
        {
            _invoice.InvoiceNumber = invoiceNumber;
            return this;
        }

        public InvoiceBuilder WithClientName(string clientName)
        {
            _invoice.ClientName = clientName;
            return this;
        }

        public InvoiceBuilder WithProduct(Func<ProductBuilder, ProductBuilder> builder)
        {
            _invoice.Products.Add(builder(new ProductBuilder()).Build());
            return this;
        }

        public InvoiceBuilder WithDueDate(DateTime dueDate)
        {
            _invoice.DueDate = dueDate;
            return this;
        }

        public Invoice Build()
        {
            return _invoice;
        }

    }
}