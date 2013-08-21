using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Waldi.Engine;

namespace Waldi.Packages
{
	public class FeatureList : NamedItemList<Feature>
	{
		public bool IsFeatureEnabled (string featurename)
		{
			return this.list.Where(f => f.Name == featurename).Count() > 0;
		}
	}
}