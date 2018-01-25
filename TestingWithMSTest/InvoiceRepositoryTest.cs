using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestingFrameworks;
using UnitTestingFrameworks.Builders;

namespace TestingWithMSTest
{
    [TestClass]
    public class InvoiceRepositoryTest
    {
        private InvoiceBuilder _invoiceBuilder;
        private static IDbConnection _db;
        private IInvoiceRepository _repository;

        [ClassInitialize]
        public static void ClassSetup(TestContext testContext)
        {
            
            _db = new SqliteConnection("Data Source=TestingUnitTestFrameworks;");
            _db.Open();
        }

        [TestInitialize]
        public void Setup()
        {
            _invoiceBuilder = new InvoiceBuilder();
            _repository = new InvoiceRepository(_db);
        }

        [TestMethod]
        public async Task ThrowsInvalidInvoiceNumberExceptionWhenNoInvoiceNumber()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(0).Build();

            await Assert.ThrowsExceptionAsync<InvalidInvoiceNumberException>(() => _repository.StoreInvoice(invoice));
        }

        [TestMethod]
        public async Task DoesNotThrowWhenInvoiceHasNumber()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(5).Build();

            await _repository.StoreInvoice(invoice);
        }

        [TestMethod]
        public async Task BeforeSavingWasFired()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(5).Build();

            var beforeSavingCalled = false;
            _repository.BeforeSaving += (sender, args) => beforeSavingCalled = true;
            await _repository.StoreInvoice(invoice);

            Assert.IsTrue(beforeSavingCalled);
        }

        [TestMethod]
        public async Task DoesNotSaveWhenCancelled()
        {
            _repository.BeforeSaving += (sender, args) => args.Cancel = true;
            var savedCalled = false;
            _repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.IsFalse(savedCalled);
        }

        [TestMethod]
        public async Task StoresInvoice()
        {
            var saved = await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.IsTrue(saved);
        }

        [TestMethod]
        public void AlsoStoresInvoice()
        {
            var saved = _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build()).Result;

            Assert.IsTrue(saved);
        }

        [TestMethod]
        public async Task FailingWithMSTestMessage()
        {
            var savedCalled = false;
            //_repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.IsTrue(savedCalled, "Saved was not called");
        }
    }
}