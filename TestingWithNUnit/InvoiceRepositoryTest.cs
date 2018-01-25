using System;
using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using UnitTestingFrameworks;
using UnitTestingFrameworks.Builders;

namespace TestingWithNUnit
{
    [TestFixture]
    public class InvoiceRepositoryTest
    {
        private InvoiceBuilder _invoiceBuilder;
        private IDbConnection _db;
        private IInvoiceRepository _repository;

        [OneTimeSetUp]
        public void ClassSetup()
        {
            _db = new SqliteConnection("Data Source=TestingUnitTestFrameworks;");
            _db.Open();
        }

        [SetUp]
        public void SetUp()
        {
            _invoiceBuilder = new InvoiceBuilder();
            _repository = new InvoiceRepository(_db);
        }

        [Test]
        public void ThrowsInvalidInvoiceNumberExceptionWhenNoInvoiceNumber()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(0).Build();

            // Fluent Assertions
            Func<Task> act = async () =>
            {
                await _repository.StoreInvoice(invoice);
            };
            act.ShouldThrow<InvalidInvoiceNumberException>("because there is NO invoice number!");

            // NUnit
            Assert.ThrowsAsync<InvalidInvoiceNumberException>(() => _repository.StoreInvoice(invoice));
        }

        [Test]
        public void DoesNotThrowWhenInvoiceHasNumber()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(5).Build();

            // Fluent Assertions
            Func<Task> act = async () =>
            {
                await _repository.StoreInvoice(invoice);
            };
            act.ShouldNotThrow("because there is an invoice number");

            // NUnit
            Assert.DoesNotThrowAsync(() => _repository.StoreInvoice(invoice));
        }

        [Test]
        public async Task BeforeSavingWasFired()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(5).Build();

            var beforeSavingCalled = false;
            _repository.BeforeSaving += (sender, args) => beforeSavingCalled = true;
            await _repository.StoreInvoice(invoice);

            // Fluent Assertions
            beforeSavingCalled.Should().BeTrue("because BeforeSaving was called");

            // NUnit
            Assert.That(beforeSavingCalled, Is.True);
        }

        [Test]
        public async Task DoesNotSaveWhenCancelled()
        {
            _repository.BeforeSaving += (sender, args) => args.Cancel = true;
            var savedCalled = false;
            _repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            // Fluent Assertions
            savedCalled.Should().BeFalse("because cancelling will not save");

            // NUnit
            Assert.That(savedCalled, Is.False);
        }

        [Test]
        public async Task StoresInvoice()
        {
            var saved = await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            // Fluent Assertions
            saved.Should().BeTrue();

            // NUnit
            Assert.That(saved, Is.True);
        }

        [Test]
        public void AlsoStoresInvoice()
        {
            var saved = _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build()).Result;

            // Fluent Assertions
            saved.Should().BeTrue();

            // NUnit
            Assert.That(saved, Is.True);
        }

        [Test]
        public async Task FailingWithFluentAssertionsMessage()
        {
            var savedCalled = false;
            //_repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            // Fluent Assertions
            savedCalled.Should().BeTrue("because Saved was called");
        }

        [Test]
        public async Task FailingWithNUnitMessage()
        {
            var savedCalled = false;
            //_repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());
            
            // NUnit
            Assert.That(savedCalled, Is.True, "Saved was not called");
        }

        
    }
}