using System;
using System.Collections.Generic;
using System.Linq;

namespace ConverterToXMLLib
{
  public class ParentNode
  {
	private List<KeyValuePair<string, string>> _values;
	private string _parentnodename;

	public List<KeyValuePair<string, string>> Values
	{
	  get { return _values; }
	}

	public string Name
	{
	  get { return _parentnodename; }
	}

	/// <summary>
	/// Create a parent node
	/// </summary>
	/// <param name="attributes">Receives node name and key-value pair paramenters to create a parent node</param>
	/// <example>
	///	  Receives Node name followed by key-pair attribures
	///	  <code>
	///		  ParentNode Varx = ParentNode("nodename", "firstattribute", "value", "secondattribute", 10, "thirdattribute", false)
	///	  </code>
	/// </example>	
	public ParentNode(params object[] attributes)
	{
	  if (attributes.Length % 2 != 0)
	  {
		_parentnodename = attributes.First().ToString();
	
		_values = new List<KeyValuePair<string, string>>();

		for (int index = 1; index < attributes.Length; index++ )
		{
		  KeyValuePair<string, string> Item = new KeyValuePair<string, string>(attributes[index].ToString(), attributes[++index].ToString());
		  _values.Add(Item);		  
		}
	  }
	  else
		throw new Exception("It must have odd parameters");
	}
  }
}
