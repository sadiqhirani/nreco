#region License
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
using System.Text;
using System.Reflection;

using NReco.Collections;

namespace NReco.Converting {

	/// <summary>
	/// Generic IDictionary converter interface
	/// </summary>
	public class GenericDictionaryConverter : BaseGenericTypeConverter {
		
		protected override bool CanConvertFromGeneric { get { return true; } }
		protected override bool CanConvertToGeneric { get { return true; } }
		protected override Type GenDefIType { get { return typeof(IDictionary<,>); } }
		protected override Type NonGenIType { get { return typeof(IDictionary); } }

		public GenericDictionaryConverter() { }

		protected override object ConvertFromGeneric(object o, Type fromGenIType) {
			return CreateGenericWrapper(typeof(DictionaryWrapper<,>), fromGenIType, o);
		}
		protected override object ConvertToGeneric(object o, Type toGenIType) {
			return CreateGenericWrapper(typeof(DictionaryGenericWrapper<,>), toGenIType, o);
		}


	}
}
