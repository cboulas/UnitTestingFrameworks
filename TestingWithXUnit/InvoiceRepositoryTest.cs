using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using UnitTestingFrameworks;
using UnitTestingFrameworks.Builders;
using Xunit;

namespace TestingWithXUnit
{
    public class DatabaseFixture
    {
        public DatabaseFixture()
        {
            DbConnection = new SqliteConnection("Data Source=TestingUnitTestFrameworks;");
            DbConnection.Open();
        }

        public IDbConnection DbConnection { get; set; }
    }

    public class InvoiceRepositoryTest : IClassFixture<DatabaseFixture>
    {
        private readonly InvoiceBuilder _invoiceBuilder;
        private DatabaseFixture _databaseFixture;
        private readonly IInvoiceRepository _repository;
        
        public InvoiceRepositoryTest(DatabaseFixture databaseFixture)
        {
            _databaseFixture = databaseFixture;
            _invoiceBuilder = new InvoiceBuilder();
            _repository = new InvoiceRepository(_databaseFixture.DbConnection);
        }

        [Fact]
        public Task ThrowsInvalidInvoiceNumberExceptionWhenNoInvoiceNumber() =>
            Assert.ThrowsAsync<InvalidInvoiceNumberException>(
                () => _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(0).Build()));

        [Fact]
        public async Task DoesNotThrowWhenInvoiceHasNumber()
        {
            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());
        }
            
        [Fact]
        public async Task BeforeSavingWasFired()
        {
            var invoice = _invoiceBuilder.WithInvoiceNumber(5).Build();

            await Assert.RaisesAsync<InvoiceBeforeSavingEventArgs>(h => _repository.BeforeSaving += h,
                                                        h => _repository.BeforeSaving -= h,
                                                        () => _repository.StoreInvoice(invoice));
        }

        [Fact]
        public async Task DoesNotSaveWhenCancelled()
        {
            _repository.BeforeSaving += (sender, args) => args.Cancel = true;
            var savedCalled = false;
            _repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.False(savedCalled);
        }

        [Fact]
        public async Task StoresInvoice()
        {
            var saved = await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.True(saved);
        }

        [Fact]
        public void AlsoStoresInvoice()
        {
            var saved = _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build()).Result;

            Assert.True(saved);
        }

        [Fact]
        public async Task FailingWithXUnitMessage()
        {
            var savedCalled = false;
            //_repository.Saved += (sender, args) => savedCalled = true;

            await _repository.StoreInvoice(_invoiceBuilder.WithInvoiceNumber(5).Build());

            Assert.True(savedCalled, "Save was not called");
        }
    }
}