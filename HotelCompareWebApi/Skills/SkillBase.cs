using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;
using System.ComponentModel;
using HotelCompareWebApi.Utilities;
using System.Reflection;

namespace HotelCompareWebApi.Skills;

public class SkillBase
{
    private readonly IKernel _kernel;
    private ISKFunction? _skillFunction;
    public SkillBase(IKernel kernel)
    {
        _kernel = kernel;
    }

    protected void InitiateSkill(string skillCollectionName, string skillName)
    {
        FileUtils fileUtils = new FileUtils();
        string skillDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), string.Format(@"Skills\{0}\", skillName));

        PromptTemplateConfig promptConfig = GetPromptConfig(skillDirectory, fileUtils);
        string promptText = GetPromptText(skillDirectory, fileUtils);

        var promptTemplate = new PromptTemplate(promptText, promptConfig, _kernel);
        var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

        _skillFunction = _kernel.RegisterSemanticFunction(skillCollectionName, skillName, functionConfig);
    }

    private string GetPromptText(string skillDirectory, FileUtils fileUtils)
    {
        string promptTextPath = Path.Combine(skillDirectory, "prompt.txt");
        return fileUtils.ReadFromFile(promptTextPath);
    }

    private PromptTemplateConfig GetPromptConfig(string skillDirectory, FileUtils fileUtils)
    {
        string promptConfigPath = Path.Combine(skillDirectory, "settings.json");
        return fileUtils.ReadFromJSONFile<PromptTemplateConfig>(promptConfigPath);
    }

    [SKFunction, Description("invoke the skill")]
    public async Task<SKContext> Invoke(string input)
    {
        return await _skillFunction!.InvokeAsync(input);
    }
}
