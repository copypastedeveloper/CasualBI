using Casual.BI.API.Commands;
using Casual.BI.LLM;
using Casual.BI.LLM.Parsers;
using Microsoft.AspNetCore.Mvc;

namespace Casual.BI.API.Controllers
{
    [Route("Query")]
    public class QueryBuilderController : ControllerBase
    {
        [HttpPost("build")]
        public async Task<IActionResult> BuildQuery([FromBody]WriteQueryCommand input)
        {
            try
            {
                const string systemPrompt = "You are an expert in postgreSQL, and are assisting in writing queries based on a schema and data examples.  You respond in SQL wrapped in a code block.  Resulting field names should be english and can contain spaces.";
                const string queryPrompt = "As a senior analyst your only output should be SQL code wrapped in a code block. Do not include any other text. Given the above schema, write a detailed and correct Postgres sql query that uses only that schema to answer the analytical question:";
                
                var chatRequest = ChatRequest.Create(temperature: 0f)
                    .WithPersona(systemPrompt)
                    .WithMessage("user",$"{input.TableContext} \n {new string('-', 20)} {queryPrompt}{input.Question}");
                var response = await chatRequest.Send();
                
                var query = response.AsCodeBlocks().FirstOrDefault() ?? response;
                
                return new JsonResult(new {query, ChatHistory = chatRequest.Messages});
            }
            catch (Exception e)
            {
                return new JsonResult(e);
            }
        }

        [HttpPost("explain")]
        public async Task<IActionResult> ExplainQuery([FromBody] ExplainQueryCommand command)
        {
            const string explainPrompt = "Explain the following sql query in one plain english sentence.  Be sure to include any information about the where and group by.  Don't include sql keywords or descriptor words like table.  use the word dataset instead of table.\n";
            var response = await ChatRequest.Create()
                .WithMessage("user", explainPrompt + command.Query)
                .Send();

            return new OkObjectResult(new {explaination = response});
        }
        
        [HttpPost("fix")]
        public async Task<IActionResult> FixQuery([FromBody]FixQueryCommand input)
        {
            const string fixPrompt = "The query above produced the following error: \n {0} \n Rewrite the query with the error fixed. wrap your query in a code block.";

            var chatRequest = ChatRequest.Create()
                .FromMessages(input.ChatHistory)
                .WithMessage("user", string.Format(fixPrompt, input.Error));
            
            var fixedQuery = await chatRequest.Send();
            
            return new JsonResult(new {Query = fixedQuery.AsCodeBlocks().First(), ChatHistory = chatRequest.Messages});
        }
    }
}