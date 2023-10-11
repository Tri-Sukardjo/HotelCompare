using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;

namespace HotelCompareWebApi.Skills;

public class ConcludeReviewSkill: SkillBase
{
    private const string SkillName = "ConcludeReview";
    public ConcludeReviewSkill(IKernel kernel): base(kernel)    
    {
        InitiateSkill("HotelReviewSkills", SkillName);
    }

    [SKFunction, Description("invoke the skill with multiple inputs")]
    public async Task<SKContext> InvokeWithContext(SKContext context)
    {
        return await this._skillFunction!.InvokeAsync(context);
    }
}
