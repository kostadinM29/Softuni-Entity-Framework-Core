using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BookShop.Models.Enums;

namespace BookShop
{
    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main()
        {
            var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //2. Age Restriction
            //var command = Console.ReadLine();
            //Console.WriteLine(GetBooksByAgeRestriction(db, command));

            //3. Golden Books
            //Console.WriteLine(GetGoldenBooks(db));

            //4.Books by Price
            //Console.WriteLine(GetBooksByPrice(db));

            //5.Not Released In
            //var command = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db,command));

            //6.Book Titles by Category
            //var command = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db,command));

            //7.Released Before Date
            //var command = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db, command));

            //8. Author Search
            //var command = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, command));

            //9.Book Search
            //var command = Console.ReadLine();
            //Console.WriteLine(GetBookTitlesContaining(db, command));

            //10. Book Search by Author
            var command = Console.ReadLine();
            Console.WriteLine(GetBooksByAuthor(db, command));

            //11.Count Books
            //var command = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(db, command));

            //12.Total Book Copies
            //Console.WriteLine(CountCopiesByAuthor(db));

            //13. Profit by Category
            //Console.WriteLine(GetTotalProfitByCategory(db));

            //14. Most Recent Books
            //Console.WriteLine(GetMostRecentBooks(db));

            //15. Increase Prices
            //IncreasePrices(db);

            //16. Remove Books
            //Console.WriteLine(RemoveBooks(db));
        }
        //16. Remove Books
        public static int RemoveBooks(BookShopContext context)
        {
            /*Remove all books, which have less than 4200 copies.
             Return an int - the number of books that were deleted from the database.*/

            var books = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToList();

            var bookCategories = context
                .BooksCategories
                .Where(bc => bc.Book.Copies < 4200)
                .ToList();

            context.BooksCategories.RemoveRange(bookCategories); // alternative to foreaching 

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return books.Count();
        }
        //15. Increase Prices
        public static void IncreasePrices(BookShopContext context)
        {
            /*Increase the prices of all books released before 2010 by 5.*/

            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges(); // idk
        }
        //14. Most Recent Books
        public static string GetMostRecentBooks(BookShopContext context)
        {
            /*Get the most recent books by categories.
             The categories should be ordered by name alphabetically.
             Only take the top 3 most recent books from each category - ordered by release date (descending).
             Select and print the category name, and for each book – its title and release year.*/

            var categories = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    Books = c.CategoryBooks.Select(cb => new
                    {
                        Title = cb.Book.Title,
                        ReleaseDate = cb.Book.ReleaseDate // didn't do year convertion here because it maybe can mess up books with the same year
                    })
                        .OrderByDescending(cb => cb.ReleaseDate)
                        .Take(3)
                        .ToList()
                })
                .OrderBy(c => c.Category)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.Category}");

                foreach (var book in c.Books)
                {
                    sb.AppendLine($"{book.Title} " + $"({book.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }
        //13. Profit by Category
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            /*Return the total profit of all books by category.
             Profit for a book can be calculated by multiplying its number of copies by the price per single book.
             Order the results by descending by total profit for category and ascending by category name.*/
            var categories = context
                .Categories
                .Select(c => new
                {
                    Category = c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Category)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var c in categories)
            {
                sb.AppendLine($"{c.Category} ${c.TotalProfit:F2}");
            }

            return sb.ToString().TrimEnd();
        }
        //12. Total Book Copies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            /*Return the total number of book copies for each author.
             Order the results descending by total book copies.
             Return all results in a single string, each on a new line. */

            var authors = context
                .Authors
                .Select(a => new
                {
                    AuthorFullName = a.FirstName + ' ' + a.LastName,
                    BookCount = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.BookCount)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.AuthorFullName} - {a.BookCount}");
            }

            return sb.ToString().TrimEnd();
        }
        //11.Count Books
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            /*Return the number of books, which have a title longer than the number given as an input.*/

            var books = context
                .Books
                .Count(b => b.Title.Length > lengthCheck);

            return books;
        }
        //10. Book Search by Author
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            /*Return all titles of books and their authors’ names for books, which are written by authors whose last names start with the given string.
             Return a single string with each title on a new row.
             Ignore casing.Order by book id ascending.*/

            var books2 = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .ToList();

            var books = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId) // ordering now to not lose the bookId after select
                .Select(b => new
                {
                    Title = b.Title,
                    AuthorFullName = b.Author.FirstName + ' ' + b.Author.LastName
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} ({b.AuthorFullName})");
            }

            return sb.ToString().TrimEnd();
        }
        //9. Book Search
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            /*Return the titles of book, which contain a given string. Ignore casing.
             Return all titles in a single string, each on a new row, ordered alphabetically. */

            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b}");
            }

            return sb.ToString().TrimEnd();

        }
        //8. Author Search
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            /*Return the full names of authors, whose first name ends with a given string.
            Return all names in a single string, each on a new row, ordered alphabetically.*/

            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    FullName = a.FirstName + ' ' + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in authors)
            {
                sb.AppendLine($"{a.FullName}");
            }

            return sb.ToString().TrimEnd();

        }
        //7. Released Before Date
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            /*Return the title, edition type and price of all books that are released before a given date.
            The date will be a string in format dd-MM-yyyy.
            Return all of the rows in a single string, ordered by release date descending. */

            var books = context
                .Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .OrderByDescending(b =>
                    b.ReleaseDate) // ordering here because after select we need to project the date again
                .Select(b => new
                {
                    Title = b.Title,
                    EditionType = b.EditionType.ToString(), // not sure if needed?
                    Price = b.Price
                })
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();

        }
        //6. Book Titles by Category
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            /*Return in a single string the titles of books by a given list of categories.
            The list of categories will be given in a single line separated with one or more spaces.
            Ignore casing.
            Order by title alphabetically.*/

            var categoriesList = input
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()) // ignore casing
                .ToList();

            var books = context
                .Books
                .Where(b => b.BookCategories.Any(bc =>
                    categoriesList.Contains(bc.Category.Name.ToLower()))) // ignore casing
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b}");
            }

            return sb.ToString().TrimEnd();
        }
        //5. Not Released In
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            /*Return in a single string all titles of books that are NOT released on a given year.
             Order them by book id ascending.*/

            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title
                })
                .OrderBy(b => b.Id)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title}");
            }

            return sb.ToString().TrimEnd();
        }
        //4. Books by Price
        public static string GetBooksByPrice(BookShopContext context)
        {
            /*Return in a single string all titles and prices of books with price higher than 40, each on a new row in the format given below.
             Order them by price descending.*/
            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(b => new
                {
                    Title = b.Title,
                    Price = b.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:F2}");
            }

            return sb.ToString().TrimEnd();

        }
        //3. Golden Books
        public static string GetGoldenBooks(BookShopContext context)
        {
            /*Return in a single string titles of the golden edition books that have less than 5000 copies, each on a new line.
             Order them by book id ascending.*/

            var books = context
                .Books
                .Where(b => b.EditionType == Enum.Parse<EditionType>("Gold", true) && b.Copies < 5000)
                .Select(b => new
                {
                    Id = b.BookId,
                    Title = b.Title
                })
                .OrderBy(b => b.Id)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title}");
            }

            return sb.ToString().TrimEnd();

        }
        //2. Age Restriction
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            /*Return in a single string all book titles, each on a new line, that have age restriction, equal to the given command.
             Order the titles alphabetically.
             Read input from the console in your main method, and call your method with the necessary arguments.
             Print the returned string to the console.
             Ignore casing of the input.*/

            var books = context
                .Books
                .Where(b => b.AgeRestriction == Enum.Parse<AgeRestriction>(command, true)) // true to ignore case
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var b in books)
            {
                sb.AppendLine($"{b}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
