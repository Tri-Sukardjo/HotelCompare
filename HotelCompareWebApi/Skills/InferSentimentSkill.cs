using Microsoft.SemanticKernel;

namespace HotelCompareWebApi.Skills;

public class InferSentimentSkill : SkillBase
{
    private const string SkillName = "InferSentiment";

    public InferSentimentSkill(IKernel kernel) : base(kernel)
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }
}
