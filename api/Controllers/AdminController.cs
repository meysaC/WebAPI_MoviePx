using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Jobs;
using api.Service;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/admin")]
    public class AdminController : ControllerBase
    {
        private readonly EmbeddingSyncJob _embeddingSyncJob;
        public AdminController(EmbeddingSyncJob embeddingSyncJob)
        {
            _embeddingSyncJob = embeddingSyncJob;
        }

        [HttpPost("embeddings")]
        public async Task<IActionResult> RunSync(int pages = 20) //end points e ?pages=30 göndererek 30x20=6000 film embedding oluşturulur
        {
            await _embeddingSyncJob.RunAsync();
            return Ok("Embedding sync job completed.");
        }
    }
}