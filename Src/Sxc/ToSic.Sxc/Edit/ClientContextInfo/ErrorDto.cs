﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using ToSic.Eav.Code.InfoSystem;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Problems;
using static System.Text.Json.Serialization.JsonIgnoreCondition;
using static ToSic.Sxc.Blocks.Problems.ProblemReport;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ErrorDto
    {
        [JsonPropertyName("type")]
        public string Type { get; }

        [JsonPropertyName("problems")]
        [JsonIgnore(Condition = WhenWritingDefault)]
        public IEnumerable<ProblemReport> Problems { get; }

        internal ErrorDto(IBlock block, string errorCode, List<Exception> exsOrNull, CodeInfosInScope codeWarnings)
        {
            // New mechanism in 16.01
            Type = errorCode;

            if (!block.Context.User.IsSiteAdmin) return;

            // New problems report in 16.02
            var problems = new List<ProblemReport>(block.Problems);
            var additional = new ProblemSuggestions().AddSuggestions(block, exsOrNull, errorCode);
            problems.AddRange(additional);

            problems.AddRange(codeWarnings
                .GetWarnings()
                .GroupBy(w => w.Use.Change)
                .Select(warningGroup => new ProblemReport
                {
                    Code = "warning",
                    Severity = ErrorSeverity.warning,
                    Link = warningGroup.Key.Link,
                    Message =
                        $"{warningGroup.Key.Message} ({warningGroup.Count()} cases{(warningGroup.Count() > 3 ? " - possibly in a loop" : "")})",
                }));

            if (codeWarnings.GetObsoletes().Any())
                problems.Add(new ProblemReport
                {
                    Code = "obsolete",
                    Severity = ErrorSeverity.warning
                });

            var appId = block.App?.AppId;
            if (appId != null && codeWarnings.CodeInfoStats.AppHasWarnings(appId.Value))
                problems.Add(new ProblemReport
                {
                    Code = "obsolete-app",
                    Severity = ErrorSeverity.warning
                });

            Problems = problems.Any() ? problems : null;
        }
    }
}
