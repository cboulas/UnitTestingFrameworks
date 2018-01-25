using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTestingFrameworks
{
    public class Invoice
    {
        public int InvoiceNumber { get; set; }
        public string ClientName { get; set; }
        public DateTime DueDate { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();
        public decimal InvoiceTotal => Products.Sum(x => x.Price) * ((DateTime.Now - DueDate).TotalDays > 30 ? 1.5m : 1);
    }
}
