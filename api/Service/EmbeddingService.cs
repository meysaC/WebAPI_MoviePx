using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace api.Service
{
    public class EmbeddingService : Interfaces.IEmbeddingService  //Batelgo.OpenAI
    {
        private readonly IOpenAIService _openAiService; 
        public EmbeddingService(IOpenAIService openAiService)
        {
            _openAiService = openAiService;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)  //Batelgo.OpenAI KULLANIYORUZ!!!!
        { 
            try
            {
                var request = new EmbeddingCreateRequest
                {
                    Input = text,
                    Model = "text-embedding-3-large" //Bu model sadece embedding üretmek için --> Yani bir metni sayısal bir vektöre (örneğin float[]) dönüştürür --> bu vektörleri de Pgvector tablosunda saklıyoruz ve embedding <=> @Embedding::vector şeklinde benzerlik karşılaştırması yapmak için kullanıyorsun
                };

                var response = await _openAiService.Embeddings.CreateEmbedding(request);

                if (response?.Data == null || !response.Data.Any())
                    throw new ApplicationException("Embedding API failed or returned empty data. Check quota and API key.");

                // Betalgo package dönüşü: List<double>
                // float[]’a dönüştürerek geri döndürüyoruz
                return response.Data[0].Embedding.Select(x => (float)x).ToArray();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Embedding işlemi sırasında hata oluştu: " + ex.Message, ex);
            }
        }
    }
}
