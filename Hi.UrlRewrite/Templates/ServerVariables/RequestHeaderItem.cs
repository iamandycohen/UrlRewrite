using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hi.UrlRewrite.Templates.ServerVariables
{
	public class RequestHeaderItem : CustomItem
	{
		public static readonly string TemplateId = "{698FFCC4-0D22-4B97-ACFE-3D04994B4B65}";

		#region Inherited Base Templates

		private readonly BaseServerVariableItem _BaseServerVariableItem;
		public BaseServerVariableItem BaseServerVariableItem { get { return _BaseServerVariableItem; } }

		#endregion

		public RequestHeaderItem(Item innerItem)
			: base(innerItem)
		{
			_BaseServerVariableItem = new BaseServerVariableItem(innerItem);
		}

		public static implicit operator RequestHeaderItem(Item innerItem)
		{
			return innerItem != null ? new RequestHeaderItem(innerItem) : null;
		}

		public static implicit operator Item(RequestHeaderItem customItem)
		{
			return customItem != null ? customItem.InnerItem : null;
		}

			
	}
}