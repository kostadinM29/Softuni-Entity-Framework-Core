using BookShop.Data.Models;
using BookShop.Data.Models.Enums;
using BookShop.DataProcessor.ImportDto;

namespace BookShop.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedBook
            = "Successfully imported book {0} for {1:F2}.";

        private const string SuccessfullyImportedAuthor
            = "Successfully imported author - {0} with {1} books.";

        public static string ImportBooks(BookShopContext context, string xmlString)
        {
            /*•	If there are any validation errors for the book entity
             (such as invalid name, genre, price, pages or published date),
            do not import any part of the entity and append an error message to the method output.*/

            StringBuilder sb = new StringBuilder();

            XmlSerializer serializer = new XmlSerializer(typeof(List<ImportBooksDto>), new XmlRootAttribute("Books"));

            var books = new List<Book>();

            using (StringReader reader = new StringReader(xmlString))
            {
                var bookDtos = (List<ImportBooksDto>)serializer.Deserialize(reader);

                foreach (var bookDto in bookDtos)
                {
                    if (!IsValid(bookDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isValidGenre = Enum.TryParse(bookDto.Genre, out Genre genre);
                    if (!isValidGenre)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    bool isPublishedOnDateValid = DateTime.TryParseExact(bookDto.PublishedOn, "MM/dd/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime publishedOn);
                    if (!isPublishedOnDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Book book = new Book()
                    {
                        Name = bookDto.Name,
                        Genre = genre,
                        Price = bookDto.Price,
                        Pages = bookDto.Pages,
                        PublishedOn = publishedOn
                    };

                    books.Add(book);

                    sb.AppendLine(string.Format(SuccessfullyImportedBook, book.Name, book.Price));
                }
            }
            context.Books.AddRange(books);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportAuthors(BookShopContext context, string jsonString)
        {
            /*•	If any validation errors occur (such as invalid first name, last name, email or phone), do not import any part of the entity and append an error message to the method output.
•	If an email exists, do not import the author and append and error message.
•	If a book does not exist in the database, do not append an error message and continue with the next book.
•	If an author have zero books (all books are invalid) do not import the author and append an error message to the method output.
*/

            StringBuilder sb = new StringBuilder();

            var authorDtos = JsonConvert.DeserializeObject<List<ImportAuthorsDto>>(jsonString);

            var authors = new List<Author>();

            foreach (var authorDto in authorDtos)
            {
                if (!IsValid(authorDto)) // If any validation errors occur (such as invalid first name, last name, email or phone), do not import any part of the entity and append an error message to the method output.
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (authors.Any(a => a.Email == authorDto.Email)) // If an email exists, do not import the author and append and error message.
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Author author = new Author
                {
                    FirstName = authorDto.FirstName,
                    LastName = authorDto.LastName,
                    Phone = authorDto.Phone,
                    Email = authorDto.Email
                };

                foreach (var bookDto in authorDto.Books)
                {
                    if (!bookDto.BookId.HasValue) //check if has value
                    {
                        continue;
                    }

                    var book = context.Books.FirstOrDefault(b => b.Id == bookDto.BookId.Value);

                    if (book == null) // If a book does not exist in the database, do not append an error message and continue with the next book.
                    {
                        continue;
                    }

                    AuthorBook bookToAdd = new AuthorBook()
                    {
                        BookId = bookDto.BookId.Value
                    };

                    author.AuthorsBooks.Add(bookToAdd);
                }

                if (author.AuthorsBooks.Count == 0) // If an author have zero books (all books are invalid) do not import the author and append an error message to the method output.
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                authors.Add(author);
                sb.AppendLine(string.Format(SuccessfullyImportedAuthor, author.FirstName + " " + author.LastName,
                    author.AuthorsBooks.Count));
            }

            context.Authors.AddRange(authors);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}