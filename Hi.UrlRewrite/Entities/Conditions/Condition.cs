using System;

namespace Hi.UrlRewrite.Entities.Conditions
{
    [Serializable]
    public class Condition
    {
        public string Name { get; set; }
        public string InputString { get; set; }
        public CheckIfInputString? CheckIfInputString { get; set; }
        public string Pattern { get; set; }
        public bool IgnoreCase { get; set; }
    }
}
