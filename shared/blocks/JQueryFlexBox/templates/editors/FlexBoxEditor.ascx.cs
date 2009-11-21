﻿#region License
/*
 * NReco library (http://nreco.googlecode.com/)
 * Copyright 2008,2009 Vitaliy Fedorchenko
 * Distributed under the LGPL licence
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Data;
using System.Web.UI.WebControls;
using System.Globalization;

using NReco;
using NReco.Converting;
using NReco.Collections;
using NReco.Web;
using NReco.Web.Site;
using NI.Data.Dalc;
using NI.Data.Dalc.Web;
using NI.Data.Dalc.Linq;
using NI.Data.RelationalExpressions;

[ValidationProperty("Value")]
public partial class FlexBoxEditor : System.Web.UI.UserControl {
	
	public string DalcServiceName { get; set; }
	public string JsScriptName { get; set; }
	public bool RegisterJs { get; set; }
	public string DataContextJs { get; set; }
	
	public string Relex { get; set; }
	public string TextFieldName { get; set; }
	public string ValueFieldName { get; set; }
	public int Width {get;set;}
	public bool LocalizationEnabled { get; set; }
	
	public string Value {
		get { return selectedValue.Value!="" ? selectedValue.Value : null; }
		set { selectedValue.Value = value; }
	}
	
	public FlexBoxEditor() {
		RegisterJs = true;
		LocalizationEnabled = true;
		Width = 0;
		JsScriptName = "js/jquery.flexbox.min.js";
	}
	
	protected override void OnLoad(EventArgs e) {
		if (RegisterJs) {
			var scriptTag = "<s"+"cript language='javascript' src='"+JsScriptName+"'></s"+"cript>";
			if (!Page.ClientScript.IsStartupScriptRegistered(Page.GetType(), JsScriptName)) {
				Page.ClientScript.RegisterStartupScript(Page.GetType(), JsScriptName, scriptTag, false);
			}
			// one more for update panel
			System.Web.UI.ScriptManager.RegisterClientScriptInclude(Page, Page.GetType(), JsScriptName, "ScriptLoader.axd?path="+JsScriptName);
		}
	}
	
	protected string GetValueText() {
		var dalc = WebManager.GetService<IDalc>(DalcServiceName);
		var relexParser = new RelExQueryParser(false);
		var exprResolver = WebManager.GetService<NI.Common.Expressions.IExpressionResolver>("defaultExprResolver");

		var qContext = new Hashtable();
		qContext["q"] = String.Empty;
		Query q = (Query)relexParser.Parse( Convert.ToString( exprResolver.Evaluate( qContext, Relex ) ) );		
		q.Root = new QueryConditionNode( (QField)ValueFieldName, Conditions.Equal, (QConst)Value ); //& q.Root; 
		var data = new Hashtable();
		if (dalc.LoadRecord(data, q)) {
			var text = Convert.ToString(data[TextFieldName]);
			return LocalizationEnabled?WebManager.GetLabel(text,this):text;
		} else {
			Value = null;
			return "";
		}
	}
	
}
