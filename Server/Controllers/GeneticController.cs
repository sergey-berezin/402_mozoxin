using GeneticPackaging;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server.Controllers
{
    [ApiController]
    [Route("")]
    public class GeneticController : ControllerBase
    {
        [HttpGet("initial")]
        public async Task<string> Init(int childrenSize, int mutationSize, int populationSize, int figuresAmount, [FromQuery(Name = "figuresSizes")] int[] figuresSizes)
        {
            var computation = await Logic.StartCalculation(childrenSize, mutationSize, populationSize, figuresAmount, figuresSizes);
            return JsonConvert.SerializeObject(computation, new JsonSerializerSettings
            {
                //ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                //PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented
            });
        }

        [HttpPost("next")]
        public async Task<ActionResult<string>> Iterate(PostBody body)
        {
            var computation = await Logic.Iterate(
                body.iterationsCompleted,
                body.childrenSize,
                body.mutationSize,
                body.populationSize,
                body.figuresAmount,
                body.figuresSizes,
                body.population
            );
           
            return JsonConvert.SerializeObject(computation, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }
    }
}
