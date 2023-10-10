using HotelCompareWebApi.Models;
using HotelCompareWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Reflection;
using System.Text.Json;

namespace HotelCompareWebApi.Controllers
{
    public class ReviewController : Controller
    {

        [Route("compare")]
        public async Task<IActionResult> CompareHotelReviews([FromServices] IKernel kernel, [FromQuery] string hotelNames, [FromQuery] string userPreferences)
        {
            SKContext userPreference = await ExtractUserPreferences(kernel, userPreferences);

            List<string> recallList = new List<string>();
            foreach (string hotelName in hotelNames.Split(","))
            {
                await GetHotelAnalysisAsync(kernel, hotelName.Trim());
                recallList.Add(string.Format(@"About {0}: {{{{recall 'give me full information on {1}.'}}}}", hotelName, hotelName));
            }

            // conclude -- this last SK Function is still work in progress, I am still engineering the prompt to do comparison on the 3 hotels correctly
            string hotelKnowledge = String.Join(Environment.NewLine, recallList.ToArray()); 
            SKContext conclusion = await GetConclusionAsync(kernel,
                hotelKnowledge,
                userPreference.Result);

            return this.Ok(conclusion.Result);
        }

        private async Task GetHotelAnalysisAsync(IKernel kernel, string hotelName)
        {
            FileUtils fileUtils = new FileUtils();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format(@"Data\{0}.txt", hotelName));
            var input = fileUtils.ReadFromFile(path);

            // get sentiment
            SKContext sentiment = await GetSentimentFromReviewsAsync(kernel, input);
            ReviewSentiment sentimentResult = JsonSerializer.Deserialize<ReviewSentiment>(sentiment.Result);

            // get likes and dislike
            SKContext likesDislikes = await GetLikesAndDislikesFromReviewsAsync(kernel, input);
            ReviewLikesAndDislikes likesDislikesResult = JsonSerializer.Deserialize<ReviewLikesAndDislikes>(likesDislikes.Result);

            // get summary
            SKContext summary = await GetSummaryFromReviewsAsync(kernel, input);
            ReviewSummary summaryResult = JsonSerializer.Deserialize<ReviewSummary>(summary.Result);

            string knowledgeToStore = GetKnowledgeToStoreInMemory(hotelName, sentimentResult, likesDislikesResult, summaryResult);
            await kernel.Memory.SaveInformationAsync("HotelCompare", id: hotelName, text: knowledgeToStore);

        }

        private string GetKnowledgeToStoreInMemory(string hotelName, ReviewSentiment sentiment, ReviewLikesAndDislikes likesDislikes, ReviewSummary summary)
        {
            return string.Format("About {0}: The review sentiment is {1} because {2} The guests like these features: {3}. But they don't like these features: {4}. In summary, {5}",
                hotelName, sentiment.Sentiment, sentiment.Reason, string.Join(", ", likesDislikes.Likes), string.Join(", ", likesDislikes.Dislikes), summary.Summary);
        }

        private async Task<SKContext> ExtractUserPreferences(IKernel kernel, string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("input", input);

            ISKFunction function = kernel.Skills.GetFunction("ExtractUserPreference", "Invoke");
            return await kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetSentimentFromReviewsAsync(IKernel kernel, string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("input", input);

            ISKFunction function = kernel.Skills.GetFunction("InferSentiment", "Invoke");
            return await kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetLikesAndDislikesFromReviewsAsync(IKernel kernel, string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("input", input);

            ISKFunction function = kernel.Skills.GetFunction("ExtractLikesAndDislikes", "Invoke");
            return await kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetSummaryFromReviewsAsync(IKernel kernel, string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("input", input);

            ISKFunction function = kernel.Skills.GetFunction("GetSummary", "Invoke");
            return await kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetConclusionAsync(IKernel kernel, string hotelSummaries, string userPreferences)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("hotelSummaries", hotelSummaries);
            contextVariables.Set("userPreferences", userPreferences);
           
            ISKFunction function = kernel.Skills.GetFunction("ConcludeReview", "Invoke");
            return await kernel.RunAsync(function!, contextVariables);
        }
    }
}
