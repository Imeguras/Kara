using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace AssetUtility.Documentation
{
    //TODO: Methods with parameters are ignored (they probably cannot be found when trying to find xml documentation)

    static class SandcastleUtility
    {

        internal static Dictionary<string, string> loadedXmlDocumentation = new Dictionary<string, string>();
        public static void LoadXmlDocumentation(string xmlDocumentation)
        {
            loadedXmlDocumentation.Clear();
            using (var xmlReader = XmlReader.Create(new StringReader(File.ReadAllText(xmlDocumentation))))
            {
                while (xmlReader.Read())
                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "member")
                    {
                        var raw_name = xmlReader["name"];
                        loadedXmlDocumentation[raw_name] = xmlReader.ReadInnerXml();
                    }
            }
        }

        public static void ClearDocumentation() =>
            loadedXmlDocumentation.Clear();

        public static string XmlDocumentationKeyHelper(Type type, MemberInfo member)
        {

            if (type is null)
                return null;

            var key = Regex.Replace(type.FullName, @"\[.*\]", string.Empty).Replace('+', '.');
            if (member != null && type != member)
                key += "." + member.Name;

            if (member is MethodInfo method)
            {

                //Generic parameters
                var typeGenericMap = new Dictionary<string, int>();
                var tempTypeGeneric = 0;
                Array.ForEach(method.DeclaringType.GetGenericArguments(), a => typeGenericMap[a.Name] = tempTypeGeneric += 1);

                var methodGenericMap = new Dictionary<string, int>();
                var tempMethodGeneric = 0;

                Array.ForEach(method.GetGenericArguments(), a => methodGenericMap.Add(a.Name, tempMethodGeneric += 1));

                var l = new List<string>();

                //Parameters
                foreach (var param in method.GetParameters())
                {
                    if (param.ParameterType.HasElementType)
                    {
                        // The type is either an array, pointer, or reference
                        if (param.ParameterType.IsArray)
                        {
                            // Append the "[]" array brackets onto the element type
                            l.Add(param.ParameterType.GetElementType().ToString() + "[]");
                        }
                        else if (param.ParameterType.IsPointer)
                        {
                            // Append the "*" pointer symbol to the element type
                            l.Add(param.ParameterType.GetElementType().ToString() + "*");
                        }
                        else if (param.ParameterType.IsByRef)
                        {
                            // Append the "@" symbol to the element type
                            l.Add(param.ParameterType.GetElementType().ToString() + "@");
                        }
                    }
                    else if (param.ParameterType.IsGenericParameter)
                    {
                        // Look up the index of the generic from the
                        // dictionaries in Figure 5, appending "`" if
                        // the parameter is from a type or "``" if the
                        // parameter is from a method
                        //if (typeGenericMap[param.Name].)
                    }
                    else
                    {
                        // Nothing fancy, just convert the type to a string
                        l.Add(param.ParameterType.ToString());
                    }
                }

                if (l.Any())
                    key += "(" + string.Join(",", l) + ")";

                key = key.Replace("`1[", "{").Replace("]", "}");

            }

            return key;

        }

        static string GetDocumentation(this MemberInfo member, string memberType, Type type = null)
        {
            var key = memberType + ":" + XmlDocumentationKeyHelper(type ?? member.DeclaringType, member);
            loadedXmlDocumentation.TryGetValue(key, out var documentation);
            return documentation;
        }

        public static string GetDocumentation(this ParameterInfo parameterInfo)
        {
            var memberDocumentation = parameterInfo.Member.GetDocumentation();
            if (memberDocumentation != null)
            {
                var match = Regex.Match(memberDocumentation, "<param name=\"" + parameterInfo.Name + "\">(.*)</param>");
                if (match.Success)
                    return match.Value;
            }
            return null;
        }

        public static string GetDocumentation(this MemberInfo memberInfo)
        {

            switch (memberInfo.MemberType)
            {
                //case MemberTypes.Constructor:
                //    return GetDocumentation((ConstructorInfo)memberInfo, xmlDocumentation);
                case MemberTypes.Event:
                    return ((EventInfo)memberInfo).GetDocumentation("E");
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetDocumentation("F");
                case MemberTypes.Method:
                    return ((MethodInfo)memberInfo).GetDocumentation("M");
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetDocumentation("P");
                case MemberTypes.TypeInfo:
                case MemberTypes.NestedType:
                    return ((TypeInfo)memberInfo).GetDocumentation("T", (Type)memberInfo);
                default:
                    return null;
            }

        }

    }

}
