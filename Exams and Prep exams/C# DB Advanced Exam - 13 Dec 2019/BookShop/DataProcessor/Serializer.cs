using System.Collections.Generic;
using BookShop.Data.Models.Enums;
using BookShop.DataProcessor.ExportDto;

namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {

            /*Select all authors along with their books. Select their name in format first name + ' ' + last name.
             For each book select its name and price formatted to the second digit after the decimal point.
            Order the books by price in descending order.
            Finally sort all authors by book count descending and then by author full name.
NOTE: Before the orders, materialize the query (This is issue by Microsoft in InMemory database library)!!!
*/
            /* {
            "AuthorName": "Angelina Tallet",
            "Books": [
            {
            "BookName": "Allen Fissidens Moss",
            "BookPrice": "78.44"
            },*/
            var authors = context
                .Authors
                .Select(a => new
                {
                    AuthorName = a.FirstName + " " + a.LastName,
                    Books = a.AuthorsBooks
                        .OrderByDescending(b => b.Book.Price) // here because we format book price
                        .Select(ab => new
                        {
                            BookName = ab.Book.Name,
                            BookPrice = ab.Book.Price.ToString("F2")
                        })
                        .ToList()
                })
                .ToList()
                .OrderByDescending(a => a.Books.Count)
                .ThenBy(a => a.AuthorName)
                .ToList();

            var json = JsonConvert.SerializeObject(authors, Formatting.Indented);

            return json;
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            /*Export top 10 oldest books that are published before the given date and are of type science.
             For each book select its name, date (in format "d") and pages.
             Sort them by pages in descending order and then by date in descending order.
NOTE: Before the orders, materialize the query (This is issue by Microsoft in InMemory database library)!!!
*/
            var books = context
                .Books
                .Where(b => b.PublishedOn < date && b.Genre == Genre.Science)
                .ToList()
                .OrderByDescending(b => b.Pages)
                .ThenByDescending(b => b.PublishedOn) // again ordering before because of formatting
                .Select(b => new ExportOldestBooksDto()
                {
                    Name = b.Name,
                    Date = b.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                    Pages = b.Pages
                })
                .Take(10)
                .ToList();

            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            StringBuilder sb = new StringBuilder();
            XmlSerializer serializer = new XmlSerializer(typeof(List<ExportOldestBooksDto>), new XmlRootAttribute("Books"));
            using (StringWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, books, namespaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}