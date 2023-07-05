using Casual.BI.API.Commands;
using Casual.BI.API.LLM;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Casual.BI.API.Controllers;

[Route("DataAnalysis")]
public class DataAnalysisController : ControllerBase
{

    [HttpPost]
    public async Task AnalyzeQuery([FromBody] WriteQueryCommand input)
    {
        HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
        var response = HttpContext.Response;
        var jobDescription = $"You are a professional data analyst working at a company. The current date is {DateTime.Today}.  Your job is to provide insightful and incredibly accurate analysis of a dataset for consumption by the Board of Directors. You format your findings in well structured MarkDown";
        var analysisPrompt = "Presented with the below csv data and the question that it answers, create a narrative explaining what it means for your company.  Be as verbose as needed to provide insights." +
                                      "Use a formal tone.  Use bullet points for lists.\n";

        var result = ChatRequest.Create()
            .WithPersona(jobDescription)
            .WithMessage("user", $"{analysisPrompt}\n{input.Question} \n {input.TableContext}")
            .SendAsStream();
        
        await using var sw = new StreamWriter(response.Body);
        await foreach (var completion in result)
        {
            if (!completion.Successful) return;
            if (string.IsNullOrEmpty(completion.Choices.FirstOrDefault()?.FinishReason))
            {
                var text = completion.Choices.FirstOrDefault()?.Message.Content;
                await sw.WriteAsync(text);
                await sw.FlushAsync();
            }
            else
            {
                await sw.FlushAsync();
            }
        }
    }
}