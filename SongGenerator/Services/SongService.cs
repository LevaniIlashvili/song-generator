using Bogus;
using SongGenerator.Models;

namespace SongGenerator.Services;

public class SongService
{
    private static readonly Dictionary<string, string[]> GenreMap = new()
    {
        { "en", new[] { "Rock", "Pop", "Jazz", "Hip Hop", "Country", "Blues", "Electronic" } },
        { "es", new[] { "Rock", "Pop", "Jazz", "Hip Hop", "Country", "Blues", "Electrónica" } }
    };

    public List<Song> GenerateBatch(int page, int pageSize, string region, double likesAvg, long? seed = null)
    {
        long finalSeed = seed ?? (long)(new Random().NextDouble() * long.MaxValue);

        var results = new List<Song>();
        int startingIndex = (page - 1) * pageSize + 1;
        var faker = new Faker(region);

        for (int i = 0; i < pageSize; i++)
        {
            int currentIndex = startingIndex + i;

            int contentSeed = HashCode.Combine(finalSeed, currentIndex, "CONTENT");
            int likesSeed = HashCode.Combine(finalSeed, currentIndex, "LIKES");

            Randomizer.Seed = new Random(contentSeed);
            var localRnd = new Random(contentSeed);

            var song = new Song
            {
                Index = currentIndex,
                Artist = localRnd.Next(0, 2) == 0 ? faker.Name.FullName() : faker.Company.CompanyName(),
                Title = faker.Commerce.ProductName(),
                Album = localRnd.Next(0, 4) == 0 ? "Single" : faker.Commerce.ProductName(),
                Genre = GetRandomGenre(region, contentSeed),

                CoverUrl = $"/api/media/cover?seed={contentSeed}&title={Uri.EscapeDataString("TBD")}&artist={Uri.EscapeDataString("TBD")}",
                AudioUrl = $"/api/media/audio?seed={contentSeed}",
                Likes = CalculateLikes(likesAvg, likesSeed)
            };

            song.CoverUrl = $"/api/media/cover?seed={contentSeed}&title={Uri.EscapeDataString(song.Title)}&artist={Uri.EscapeDataString(song.Artist)}";

            results.Add(song);
        }
        return results;
    }

    private static int CalculateLikes(double avgLikes, int seed)
    {
        var rnd = new Random(seed);
        int baseLikes = (int)Math.Floor(avgLikes);
        double fraction = avgLikes - baseLikes;

        return rnd.NextDouble() < fraction ? baseLikes + 1 : baseLikes;
    }

    private static string GetRandomGenre(string region, int seed)
    {
        var rnd = new Random(seed);
        if (GenreMap.TryGetValue(region, out var genres))
        {
            return genres[rnd.Next(genres.Length)];
        }
        return GenreMap["en"][rnd.Next(GenreMap["en"].Length)];
    }
}
