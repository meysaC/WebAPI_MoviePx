using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace api.Service
{
    public class VectorDbService  //pgvector için   
                                 // -> PgVector, PostgreSQL için bir vector (vektör) veri tipi ve extension’ıdır. 
                                // OpenAI veya benzeri embedding modellerinden aldığın float array (örn. [0.12, 0.34, ...]) verilerini direkt olarak PostgreSQL içinde saklamana ve cosine similarity, Euclidean distance gibi işlemleri SQL sorgusu ile yapmana olanak sağlar. 
                               // PgVector olmadan embeddingleri SQL içinde direkt sorgulamak mümkün değil, Dapper ile manuel hesap yapman gerekir ki çok verimsiz olur.
    {
        private readonly string _connectionString;
        public VectorDbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("PgVectorDb");
        }

        // Embedding + meta yı db kaydet
        public async Task InsertMovieEmbeddingAsync(int movieId, string title, string overview, float[] embedding) //simalarity için embedding
        {
            using var connection = new NpgsqlConnection(_connectionString);
            var sql = @"
                INSERT INTO movie_embeddings (movie_id, title, overview, embedding)
                VALUES (@MovieId, @Title, @Overview, @Embedding)";
            await connection.ExecuteAsync(sql, new
            {
                MovieId = movieId,
                Title = title,
                Overview = overview,
                Embedding = embedding
            });
        }

        // Cosine similarity ile en benzer N film
        public async Task<IEnumerable<MovieEmbeddingResult>> GetSimilarMoviesAsync(float[] embedding, int topN = 10)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            
            //C# tarafında float[] olarak yolluyorsun → Pg tarafında bu real[]
            //pgvector uzantısının operatorleri sadece vector <=> vector şeklinde çalışır.
            //::vector ile type-cast yapmış oluyoruz → iki taraf da aynı tipe dönüşüyor.
            var sql = @"
                SELECT movie_id, title, overview,
                       embedding <=> @Embedding::vector AS distance
                FROM movie_embeddings
                ORDER BY distance ASC
                LIMIT  @topN";

            return await connection.QueryAsync<MovieEmbeddingResult>(sql, new { embedding, topN });
        }

        public record MovieEmbeddingResult(int movie_id, string title, string overview, double distance);

    }
}