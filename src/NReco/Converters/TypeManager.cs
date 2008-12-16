#region License
/*
 * NReco library (http://code.google.com/p/nreco/)
 * Copyright 2008 Vitaliy Fedorchenko
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
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using NReco.Logging;

namespace NReco.Converters {
	
	/// <summary>
	/// Provides access to default type conversion mechanizm.
	/// </summary>
	public static class TypeManager {
		static IList<ITypeConverter> _Converters;
		static ILog log = LogManager.GetLogger(typeof(TypeManager));

		static TypeManager() {
			_Converters = new List<ITypeConverter>();
			// default set
			Converters.Add(new GenericListConverter());
			Converters.Add(new GenericDictionaryConverter());
			Converters.Add(new GenericProviderConverter());
			Converters.Add(new GenericOperationConverter());
			Converters.Add(new GenericCollectionConverter());
		}

		/// <summary>
		/// Configure type manager from application config.
		/// </summary>
		public static void Configure() {
			string sectionName = typeof(TypeManager).Namespace;
			object config = ConfigurationSettings.GetConfig(sectionName);
			if (config == null)
				config = ConfigurationSettings.GetConfig(sectionName.ToLower());
			if (config != null) {
				IList<Type> convTypes = config as IList<Type>;
				if (convTypes == null) {
					log.Write(LogEvent.Warn, "Invalid converters configuration type: {0}", config.GetType());
				} else {
					int addedNewConv = 0;
					foreach (Type t in convTypes) {
						ITypeConverter conv = Activator.CreateInstance(t) as ITypeConverter;
						if (conv != null) {
							// skip duplicates
							Converters.Add(conv);
						} else {
							log.Write(LogEvent.Warn, "Converter type {0} does not implement ITypeConverter interface - ignored", t);
						}
					}
					log.Write(LogEvent.Info, "Initialized {0} new converters from application config.", addedNewConv);
				}
					
			}
		}

		/// <summary>
		/// List of default converters.
		/// </summary>
		public static IList<ITypeConverter> Converters {
			get { return _Converters; }
		}
		
		/// <summary>
		/// Find converter in default converters for given conversion.
		/// </summary>
		/// <param name="fromType">from type</param>
		/// <param name="toType">to type</param>
		/// <returns>type converter that can perform conversion or null</returns>
		public static ITypeConverter FindConverter(Type fromType, Type toType) {
			for (int i=0; i<Converters.Count; i++)
				if (Converters[i].CanConvert(fromType, toType))
					return Converters[i];
			return null;
		}

		public static bool CanConvert(Type fromType, Type toType) {
			return FindConverter(fromType,toType)!=null;
		}

		public static object Convert(object o, Type toType) {
			if (o==null) {
				if (toType.IsValueType) return null;
				throw new InvalidCastException("Cannot convert null to value type");
			}
			ITypeConverter conv = FindConverter(o.GetType(),toType);
			if (conv!=null)
				return conv.Convert(o,toType);
			throw new InvalidCastException(
					String.Format("Cannot convert from {0} to {1}",o.GetType().Name,toType.Name));
		}

	}
}
