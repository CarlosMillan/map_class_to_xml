using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Web.Script.Serialization;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

namespace ConverterToXMLLib
{
    public class ConverterToXML<T> where T : BaseConverter
    {
	  private List<T> _target;
	  private XmlDocument _outputfile;
	  private XmlNode _currentnode;
	  private XmlNode _rootnode;
	  private ParentNode _parentnodedata;
	  private string _path;	  

	  public string XmlString
	  {
		get { return _outputfile.OuterXml; }
	  }

	  public string PathFile
	  {
		set 
		{
		  _path = value;
		}
	  }

	  #region Public Methods
	  private ConverterToXML() 
	  {				
		_outputfile = new XmlDocument();
		_currentnode = _outputfile;
		_rootnode = null;
		_path = string.Empty;
	  }

	  public ConverterToXML(List<T> target, ParentNode parent)
		:this()
	  {
		_target = target;
		_parentnodedata = parent;
	  }

	  public ConverterToXML(List<T> target, ParentNode parent, string path)
		: this(target, parent)
	  {
		_path = path;
	  }

	  public void Convert() 
	  {		
		CreateXmlDocument();		
	  }

	  public void SaveDocument()
	  {
		_outputfile.Save(String.Concat(_path, _parentnodedata.Name, ".xml"));
	  }
	  #endregion

	  #region Private Methods
	  private void CreateXmlDocument()
	  {				
		MapParentNode();

		foreach (var S in _target)
		{
		  XmlNode Current = _currentnode;
		  CreateXmlContent(S.GetType(), S);
		  _currentnode = Current;
		}

		//XmlDeclaration XmlDeclaration = _outputfile.CreateXmlDeclaration("1.0", "UTF-8", null);
		//_outputfile.InsertBefore(XmlDeclaration, _rootnode);
	  }

	  private void CreateXmlContent(Type fromclass, object target)
	  {
		string NodeName = fromclass.Name;		
		var CAttr = fromclass.GetCustomAttributes(typeof(MapToXML), true);

		if (CAttr.Count() > 0)		  
		  NodeName = ((MapToXML)CAttr.First()).Name;

		if (target != null)
		{
		  CreateNode(NodeName);
		  CreateNodeAttributes(fromclass, target);
		}
	  }

	  private void CreateNode(string name)
	  {
		XmlNode NewNode = _outputfile.CreateElement(name);
		_currentnode.AppendChild(NewNode);
		_currentnode = NewNode;

		if (_rootnode == null)
		  _rootnode = NewNode;
	  }

	  private void CreateNodeAttributes(Type from, object target)
	  {
		PropertyInfo[] Properties = GetClassAttributes(from);
		XmlNode Current = _currentnode;

		for (int IndexProperties = 0; IndexProperties < Properties.Length; IndexProperties++)
		{
		  if (IsParseLikeAttribute(Properties[IndexProperties].PropertyType))
		  {
			var CAttr = Properties[IndexProperties].GetCustomAttributes(typeof(MapToXML), true);			  

			if (CAttr.Count() > 0)
			{			  
			  string NodeName = ((MapToXML)CAttr.First()).Name;

			  XmlAttribute NodeAttribute = _outputfile.CreateAttribute(NodeName);
			  var Value = GetPropValue(target, Properties[IndexProperties].Name);

			  if (Value != null)
			  {
				NodeAttribute.Value = Value.ToString();
				_currentnode.Attributes.Append(NodeAttribute);
			  }
			}
		  }
		  else
		  {
			object Src = GetPropValue(target, Properties[IndexProperties].Name);

			var Enumerable = Src as System.Collections.IEnumerable;

			if (Enumerable != null)
			{
			  foreach (var S in Enumerable)
			  {
				CreateXmlContent(S.GetType(), S);
				_currentnode = Current;
			  }
			}
			else			
			  CreateXmlContent(Properties[IndexProperties].PropertyType, Src);

			_currentnode = Current;
		  }
		}
	  }

	  private PropertyInfo[] GetClassAttributes(Type parent)
	  {
		PropertyInfo[] Prop = parent.GetProperties(BindingFlags.Public | BindingFlags.Instance);		
		PropertyInfo[] FilteredProp = Prop.Where(x => x.GetCustomAttributes(typeof(MapToXML), true).Count() > 0).ToArray();
		return FilteredProp;
	  }

	  private object GetPropValue(object src, string propName)
	  {
		if(src != null)
		  return  src.GetType().GetProperty(propName).GetValue(src, null);

		return string.Empty;
	  }

	  private bool IsParseLikeAttribute(Type target)
	  {
		if (target == typeof(string)
			|| target == typeof(int) || target == typeof(Nullable<int>)
			|| target == typeof(bool) || target == typeof(Nullable<bool>)
			|| target == typeof(decimal) || target == typeof(Nullable<decimal>) 
			|| target == typeof(float) || target == typeof(Nullable<float>)
			|| target == typeof(double) || target == typeof(Nullable<double>)
			|| target == typeof(DateTime) || target == typeof(Nullable<DateTime>))
		  return true;

		return false;
	  }

	  private void MapParentNode()
	  {
		CreateNode(_parentnodedata.Name);

		foreach (var Val in _parentnodedata.Values)
		{
		  XmlAttribute NodeAttributeVersion = _outputfile.CreateAttribute(Val.Key);
		  NodeAttributeVersion.Value = Val.Value;
		  _currentnode.Attributes.Append(NodeAttributeVersion);
		}
	  }
	  #endregion
	}
}
