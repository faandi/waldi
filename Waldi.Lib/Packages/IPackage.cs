using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waldi.Engine;

namespace Waldi.Packages
{
	public interface IPackage : INamedItem, IEquatable<IPackage>
    {
		string Description { get; }
		string Url { get; }
		bool IsMaster { get; }
        NamedItemList<PackageVersion> Versions { get; }
		DependencyList Dependencies { get; }

        // gehört das nicht ins Project ? nein
		PackageVersion SelectedVersion { get; set; }
		
        // dependency packages
		PackageList Packages { get; }
		FeatureList Features { get; }

        // Besserer Name für Source ?? ConfigSource ?? PackageConfig ?? 
        // SourceList Sources { get; } (ISource)


		// Hier alle features, (von Dependant geforderten Features und User gwählten)
		// List<Feature> EnabledFeatures { get; }

		// "Eltern" Packages.
		// Wird gebraucht für Code in Templates
		// zb.: Package.Dependencies["jQuery.XXX"].Config.TemplatePath
		// zb.: Package.Dependencies["jQuery.XXX"].Definition.Files["CCCC"].Path
		// PackageList Dependencies { get; }

		// bool IsFeatureEnabled (string featurename);

		//string DefinitionPath { get; }
		//string ConfigPath { get; }

        //void Compile(TemplateEngine engine);
        //void SetSources();
    }
}
