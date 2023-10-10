using Microsoft.SemanticKernel;

namespace HotelCompareWebApi.Skills;

public class GetSummarySkill : SkillBase
{
    private const string SkillName = "GetSummary";

    public GetSummarySkill(IKernel kernel) : base(kernel)
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }
}
