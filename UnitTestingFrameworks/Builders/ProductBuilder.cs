namespace UnitTestingFrameworks.Builders
{
    public class ProductBuilder
    {
        private Product _product;

        public ProductBuilder()
        {
            _product = new Product();

        }

        public ProductBuilder WithName(string name)
        {
            _product.Name = name;
            return this;
        }

        public ProductBuilder WithPrice(decimal price)
        {
            _product.Price = price;
            return this;
        }

        public Product Build()
        {
            return _product;
        }
    }
}