﻿using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using System.Text.Json;

namespace RequestApproval
{
    public static class HttpRequestDataExtentions
    {
        public static async Task<T> GetFromBody<T>(this HttpRequestData req)
        {
            var bodyBinaryData = await BinaryData.FromStreamAsync(req.Body);
            return bodyBinaryData.ToObjectFromJson<T>();
        }

        public static T GetContextInput<T>(this TaskOrchestrationContext context)
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
