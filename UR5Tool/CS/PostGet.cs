using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace UR5Tool
{
    class PostGet
    {
        /*DictionaryToXml*/
        public static string DictionaryToXml(Dictionary<string, object> Post)
        {
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement xmlElement_class = xmlDocument.CreateElement("Post");
            foreach (KeyValuePair<string, object> item in Post)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    XmlElement xmlElement_data = xmlDocument.CreateElement(item.Key);
                    xmlElement_data.InnerText = $"{item.Value}";
                    xmlElement_class.AppendChild(xmlElement_data);
                }
                if (item.Value.GetType() == typeof(List<string>))
                {
                    XmlElement xmlElement_data = xmlDocument.CreateElement(item.Key);

                    XmlElement Id_xml;
                    foreach (string Value in (List<string>)item.Value)
                    {
                        Id_xml = xmlDocument.CreateElement("data");
                        Id_xml.InnerText = $"{Value}";
                        xmlElement_data.AppendChild(Id_xml);
                    }
                    xmlElement_class.AppendChild(xmlElement_data);
                }
            }
            xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", ""));
            xmlDocument.AppendChild(xmlElement_class);
            return xmlDocument.OuterXml;
        }
        /*XmlToDictionary*/
        public static Dictionary<string, object> XmlToDictionary(string XmlText)
        {
            Dictionary<string, object> Post = new Dictionary<string, object> { };
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(XmlText);
            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("Post").ChildNodes;
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                try
                {
                    XmlNodeList Keys = xmlDocument.SelectSingleNode("Post").SelectSingleNode(xmlNode.Name).SelectSingleNode("data").ChildNodes;
                    XmlNodeList nodelist = ((XmlElement)xmlNode).ChildNodes;
                    List<string> Valuues = new List<string> { };
                    foreach (XmlNode Key in nodelist)
                    {
                        string XML_TEXT = "";
                        try { XML_TEXT = Key.InnerText; } catch { }
                        Valuues.Add(XML_TEXT);
                    }
                    Post.Add(xmlNode.Name, Valuues);
                }
                catch
                {
                    string XML_TEXT = "";
                    try { XML_TEXT = xmlNode.InnerText; } catch { }
                    Post.Add(xmlNode.Name, XML_TEXT);
                }
            }
            return Post;
        }

        /*DictionaryToString*/
        public static string DictionaryToString(Dictionary<string, object> Post)
        {
            string Result = "{" + Environment.NewLine;
            foreach (KeyValuePair<string, object> item in Post)
            {
                if (item.Value.GetType() == typeof(string))
                {
                    Result += $@"""{item.Key}"":""{item.Value}""" + Environment.NewLine;
                }
                if (item.Value.GetType() == typeof(List<string>))
                {
                    Result += $@"""{item.Key}"":{{""{string.Join(@""",""", (List<string>)item.Value)}""}}" + Environment.NewLine;
                }
            }
            Result += "}";
            return Result;
        }
    }
}
