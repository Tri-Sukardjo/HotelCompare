using Microsoft.SemanticKernel;

namespace HotelCompareWebApi.Skills;

public class ConcludeReviewSkill: SkillBase
{
    private const string SkillName = "ConcludeReview";
    public ConcludeReviewSkill(IKernel kernel): base(kernel)    
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }
}
