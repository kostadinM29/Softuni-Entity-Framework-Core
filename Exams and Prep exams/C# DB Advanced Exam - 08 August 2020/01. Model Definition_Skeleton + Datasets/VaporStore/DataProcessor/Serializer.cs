using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using VaporStore.Data.Models.Enums;
using VaporStore.DataProcessor.Dto.Export;

namespace VaporStore.DataProcessor
{
    using System;
    using Data;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            /*
			  The given method in the project skeleton receives an array of genre names. Export all games in those genres, which have any purchases.
			  For each genre, export its id, genre name, games and total players (total purchase count).
			  For each game, export its id, name, developer, tags (separated by ", ") and total player count (purchase count).
			  Order the games by player count (descending), then by game id (ascending).
			  Order the genres by total player count (descending), then by genre id (ascending)
			 */

            /*{
            "Id": 4,
            "Genre": "Violent",
            "Games": [
            {
            "Id": 49,
            "Title": "Warframe",
            "Developer": "Digital Extremes",
            "Tags": "Single-player, In-App Purchases, Steam Trading Cards, Co-op, Multi-player, Partial Controller Support",
            "Players": 6
            },
            */
            var genres = context.Genres
                .ToList() // inmemory error ?
                .Where(g => genreNames.Contains(g.Name))
                .Select(g => new
                {
                    Id = g.Id,
                    Genre = g.Name,
                    Games = g.Games.Where(ga => ga.Purchases.Any()) //which have any purchases.
                        .Select(ga => new
                        {
                            Id = ga.Id,
                            Title = ga.Name,
                            Developer = ga.Developer.Name,
                            Tags = string.Join(", ", ga.GameTags.Select(gt => gt.Tag.Name)
                                .ToList()),
                            Players = ga.Purchases.Count
                        })
                        .OrderByDescending(ga => ga.Players)
                        .ThenBy(ga => ga.Id)
                        .ToList(),
                    TotalPlayers = g.Games.Sum(ga => ga.Purchases.Count)
                })
                .OrderByDescending(g => g.TotalPlayers)
                .ThenBy(g => g.Id)
                .ToList();

            var jsonResult = JsonConvert.SerializeObject(genres, Formatting.Indented);

            return jsonResult;
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
        {
            /*Use the method provided in the project skeleton, which receives a purchase type as a string.
             Export all users who have any purchases.
            For each user, export their username, purchases for that purchase type and total money spent for that purchase type.
            For each purchase, export its card number, CVC, date in the format "yyyy-MM-dd HH:mm" (make sure you use CultureInfo.InvariantCulture) and the game.
            For each game, export its title (name), genre and price.
            Order the users by total spent (descending), then by username (ascending).
            For each user, order the purchases by date (ascending).
            Do not export users, who don’t have any purchases.
            Note: All prices must be in decimal without any formatting!
*/
            var purchaseTypeEnum = Enum.Parse<PurchaseType>(storeType);

            var userDtos = context.Users
                .ToList() // just in case
                .Where(u => u.Cards.Any(c => c.Purchases.Any())) //Do not export users, who don’t have any purchases.
                .Select(u => new ExportUserDto()
                {
                    Username = u.Username,
                    Purchases = context.Purchases
                        .ToList() // just in case
                        .Where(p => p.Card.User.Username == u.Username && p.Type == purchaseTypeEnum)
                        .OrderBy(p => p.Date)
                        .Select(p => new ExportPurchaseDto()
                        {
                            CardNumber = p.Card.Number,
                            Cvc = p.Card.Cvc,
                            Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                            Game = new ExportGameDto()
                            {
                                Name = p.Game.Name,
                                GenreName = p.Game.Genre.Name,
                                Price = p.Game.Price
                            }
                        })
                        .ToList(),
                    TotalSpent = context.Purchases
                        .ToList() // just in case
                        .Where(p => p.Card.User.Username == u.Username && p.Type == purchaseTypeEnum)
                        .Sum(p => p.Game.Price)
                })
                .Where(u => u.Purchases.Any()) //Do not export users, who don’t have any purchases.
                .OrderByDescending(u => u.TotalSpent)
                .ThenBy(u => u.Username)
                .ToList();

            StringBuilder sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var serializer = new XmlSerializer(typeof(List<ExportUserDto>), new XmlRootAttribute("Users"));

            using (var writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, userDtos, namespaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}