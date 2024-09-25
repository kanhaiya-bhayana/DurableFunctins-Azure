using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LeaveApproval
{
    public static class HttpRequestDataExtentions
    {
        public static async Task<T> GetFromBody<T>(this HttpRequestData req)
        {
            var bodyBinaryData = await BinaryData.FromStreamAsync(req.Body);
            return bodyBinaryData.ToObjectFromJson<T>();
        }

        public static T GetContextType<T>(this TaskOrchestrationContext context)
        {
            var jsonElement = context.GetInput<JsonElement>();
            var inputResult = jsonElement.Deserialize<T>();
            if (inputResult == null)
            {
                throw new ArgumentNullException();
            }
            return inputResult;
        }
    }
}
