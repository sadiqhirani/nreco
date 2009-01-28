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
using System.Collections;
using System.Collections.Generic;

namespace NReco.Providers
{
	/// <summary>
	/// Union-type composite list provider implementation.
	/// </summary>
	public class UnionListProvider<Context,T> : IProvider<Context,IList<T>>
	{
		bool _IgnoreNullResult = true;
		bool _SkipInvalidEntryType = false;
		

		public bool IgnoreNullResult {
			get { return _IgnoreNullResult; }
			set { _IgnoreNullResult = value; }
		}

		public bool SkipInvalidEntryType {
			get { return _SkipInvalidEntryType; }
			set { _SkipInvalidEntryType = value; }
		}

		public IProvider<object, object>[] Providers { get; set; }

		public UnionListProvider()
		{
		}

		protected bool IsEnumerable(object o) {
			return o is IEnumerable && !(o is string);
		}

		public IList<T> GetList(Context context) {
			IList<T> result = new List<T>();
			for (int i=0; i<Providers.Length; i++) {
				object subList = Providers[i].Provide(context);
				if (subList==null && IgnoreNullResult)
					continue;
				if (subList==null)
					result.Add( (T)subList );
				else if (subList is T)
					result.Add((T)subList );
				else if (subList is IList<T>) {
					foreach (T entry in (IList<T>)subList)
						result.Add(entry);
				} else if (IsEnumerable(subList)) {
					foreach (object o in (IEnumerable)subList) {
						if (SkipInvalidEntryType) {
							if (o is T)
								result.Add( (T)o );
						} else
							result.Add( (T)o );
					}

				} else {
					if (!SkipInvalidEntryType)
						result.Add( (T)subList );
				}
				
			}
			return result;
		}

		public IList<T> Provide(Context context) {
			return GetList(context);
		}

	}
}
