using System.Text.Json;
using Casual.BI.API.Commands;
using Casual.BI.API.LLM.Agent;
using Casual.BI.API.LLM.Prompts;
using Casual.BI.API.LLM.Tools;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Casual.BI.API.Controllers;

[Route("api/agent")]
public class AgentController : ControllerBase
{
    [HttpPost("initiate")]
    public async Task Initiate([FromBody]InitiateAgentCommand command)
    {
        HttpContext.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
        var response = HttpContext.Response;
        
        var prompt = new DataAnalysisAgentPrompt(command.TableContext, command.Question);

        try
        {
            var agent = await Agent.FromPrompt(prompt);
            await using var sw = new StreamWriter(response.Body);
            
            while (agent.CurrentStep?.Tool is ServerTool serverTool)
            {
                await agent.Execute(async json =>
                {
                    await sw.WriteAsync(json);
                    await sw.FlushAsync();
                });
            }
            
            await sw.WriteAsync(JsonSerializer.Serialize(agent));
            await sw.FlushAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}