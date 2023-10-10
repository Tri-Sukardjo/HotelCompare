using Microsoft.SemanticKernel;

namespace HotelCompareWebApi.Skills;

public class ExtractLikesAndDislikesSkill : SkillBase
{
    private const string SkillName = "ExtractLikesAndDislikes";
    public ExtractLikesAndDislikesSkill(IKernel kernel): base(kernel)
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }
}
