using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using VaporStore.Data.Models;
using VaporStore.Data.Models.Enums;
using VaporStore.DataProcessor.Dto.Import;

namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Data;

    public static class Deserializer
    {
        private const string ErrorMessage = "Invalid Data";
        private const string SuccessfullyAddedGame = "Added {0} ({1}) with {2} tags";
        private const string SuccessfullyAddedUser = "Imported {0} with {1} cards";
        private const string SuccessfullyAddedPurchase = "Imported {0} for {1}";
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            /*•	If any validation errors occur (such as if a Price is negative, a Name/ReleaseDate/Developer/Genre is missing,
			 Tags are missing or empty), do not import any part of the entity and append an error message to the method output.
•	Dates are always in the format “yyyy-MM-dd”. Do not forget to use CultureInfo.InvariantCulture!
•	If a developer/genre/tag with that name doesn’t exist, create it. 
•	If a game is invalid, do not import its genre, developer or tags.
*/
            StringBuilder sb = new StringBuilder();

            var gameDtos = JsonConvert.DeserializeObject<List<ImportGamesDto>>(jsonString);

            var games = new List<Game>();

            var developers = new List<Developer>();
            var genres = new List<Genre>();
            var tags = new List<Tag>();

            foreach (var gameDto in gameDtos)
            {
                if (!IsValid(gameDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var isReleaseDateValid = DateTime.TryParseExact(gameDto.ReleaseDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var releaseDate);
                if (!isReleaseDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var gameToAdd = new Game()
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = releaseDate
                };

                var developer = developers.FirstOrDefault(d => d.Name == gameDto.Developer) ??
                                new Developer()
                                {
                                    Name = gameDto.Developer
                                };

                developers.Add(developer);

                gameToAdd.Developer = developer;

                var genre = genres.FirstOrDefault(g => g.Name == gameDto.Genre) ??
                            new Genre()
                            {
                                Name = gameDto.Genre
                            };

                genres.Add(genre);


                gameToAdd.Genre = genre;

                foreach (var tag in gameDto.Tags)
                {

                    var tagToAdd = tags.FirstOrDefault(t => t.Name == tag) ??
                                   new Tag()
                                   {
                                       Name = tag
                                   };

                    tags.Add(tagToAdd);

                    gameToAdd.GameTags.Add(new GameTag()
                    {
                        Game = gameToAdd,
                        Tag = tagToAdd
                    });
                }

                if (!gameToAdd.GameTags.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                games.Add(gameToAdd);

                sb.AppendLine(string.Format(SuccessfullyAddedGame, gameToAdd.Name, gameToAdd.Genre.Name, gameToAdd.GameTags.Count));
            }

            context.Games.AddRange(games);

            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            /*•	If any validation errors occur (such as invalid full name, too short/long username, missing email, too low/high age, incorrect card number/CVC, no cards, etc.)
             , do not import any part of the entity and append an error message to the method output.
•	If any validation errors occur with card entity (such as invalid number/CVC, invalid Type) you should not import any part of the User entity holding this card and append an error message to the method output. 
*/
            StringBuilder sb = new StringBuilder();

            var userDtos = JsonConvert.DeserializeObject<List<ImportUsersDto>>(jsonString);

            var usersToAdd = new List<User>();

            foreach (var userDto in userDtos)
            {
                if (!IsValid(userDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var user = new User()
                {
                    Username = userDto.Username,
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                foreach (var cardDto in userDto.Cards)
                {
                    if (!IsValid(cardDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    var isValidCardType = Enum.TryParse(cardDto.Type, out CardType type);
                    if (!isValidCardType)
                    {
                        sb.AppendLine(ErrorMessage);
                        break;
                    }

                    var card = new Card()
                    {
                        Cvc = cardDto.Cvc,
                        Number = cardDto.Number,
                        Type = type,
                        User = user
                    };
                    user.Cards.Add(card);
                }
                usersToAdd.Add(user);

                sb.AppendLine(string.Format(SuccessfullyAddedUser, user.Username, user.Cards.Count));
            }
            context.Users.AddRange(usersToAdd);

            context.SaveChanges();

            return sb.ToString().TrimEnd();

        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            /*
            •	If there are any validation errors, do not import any part of the entity and append an error message to the method output.
•	Dates will always be in the format: “dd/MM/yyyy HH:mm”. Do not forget to use CultureInfo.InvariantCulture!
*/
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ImportPurchasesDto>), new XmlRootAttribute("Purchases"));

            var purchaseToAdd = new List<Purchase>();

            using (StringReader reader = new StringReader(xmlString))
            {
                var purchaseDtos = (List<ImportPurchasesDto>)serializer.Deserialize(reader);

                foreach (var purchaseDto in purchaseDtos)
                {
                    if (!IsValid(purchaseDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }


                    var isDateValid = DateTime.TryParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out var date);

                    if (!isDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var isValidType = Enum.TryParse(purchaseDto.PurchaseType, out PurchaseType type);
                    if (!isValidType)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var card = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.CardNumber);
                    if (card == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var game = context.Games.FirstOrDefault(g => g.Name == purchaseDto.Game);
                    if (game == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Purchase purchase = new Purchase()
                    {
                        Card = card,
                        Date = date,
                        Game = game,
                        ProductKey = purchaseDto.ProductKey,
                        Type = type
                    };

                    purchaseToAdd.Add(purchase);

                    sb.AppendLine(string.Format(SuccessfullyAddedPurchase, purchase.Game.Name, purchase.Card.User.Username));
                }
                context.Purchases.AddRange(purchaseToAdd);

                context.SaveChanges();

                return sb.ToString().TrimEnd();
            }
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}