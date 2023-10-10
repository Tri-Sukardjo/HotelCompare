using Microsoft.SemanticKernel;

namespace HotelCompareWebApi.Skills;

public class ExtractUserPreferenceSkill : SkillBase
{
    private const string SkillName = "ExtractUserPreference";

    public ExtractUserPreferenceSkill(IKernel kernel) : base(kernel)
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }
}
