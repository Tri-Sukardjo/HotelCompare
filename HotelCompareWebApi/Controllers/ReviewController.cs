using Azure;
using Azure.AI.TextAnalytics;
using Azure.AI.Translation.Text;
using HotelCompareWebApi.Models;
using HotelCompareWebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.Reflection;
using System.Text.Json;
using DetectedLanguage = Azure.AI.TextAnalytics.DetectedLanguage;

namespace HotelCompareWebApi.Controllers
{
    public class ReviewController : Controller
    {
        private TextAnalyticsClient _textAnalyticsClient;
        private TextTranslationClient _textTranslationClient;
        private IKernel _kernel;

        [Route("compare")]
        public async Task<IActionResult> CompareHotelReviews(
            [FromServices] IKernel kernel,
            [FromServices] TextAnalyticsClient textAnalyticsClient,
            [FromServices] TextTranslationClient textTranslationClient,
            [FromQuery] string hotelNames, 
            [FromQuery] string userPreferences)
        {
            _textAnalyticsClient = textAnalyticsClient;
            _textTranslationClient = textTranslationClient;
            _kernel = kernel;

            SKContext extractedUserPreferences = await ExtractUserPreferences(userPreferences);

            List<string> recallList = new List<string>();
            foreach (string hotelName in hotelNames.Split(","))
            {
                await GetHotelAnalysisAsync(hotelName.Trim());
                recallList.Add(string.Format(@"About {0}: {{{{recall 'give me full information on {1}.'}}}}", hotelName, hotelName));
            }

            // conclude -- this last SK Function is still work in progress, I am still engineering the prompt to do comparison on the 3 hotels correctly
            string hotelKnowledge = String.Join(Environment.NewLine, recallList.ToArray()); 
            SKContext conclusion = await GetConclusionAsync(hotelKnowledge, extractedUserPreferences.Result);

            return this.Ok(conclusion.Result);
        }

        private async Task GetHotelAnalysisAsync(string hotelName)
        {
            FileUtils fileUtils = new FileUtils();

            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format(@"Data\{0}.txt", hotelName));
            var rawInput = fileUtils.ReadFromFile(path);

            string sanitisedInput = SanitiseReviewLanguage(rawInput);

            // get sentiment
            SKContext sentiment = await GetSentimentFromReviewsAsync(sanitisedInput);
            ReviewSentiment sentimentResult = JsonSerializer.Deserialize<ReviewSentiment>(sentiment.Result);

            // get likes and dislike
            SKContext likesDislikes = await GetLikesAndDislikesFromReviewsAsync(sanitisedInput);
            ReviewLikesAndDislikes likesDislikesResult = JsonSerializer.Deserialize<ReviewLikesAndDislikes>(likesDislikes.Result);

            // get summary
            SKContext summary = await GetSummaryFromReviewsAsync(sanitisedInput);
            ReviewSummary summaryResult = JsonSerializer.Deserialize<ReviewSummary>(summary.Result);

            string knowledgeToStore = GetKnowledgeToStoreInMemory(hotelName, sentimentResult, likesDislikesResult, summaryResult);
            await _kernel.Memory.SaveInformationAsync("HotelCompare", id: hotelName, text: knowledgeToStore);

        }

        private string SanitiseReviewLanguage(string rawInput)
        {
            string toLanguageId = "en";
            List<string> reviewList = rawInput.Split(Environment.NewLine).ToList();
            List<string> sanitisedReviews = new List<string>();

            foreach (string review in reviewList)
            {
                // detect original language
                DetectedLanguage detectedLanguage = _textAnalyticsClient.DetectLanguage(review);

                // translate to english, if needed
                if (detectedLanguage.Iso6391Name != toLanguageId)
                {
                    // translate to english
                    Response<IReadOnlyList<TranslatedTextItem>> response = _textTranslationClient.Translate(toLanguageId, review);
                    IReadOnlyList<TranslatedTextItem> translations = response.Value;
                    TranslatedTextItem translation = translations.FirstOrDefault();

                    sanitisedReviews.Add(translation.Translations.FirstOrDefault().Text);
                }
                else
                {
                    sanitisedReviews.Add(review.Trim());
                }
            }

           return string.Join(Environment.NewLine, sanitisedReviews);
        }

        private string GetKnowledgeToStoreInMemory(string hotelName, ReviewSentiment sentiment, ReviewLikesAndDislikes likesDislikes, ReviewSummary summary)
        {
            return string.Format("About {0}: The review sentiment is {1} because {2} The guests like these features: {3}. But they don't like these features: {4}. In summary, {5}",
                hotelName, sentiment.Sentiment, sentiment.Reason, string.Join(", ", likesDislikes.Likes), string.Join(", ", likesDislikes.Dislikes), summary.Summary);
        }

        private async Task<SKContext> ExtractUserPreferences(string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("rawInput", input);

            ISKFunction function = _kernel.Skills.GetFunction("ExtractUserPreference", "Invoke");
            return await _kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetSentimentFromReviewsAsync(string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("rawInput", input);

            ISKFunction function = _kernel.Skills.GetFunction("InferSentiment", "Invoke");
            return await _kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetLikesAndDislikesFromReviewsAsync(string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("rawInput", input);

            ISKFunction function = _kernel.Skills.GetFunction("ExtractLikesAndDislikes", "Invoke");
            return await _kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetSummaryFromReviewsAsync(string input)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("rawInput", input);

            ISKFunction function = _kernel.Skills.GetFunction("GetSummary", "Invoke");
            return await _kernel.RunAsync(function!, contextVariables);
        }

        private async Task<SKContext> GetConclusionAsync(string hotelSummaries, string userPreferences)
        {
            var contextVariables = new ContextVariables();
            contextVariables.Set("hotelSummaries", hotelSummaries);
            contextVariables.Set("userPreferences", userPreferences);
           
            ISKFunction function = _kernel.Skills.GetFunction("ConcludeReview", "Invoke");
            return await _kernel.RunAsync(function!, contextVariables);
        }
    }
}
