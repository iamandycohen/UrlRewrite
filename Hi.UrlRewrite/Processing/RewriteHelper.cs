using Hi.UrlRewrite.Entities.Conditions;
using Hi.UrlRewrite.Processing.Results;
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Web;

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

        public static string ReplaceTokens(Replacements replacements, string input)
        {
            var tokenRegex = new Regex(@"{(\w+)}");
            var tokenMatches = tokenRegex.Matches(input);
            string output = input;

            foreach (Match tokenMatch in tokenMatches)
            {
                var wholeToken = tokenMatch.Groups[0].Value;
                var token = tokenMatch.Groups[1].Value;
                string tokenReplacement = null;

                const string RESPONSE = "RESPONSE_";
                const string REQUEST = "REQUEST_";

                if (replacements.ResponseHeaders != null && token.StartsWith(RESPONSE))
                {
                    var tokenKey = token.Remove(0, RESPONSE.Length);
                    tokenKey = tokenKey.Replace("_", "-");

                    tokenReplacement = replacements.ResponseHeaders[tokenKey];
                }
                else if (replacements.RequestHeaders != null && token.StartsWith(REQUEST))
                {
                    var tokenKey = token.Remove(0, REQUEST.Length);
                    tokenKey = tokenKey.Replace("_", "-");

                    tokenReplacement = replacements.RequestHeaders[tokenKey];
                }
                else if (replacements.RequestServerVariables != null)
                {
                    tokenReplacement = replacements.RequestServerVariables[token];
                }

                if (!string.IsNullOrEmpty(tokenReplacement))
                {
                    output = output.Replace(wholeToken, tokenReplacement);
                }
            }

            return output;
        }

        public static string ReplaceTokenInput(NameValueCollection serverVariables, string input, Tokens token)
        {
            input = input.Replace(token.Formatted(), serverVariables[token.ToString()]);

            return input;
        }

        public static ConditionMatch TestConditionMatch(Replacements replacements, Condition condition, Match previousConditionMatch = null)
        {
            var conditionRegex = new Regex(condition.Pattern, condition.IgnoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);

            var conditionInput = ReplaceTokens(replacements, condition.InputString);
            if (previousConditionMatch != null)
            {
                conditionInput = ReplaceConditionBackReferences(previousConditionMatch, conditionInput);
            }

            var returnMatch = conditionRegex.Match(conditionInput);

            return new ConditionMatch { Match = returnMatch, ConditionInput = conditionInput };
        }

        public static ConditionMatchResult TestConditionMatches(IConditionsProperties rule, Replacements replacements, out Match lastConditionMatch)
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
                    var conditionMatched = RewriteHelper.TestConditionMatch(replacements, condition, lastConditionMatch);
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
                    var conditionMatched = RewriteHelper.TestConditionMatch(replacements, condition);
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

        public class Replacements
        {
            public Uri Uri { get; set; }
            public NameValueCollection RequestServerVariables { get; set; }
            public NameValueCollection RequestHeaders { get; set; }
            public NameValueCollection ResponseHeaders { get; set; }
        }

    }
}