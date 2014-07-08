using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi.UrlRewrite
{
    public static class Constants
    {
        public const string RedirectFolderItemsQuery = "fast:/sitecore//*[@@templateid = '{0}']";
        public const string RedirectFolderTemplateId = "{CBE995D0-FCE0-4061-B807-B4BBC89962A7}";
        public const string RedirectFolderConditionItemsQuery = "*[@@templateid = '{0}']";
        public const string ConditionItemTemplateId = "{2083F66B-0A94-4F9C-9833-EF53FAD05D70}";
        public const string InboundRuleTemplateId = "{69DCE9A6-D8C1-463D-AF95-B7FEB326013F}";
        public const string SimpleRedirectInternalTemplateId = "{E30B15B9-34CD-419C-8671-60FEAAAD5A46}";

        public const string RequestedUrlType_MatchesThePattern_ItemId = "{2C94D94E-6FDA-465B-BCA1-4C18EF249EAB}";
        public const string RequestedUrlType_DoesNotMatchThePattern_ItemId = "{35479F72-B51C-4878-8BE1-53373D66633A}";

        public const string UsingType_RegularExpressions_ItemId = "{75BFA469-AE7D-47FD-9A2F-DD8B3AF0865C}";
        public const string UsingType_Wildcards_ItemId = "{E936A17D-0014-4848-9779-1D9BE9095A7D}";
        public const string UsingType_ExactMatch_ItemId = "{3323E74E-2BC8-4055-8A2B-B95656B2E786}";

        public const string LogicalGroupingType_MatchAll_ItemId = "{3FCBE882-C812-4C3C-B89F-4E98A2596C97}";
        public const string LogicalGroupingType_MatchAny_ItemId = "{8E8F11D0-5401-417B-B9E7-46264A2B1D7C}";

        public const string RedirectActionTemplateId = "{D199EF8B-9D4D-420F-A283-E16D7B575625}";

        public const string RedirectType_Permanent_ItemId = "{C194D441-47EB-4C89-A336-4FDE6A2DC6B3}";
        public const string RedirectType_Found_ItemId = "{D80D36EB-F98A-419B-B4C4-497E31FBA8A0}";
        public const string RedirectType_SeeOther_ItemId = "{6AC362BB-AFFD-4FE7-AB23-C2B2B6E33105}";
        public const string RedirectType_Temporary_ItemId = "{5A6BE6F1-9D9A-460F-B990-C40BBF78FC6E}";

        public const string CheckIfInputStringType_IsAFile_ItemId = "{B8D9255F-03CF-4331-AE54-B771E9815A55}";
        public const string CheckIfInputStringType_IsNotAFile_ItemId = "{4F431A8F-3DA5-439B-9640-B5B3D2FDF643}";
        public const string CheckIfInputStringType_IsADirectory_ItemId = "{60952AD2-8862-4B24-AD28-27B69678C6BC}";
        public const string CheckIfInputStringType_IsNotADirectory_ItemId = "{7D4064C9-68D3-4D49-9F47-345CA7675DAB}";
        public const string CheckIfInputStringType_MatchesThePattern_ItemId = "{B30DC355-4122-4260-8AC6-0F9E93205556}";
        public const string CheckIfInputStringType_DoesNotMatchThePattern_ItemId = "{2F8AEB84-DE5C-4102-85B6-AB8059F7CB85}";

        public const string ConditionInputType_QueryString_ItemId = "{DBD45014-AA3C-4F63-92B6-D72C23DD5C26}";
        public const string ConditionInputType_HttpHost_ItemId = "{CE714D26-9BC2-44AB-91CB-5D98F7BF7DE4}";
        public const string ConditionInputType_Https_ItemId = "{F9D6EA61-3C0B-41FA-8DA1-8405BED83BAD}";

    }
}
