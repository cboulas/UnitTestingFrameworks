using System;
using FluentAssertions;
using NUnit.Framework;
using UnitTestingFrameworks.Builders;

namespace TestingWithNUnit
{
    [TestFixture]
    public class InvoiceTest
    {
        private const int InvoiceNumber = 5;
        private InvoiceBuilder _invoiceBuilder;

        private static readonly DateTime RegularDueDate = DateTime.Today;
        private static readonly DateTime OverdueDueDate = DateTime.Today.AddMonths(-2);

        [SetUp]
        public void SetUp()
        {
            _invoiceBuilder = new InvoiceBuilder()
                .WithInvoiceNumber(InvoiceNumber);
        }

        [Test]
        public void CalculatePriceOnNewInvoice()
        {
            var invoice = _invoiceBuilder.Build();

            invoice.InvoiceTotal.Should().Be(0);
        }

        [DatapointSource]
        // ReSharper disable once UnusedMember.Local
        private decimal[] _calculateInvoiceTestData = { -5m, 5m, 10m, 15m };

        [Theory]
        public void CalculateInvoiceTotalWithOnlySimpleProducts(decimal itemPrice)
        {
            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(itemPrice))
                .WithDueDate(RegularDueDate)
                .Build();

            invoice.InvoiceTotal.Should().Be(itemPrice);
        }

        [Theory]
        public void CalculateInvoiceWhenOverdue(decimal itemPrice)
        {
            Assume.That(itemPrice > 0);

            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(10))
                .WithDueDate(OverdueDueDate)
                .Build();

            invoice.InvoiceTotal.Should().Be(15);
        }
        
        private static object[] _dueDateTestData = {
            new object[] { RegularDueDate, 10m, 10m },
            new object[] { OverdueDueDate, 10m, 15m }
        };

        [Test, TestCaseSource(nameof(_dueDateTestData))]
        public void CalculateInvoiceTotalWithDueDate(DateTime dueDate, decimal itemPrice, decimal expectedTotal)
        {
            var invoice = _invoiceBuilder
                .WithProduct(pb => pb.WithPrice(itemPrice))
                .WithDueDate(dueDate)
                .Build();

            invoice.InvoiceTotal.Should().Be(expectedTotal);
        }
    }
}
