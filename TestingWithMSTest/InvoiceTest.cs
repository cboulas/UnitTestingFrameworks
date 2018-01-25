using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestingFrameworks.Builders;

namespace TestingWithMSTest
{
    [TestClass]
    public class InvoiceTest
    {
        private const int InvoiceNumber = 5;
        private InvoiceBuilder _invoiceBuilder;

        private static readonly DateTime RegularDueDate = DateTime.Today;
        private static readonly DateTime OverdueDueDate = DateTime.Today.AddMonths(-2);

        [TestInitialize]
        public void SetUp()
        {
            _invoiceBuilder = new InvoiceBuilder()
                .WithInvoiceNumber(InvoiceNumber);
        }

        [TestMethod]
        public void CalculatePriceOnNewInvoice()
        {
            var invoice = _invoiceBuilder.Build();

            Assert.AreEqual(0, invoice.InvoiceTotal);
        }

        [DataRow(-5)]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(15)]
        [DataTestMethod]
        public void CalculateInvoiceTotalWithOnlySimpleProducts(int itemPrice)
        {
            var decimalItemPrice = Convert.ToDecimal(itemPrice);

            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(decimalItemPrice))
                .WithDueDate(RegularDueDate)
                .Build();

            Assert.AreEqual(decimalItemPrice, invoice.InvoiceTotal);
        }

        [DataTestMethod]
        [DataRow(5)]
        [DataRow(10)]
        [DataRow(15)]
        public void CalculateInvoiceWhenOverdue(int itemPrice)
        {
            var decimalItemPrice = Convert.ToDecimal(itemPrice);
            var overduePrice = decimalItemPrice * 1.5m;

            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(decimalItemPrice))
                .WithDueDate(OverdueDueDate)
                .Build();

            Assert.AreEqual(overduePrice, invoice.InvoiceTotal);
        }

        [TestMethod]
        public void CalculateInvoiceTotalWithDueDate(/*DateTime dueDate, decimal itemPrice, decimal expectedTotal*/)
        {
        }
    }
}
