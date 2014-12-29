using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Processing.Results;

namespace Hi.UrlRewrite.Processing
{
    public class RewriteHelper
    {
        public static string ReplaceRuleBackReferences(Match inboundRuleMatch, string input)
        {
            return ReplaceBackReferences(inboundRuleMatch, input, "R");
        }

        public static string ReplaceConditionBackReferences(Match conditionMatch, string input)
        {
            return ReplaceBackReferences(conditionMatch, input, "C");
        }

        public static string ReplaceBackReferences(Match match, string input, string backReferenceVariable)
        {
            var groupRegex = new Regex(@"({" + backReferenceVariable + @":(\d+)})", RegexOptions.None);

            foreach (Match groupMatch in groupRegex.Matches(input))
            {
                var num = groupMatch.Groups[2];
                var groupIndex = Convert.ToInt32(num.Value);
                var group = match.Groups[groupIndex];
                var matchText = groupMatch.ToString();
                var groupValue = group.Value;

                input = input.Replace(matchText, groupValue);
            }
            return input;
        }

        public static string ReplaceTokens(Uri uri, string input)
        {

            // host replacement
            input = input.Replace(Tokens.HTTP_HOST.Formatted(), uri.Host);

            // querystring replacement
            var query = uri.Query;
            if (query.Length > 0)
            {
                query = query.Substring(1);
            }
            input = input.Replace(Tokens.QUERY_STRING.Formatted(), HttpUtility.UrlDecode(query));

            // scheme replacement
            var https = uri.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.InvariantCultureIgnoreCase) ? "on" : "off";
            input = input.Replace(Tokens.HTTPS.Formatted(), https);

            return input;
        }

        public static string ReplaceTokenInput(NameValueCollection serverVariables, string input, Tokens token)
        {
            input = input.Replace(token.Formatted(), serverVariables[token.ToString()]);

            return input;
        }

        public static ConditionMatch ConditionMatch(Uri uri, Condition condition, Match previousConditionMatch = null)
        {
            var conditionRegex = new Regex(condition.Pattern, condition.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            var conditionInput = ReplaceTokens(uri, condition.InputString);
            if (previousConditionMatch != null)
            {
                conditionInput = ReplaceConditionBackReferences(previousConditionMatch, conditionInput);
            }

            var returnMatch = conditionRegex.Match(conditionInput);

            return new ConditionMatch { Match = returnMatch, ConditionInput = conditionInput };
        }

        public static ConditionMatchResult TestConditionMatches(IConditionsProperties rule, Uri originalUri, out Match lastConditionMatch)
        {
            var conditionMatchResult = new ConditionMatchResult();
            bool conditionMatches;
            lastConditionMatch = null;

            var conditionLogicalGrouping = rule.ConditionLogicalGrouping.HasValue
                ? rule.ConditionLogicalGrouping.Value
                : LogicalGrouping.MatchAll;

            conditionMatchResult.LogincalGrouping = conditionLogicalGrouping;

            if (conditionLogicalGrouping == LogicalGrouping.MatchAll)
            {
                foreach (var condition in rule.Conditions)
                {
                    var conditionMatched = RewriteHelper.ConditionMatch(originalUri, condition, lastConditionMatch);
                    var conditionMatch = conditionMatched.Match;
                    var conditionInput = conditionMatched.ConditionInput;

                    conditionMatches = conditionMatch.Success;

                    if (condition.CheckIfInputString != null && condition.CheckIfInputString.Value == CheckIfInputString.DoesNotMatchThePattern)
                    {
                        conditionMatches = !conditionMatches;
                    }

                    if (conditionMatches)
                    {
                        lastConditionMatch = conditionMatch;
                        conditionMatchResult.MatchedConditions.Add(new MatchedCondition(condition, conditionInput));
                        conditionMatchResult.Matched = true;
                    }
                }
            }
            else
            {
                foreach (var condition in rule.Conditions)
                {
                    var conditionMatched = RewriteHelper.ConditionMatch(originalUri, condition);
                    var conditionMatch = conditionMatched.Match;
                    var conditionInput = conditionMatched.ConditionInput;

                    conditionMatches = conditionMatch.Success;

                    if (condition.CheckIfInputString != null && condition.CheckIfInputString.Value == CheckIfInputString.DoesNotMatchThePattern)
                    {
                        conditionMatches = !conditionMatches;
                    }

                    if (!conditionMatches) continue;

                    conditionMatchResult.MatchedConditions.Add(new MatchedCondition(condition, conditionInput));
                    lastConditionMatch = conditionMatch;
                    conditionMatchResult.Matched = true;

                    break;
                }
            }

            return conditionMatchResult;
        }



    }
}