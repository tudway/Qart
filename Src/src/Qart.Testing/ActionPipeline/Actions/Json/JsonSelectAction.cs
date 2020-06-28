﻿using Qart.Testing.Framework;

namespace Qart.Testing.ActionPipeline.Actions.Json
{
    public class JsonSelectAction : IPipelineAction
    {
        private readonly string _jsonPath;
        private readonly string _sourceKey;
        private readonly string _targetKey;

        public JsonSelectAction(string jsonPath, string sourceKey = null, string targetKey = null)
        {
            _jsonPath = jsonPath;
            _sourceKey = sourceKey;
            _targetKey = targetKey ?? sourceKey;
        }

        public void Execute(TestCaseContext testCaseContext)
        {
            var effectiveSourceKey = testCaseContext.GetEffectiveItemKey(_sourceKey);
            var effectiveTargetKey = testCaseContext.GetEffectiveItemKey(_targetKey);

            testCaseContext.DescriptionWriter.AddNote("JsonPathSelect", $"{effectiveSourceKey} => {effectiveTargetKey}");
            var jtoken = testCaseContext.GetRequiredItemAsJToken(effectiveSourceKey);
            var result = jtoken.SelectToken(_jsonPath);
            testCaseContext.SetItem(effectiveTargetKey, result);
        }
    }
}
