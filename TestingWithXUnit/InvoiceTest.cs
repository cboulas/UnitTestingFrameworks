using System;
using System.Collections.Generic;
using UnitTestingFrameworks.Builders;
using Xunit;

namespace TestingWithXUnit
{
    public class InvoiceTest
    {
        private const int InvoiceNumber = 5;
        private readonly InvoiceBuilder _invoiceBuilder;

        private static readonly DateTime RegularDueDate = DateTime.Today;
        private static readonly DateTime OverdueDueDate = DateTime.Today.AddMonths(-2);
        
        public InvoiceTest()
        {
            _invoiceBuilder = new InvoiceBuilder()
                .WithInvoiceNumber(InvoiceNumber);
        }

        [Fact]
        public void CalculatePriceOnNewInvoice()
        {
            var invoice = _invoiceBuilder.Build();

            Assert.Equal(0, invoice.InvoiceTotal);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void CalculateInvoiceTotalWithOnlySimpleProducts(decimal itemPrice)
        {
            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(itemPrice))
                .WithDueDate(DateTime.Today)
                .Build();

            Assert.Equal(itemPrice, invoice.InvoiceTotal);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(15)]
        public void CalculateInvoiceWhenOverdue(decimal itemPrice)
        {
            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(itemPrice))
                .WithDueDate(DateTime.Today.AddDays(-35))
                .Build();

            var overduePrice = itemPrice * 1.5m;
            Assert.Equal(overduePrice, invoice.InvoiceTotal);
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public static IEnumerable<object[]> DueDateTestData = new List<object[]>
        {
            new object[] { RegularDueDate, 10m, 10m },
            new object[] { OverdueDueDate, 10m, 15m }
        };

        [Theory]
        [MemberData(nameof(DueDateTestData))]
        public void CalculateInvoiceTotalWithDueDate(DateTime dueDate, decimal itemPrice, decimal expectedTotal)
        {
            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(itemPrice))
                .WithDueDate(dueDate)
                .Build();

            Assert.Equal(expectedTotal, invoice.InvoiceTotal);
        }
    }
}
