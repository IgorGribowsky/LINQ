using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task1.DoNotChange;

namespace Task1
{
    public static class LinqTask
    {
        public static IEnumerable<Customer> Linq1(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(customer => customer.Orders.Sum(order => order.Total) > limit);
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            return customers.Select(customer => (customer,
            suppliers.Where(supplier => supplier.Country == customer.Country && supplier.City == customer.City)));
        }

        public static IEnumerable<(Customer customer, IEnumerable<Supplier> suppliers)> Linq2UsingGroup(
            IEnumerable<Customer> customers,
            IEnumerable<Supplier> suppliers
        )
        {
            var suppliersGroupedByCountryAndCity = suppliers.GroupBy(supplier => ( supplier.Country, supplier.City ));

            return customers.Select(customer => (customer,
            GetSuppliersFromGroups(suppliersGroupedByCountryAndCity, customer)
            )) ;
        }

        private static IEnumerable<Supplier> GetSuppliersFromGroups(IEnumerable<IGrouping<(string, string),Supplier>> supplierGroups, Customer customer)
        {
            var suppliers = supplierGroups.FirstOrDefault(suppliersGroup => suppliersGroup.Key.Item1 == customer.Country && suppliersGroup.Key.Item2 == customer.City);
            return suppliers != null 
                ? suppliers.AsEnumerable()
                : new List<Supplier>();
        }

        public static IEnumerable<Customer> Linq3(IEnumerable<Customer> customers, decimal limit)
        {
            return customers.Where(customer => customer.Orders.Sum(order => order.Total) > limit);
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq4(
            IEnumerable<Customer> customers
        )
        {
            return customers
                .Where(customer => customer.Orders.Any())
                .Select(customer =>
                (customer, customer.Orders.OrderBy(order => order.OrderDate).First().OrderDate));
        }

        public static IEnumerable<(Customer customer, DateTime dateOfEntry)> Linq5(
            IEnumerable<Customer> customers
        )
        {
            var customersWithDateOfFirstOrder = Linq4(customers);
            return customersWithDateOfFirstOrder
                .OrderBy(customer => customer.dateOfEntry.Year)
                .ThenBy(customer => customer.dateOfEntry.Month)
                .ThenByDescending(customer => customer.customer.Orders.Sum(order => order.Total))
                .ThenBy(customer => customer.customer.CompanyName);
        }

        public static IEnumerable<Customer> Linq6(IEnumerable<Customer> customers)
        {
            return customers.Where(
                customer => !customer.PostalCode.All(char.IsDigit)
                || customer.Region == null
                || !customer.Phone.Any(ch => ch.Equals('(') || ch.Equals(')')));
        }

        public static IEnumerable<Linq7CategoryGroup> Linq7(IEnumerable<Product> products)
        {
            return products.GroupBy(product => product.Category)
                .Select(categoryGroup => new Linq7CategoryGroup()
                {
                    Category = categoryGroup.Key,
                    UnitsInStockGroup = categoryGroup
                        .GroupBy(product => product.UnitsInStock)
                        .Select(unitGroup => new Linq7UnitsInStockGroup()
                        {
                            UnitsInStock = unitGroup.Key,
                            Prices = unitGroup.Select(product => product.UnitPrice),
                        })
                });
        }

        public static IEnumerable<(decimal category, IEnumerable<Product> products)> Linq8(
            IEnumerable<Product> products,
            decimal cheap,
            decimal middle,
            decimal expensive
        )
        {
            var categories = new List<Tuple<decimal, decimal>>() 
            {
                new Tuple<decimal, decimal>(0, cheap),
                new Tuple<decimal, decimal>(cheap, middle),
                new Tuple<decimal, decimal>(middle, expensive),
            };

            return categories.Select(category => (category.Item2,
                products.Where(product => product.UnitPrice > category.Item1 && product.UnitPrice <= category.Item2)));
        }

        public static IEnumerable<(string city, int averageIncome, int averageIntensity)> Linq9(
            IEnumerable<Customer> customers
        )
        {
            var customersGroupedByCity = customers.GroupBy(customer => customer.City);
            return customersGroupedByCity.Select(customersPerCity => (
                customersPerCity.Key,
                Convert.ToInt32(customersPerCity.Average(customer => customer.Orders.Sum(order => order.Total))),
                Convert.ToInt32(customersPerCity.Average(customer => customer.Orders.Count()))));
        }

        public static string Linq10(IEnumerable<Supplier> suppliers)
        {
            var countryNames = suppliers.GroupBy(supplier => supplier.Country).Select(group => group.Key);

            var sortedCountryNames = countryNames.OrderBy(name => name.Length).ThenBy(name => name).ToList();

            var stringBuilder = new StringBuilder();
            sortedCountryNames.ForEach(name => stringBuilder.Append(name));

            return stringBuilder.ToString();
        }
    }
}