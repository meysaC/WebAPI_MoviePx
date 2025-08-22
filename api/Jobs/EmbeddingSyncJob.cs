using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Interfaces;
using api.Service;

namespace api.Jobs
{
    public class EmbeddingSyncJob // Bu job, TMDb'den popüler filmleri alır, embedding oluşturur ve pgvector'a kaydeder
    {
        private readonly ITmdbService _tmdbService;
        private readonly IEmbeddingService _embeddingService;
        private readonly VectorDbService _vectorDbService;
        public EmbeddingSyncJob(ITmdbService tmdbService, IEmbeddingService embeddingService, VectorDbService vectorDbService)
        {
            _tmdbService = tmdbService;
            _embeddingService = embeddingService;
            _vectorDbService = vectorDbService;
        }

        public async Task RunAsync(int totaltPages = 5) // 5*
        {
            Console.WriteLine("EmbeddingSyncJob started...");
            for (int page = 1; page <= totaltPages; page++)
            {
                Console.WriteLine($"Fetching TMDb popular movies page {page}...");
                var movies = await _tmdbService.GetPopularMoviesForEmbeddingAsync(page);
                foreach (var movie in movies)
                {
                    try
                    {
                        // Eğer overview boşsa atla
                        if (string.IsNullOrWhiteSpace(movie?.Overview) || string.IsNullOrWhiteSpace(movie?.Overview))
                        {
                            Console.WriteLine($"Skipping movie {movie?.Id} due to missing title or overview.");
                            continue;
                        }

                        // Embedding oluştur
                        var vector = await _embeddingService.GetEmbeddingAsync(movie.Overview);
                        if (vector == null || vector.Length == 0)
                        {
                            Console.WriteLine($"Skipping movie {movie.Id} due to empty embedding.");
                            continue;
                        }

                        // pgvector’a kaydet 
                        await _vectorDbService.InsertMovieEmbeddingAsync(
                            movie.Id,
                            movie.Title,
                            movie.Overview,
                            vector
                        );
                        Console.WriteLine($"Inserted embedding for movie: {movie.Title}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing movie {movie.Id}, {movie.Title}: {ex.Message}");
                        continue;
                    }
                    Console.WriteLine("Embedding sync job finished!");
                }
            }
        }
    }
}