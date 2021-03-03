using System.Globalization;
using System.Linq;
using System.Text;
using MusicHub.Data.Models;

namespace MusicHub
{
    using System;

    using Data;
    using Initializer;

    public class StartUp
    {
        public static void Main(string[] args)
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            var albums = ExportAlbumsInfo(context, 9);
            Console.WriteLine(albums);

            var songs = ExportSongsAboveDuration(context, 4);
            Console.WriteLine(songs);
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            /*You need to write method string ExportAlbumsInfo(MusicHubDbContext context, int producerId) in the StartUp class that receives a Producer Id.
             Export all albums which are produced by the provided Producer Id.
             For each Album, get the Name, Release date in format "MM/dd/yyyy", Producer Name, the Album Songs with each Song Name, Price (formatted to the second digit) and the Song Writer Name.
             Sort the Songs by Song Name (descending) and by Writer (ascending).
             At the end export the Total Album Price with exactly two digits after the decimal place.
             Sort the Albums by their Total Price (descending).*/
            var albums = context
                .Albums
                .ToList()// need to load producer or else its null
                .Where(a => a.Producer.Id == producerId)
                .Select(a => new
                {
                    AlbumName = a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                    ProducerName = a.Producer.Name,
                    Songs = a.Songs.Select(s => new
                    {
                        SongName = s.Name,
                        Price = s.Price,
                        Writer = s.Writer.Name
                    })
                        .OrderByDescending(s => s.SongName)
                        .ThenBy(s => s.Writer)
                        .ToList(),
                    AlbumPrice = a.Price // total album price?
                })
                .OrderByDescending(a => a.AlbumPrice)
                .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var a in albums)
            {
                sb.AppendLine($"-AlbumName: {a.AlbumName}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine("-Songs:");

                int counter = 1;

                foreach (var s in a.Songs)
                {
                    sb.AppendLine($"---#{counter}");
                    sb.AppendLine($"---SongName: {s.SongName}");
                    sb.AppendLine($"---Price: {s.Price:F2}");
                    sb.AppendLine($"---Writer: {s.Writer}");

                    counter++;
                }

                sb.AppendLine($"-AlbumPrice: {a.AlbumPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            /*You need to write method string ExportSongsAboveDuration(MusicHubDbContext context, int duration) in the StartUp class that receives Song duration (integer, in seconds).
             Export the songs which are above the given duration. For each Song, export its Name, Performer Full Name, Writer Name, Album Producer and Duration (in format("c")).
             Sort the Songs by their Name (ascending), by Writer (ascending) and by Performer (ascending).*/

            var songs = context
                .Songs
                .ToList() // need to load duration or else it doesn't work
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    SongName = s.Name,
                    PerformerName = s.SongPerformers
                        .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                        .FirstOrDefault(),
                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                })
                .OrderBy(s => s.SongName)
                .ThenBy(s => s.WriterName)
                .ThenBy(s => s.PerformerName)
                .ToList();

            StringBuilder sb = new StringBuilder();

            int counter = 1;
            foreach (var s in songs)
            {

                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {s.SongName}");
                sb.AppendLine($"---Writer: {s.WriterName}");
                sb.AppendLine($"---Performer: {s.PerformerName}");
                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");

                counter++;
            }

            return sb.ToString().TrimEnd();
        }
    }
}
