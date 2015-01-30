using System;

namespace ConverterToXMLLib
{
  [System.AttributeUsage(System.AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Field)]
  public class MapToXML : System.Attribute
  {
	private string name;	

	public string Name
	{
	  get { return name; }
	}

	public MapToXML() { }

	public MapToXML(string name)
	{
	  this.name = name;	  
	}	
  }
}
