using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace FeatureSamplesForiOS
{
	public class SamplesList
	{
		public SamplesList()
		{
			Sections = new Collection<SampleSection>();
		}
		
		public static SamplesList LoadAsync()
		{
			SamplesList sampleList = new SamplesList();

			XDocument doc = XDocument.Load("SampleList.xml");
			foreach (var samplesXml in doc.Root.Elements("samples"))
			{
				SampleSection section = new SampleSection { Name = samplesXml.Attribute("name").Value };
				sampleList.Sections.Add(section);

				foreach (var sampleXml in samplesXml.Elements("sample"))
				{
					Sample sample = new Sample();
                    sample.Name = sampleXml.Attribute("name").Value;
                    sample.TypeName = sampleXml.Attribute("type").Value;
					section.Samples.Add(sample);
				}
			}

			return sampleList;
		}

		public Collection<SampleSection> Sections { get; private set; }
	}

	public class SampleSection
	{
		public SampleSection()
		{
			Samples = new Collection<Sample>();
		}

		public string Name { get; set; }

		public Collection<Sample> Samples { get; private set; }
    }

	public class Sample
	{
		public string Name { get; set; }

		public string TypeName { get; internal set; }
	}
}
