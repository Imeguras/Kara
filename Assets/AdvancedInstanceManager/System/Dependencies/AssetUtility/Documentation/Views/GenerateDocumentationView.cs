using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using InstanceManager.Editor;
using UnityEditor;
using UnityEngine;

namespace AssetUtility.Documentation
{

    class GenerateDocumentationView : View
    {

        public override string header { get; } = "Generate documentation";

        public string path = "Assets/Documentation";
        public string[] addedProjects;
        [NonSerialized] public Project[] projects;

        bool isRunning;

        [Serializable]
        public class Project
        {
            public string path;
            public string documentationFile;
            public bool isAdded; //Include in auto generation
        }

        #region Window

        public override void OnEnable()
        {
            context = SynchronizationContext.Current;
            Load();
        }

        public override void OnDisable() =>
            Save();

        public override void OnGUI()
        {

            GUIExt.BeginEnabledScope(!isRunning);

            if (projects is null || projects.Length == 0)
                RefreshProjects();

            EditorGUILayout.HelpBox("Can only be used with Visual Studio", MessageType.Warning);

            EditorGUI.BeginChangeCheck();

            GUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) { padding = new RectOffset(12, 12, 12, 12) });
            EditorGUILayout.LabelField("Projects to generate documentation for:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            if (projects != null)
                foreach (var project in projects)
                    DrawProject(project);
            GUILayout.EndVertical();

            EditorGUILayout.Space();
            path = EditorGUILayout.TextField("Output Folder:", path);

            if (EditorGUI.EndChangeCheck())
                Save();

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate"))
                Generate();

            GUIExt.EndEnabledScope();

        }

        #endregion
        #region Generate

        SynchronizationContext context;
        async void Generate()
        {

            isRunning = true;
            var currentFile = "";
            var i = 0;
            var max = 0;
            var tasks = projects.Where(p => p.isAdded).
                Select(async p =>
                {

                    await GenerateSandCastleFile(p);

                    var docs = ConvertSandcastleToMarkdown(p);
                    if (docs != null)
                        foreach (var type in docs)
                            WriteFile(type.markdown, path + "/" + Path.GetFileNameWithoutExtension(p.path) + "/" + type.type.Name + ".md");

                });

            max = tasks.Count();

            await Task.WhenAll(tasks);
            SandcastleUtility.ClearDocumentation();
            isRunning = false;
            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();

            Task GenerateSandCastleFile(Project project) =>
                Task.Run(() =>
                {

                    Progress(file: project.path);

                    EnableDocumentationFile(project);
                    var msBuild1 = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft Visual Studio"), "msbuild.exe", SearchOption.AllDirectories).FirstOrDefault();
                    var msBuild2 = Directory.GetFiles(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Visual Studio"), "msbuild.exe", SearchOption.AllDirectories).FirstOrDefault();
                    var msBuild = msBuild1 ?? msBuild2;

                    var p = Process.Start(new ProcessStartInfo(msBuild, @"""" + project.path + @"""") { CreateNoWindow = true, WindowStyle = ProcessWindowStyle.Hidden });
                    p.WaitForExit();

                    Progress(step: true);

                });

            void Progress(bool step = false, string file = null) =>
                context.Post(new SendOrPostCallback(_ =>
                {

                    if (!isRunning)
                        return;

                    if (file != null) currentFile = file;
                    if (step) i += 1;
                    EditorUtility.DisplayProgressBar("Generating documentation...", currentFile, (float)i / max);
                    EditorApplication.Step();
                    EditorApplication.QueuePlayerLoopUpdate();

                }), null);

        }

        static string GetTypeName(Type type)
        {

            if (type is null)
                return "void";

            var codeDomProvider = CodeDomProvider.CreateProvider("C#");
            var typeReferenceExpression = new CodeTypeReferenceExpression(new CodeTypeReference(type));
            using (var writer = new StringWriter())
            {
                codeDomProvider.GenerateCodeFromExpression(typeReferenceExpression, writer, new CodeGeneratorOptions());
                return writer.GetStringBuilder().ToString();
            }

        }

        static void WriteFile(string markdown, string file)
        {
            Directory.GetParent(file).Create();
            File.WriteAllText(file, markdown);
        }

        #endregion
        #region Projects

        void DrawProject(Project project)
        {
            var isAdded = EditorGUILayout.Toggle(Path.GetFileNameWithoutExtension(project.path), project.isAdded);
            if (isAdded != project.isAdded)
            {

                if (isAdded)
                {
                    ArrayUtility.Add(ref addedProjects, project.path);
                    EnableDocumentationFile(project);
                }
                else
                    ArrayUtility.Remove(ref addedProjects, project.path);

                project.isAdded = isAdded;

            }
        }

        void RefreshProjects()
        {
            var folder = Directory.GetParent(Application.dataPath);
            projects =
                Directory.GetFiles(folder.FullName, "*.csproj").
                Select(path => new Project()
                {
                    path = path,
                    isAdded = addedProjects?.Contains(path) ?? false,
                    documentationFile = GetDocumentationFile(path)
                }).
            ToArray();
        }

        const string startTag = "<DocumentationFile>";
        const string endTag = "</DocumentationFile>";

        string GetDocumentationFile(string projectPath)
        {

            if (!projectPath.EndsWith(".csproj"))
                return null;

            var file = File.ReadAllText(projectPath);
            if (!file.Contains(startTag))
                return null;

            var s = file.Substring(file.IndexOf(startTag) + startTag.Length);
            s = s.Remove(s.IndexOf(endTag));
            return s;

        }

        void EnableDocumentationFile(Project project)
        {

            if (!string.IsNullOrEmpty(project.documentationFile))
                return;

            var file = File.ReadAllText(project.path);

            //Set to default value when checking toggle in project settings
            project.documentationFile = @"Temp\Bin\Debug\" + Path.GetFileNameWithoutExtension(project.path) + ".xml";

            var startTag = "<PropertyGroup>";
            var endTag = "</PropertyGroup>";

            var startIndex = file.IndexOf(startTag);
            file = file.Insert(startIndex,
                startTag + "\n\t\t" +
                GenerateDocumentationView.startTag + project.documentationFile + GenerateDocumentationView.endTag + "\n" +
                "\t" + endTag + "\n\t");
            File.WriteAllText(project.path, file);

        }

        #endregion
        #region SandcastleToMarkdown

        static (Type type, string markdown)[] ConvertSandcastleToMarkdown(Project project)
        {

            try
            {

                if (!File.Exists(project.documentationFile))
                    return default;

                SandcastleUtility.LoadXmlDocumentation(project.documentationFile);

                var types = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == Path.GetFileNameWithoutExtension(project.path)).ExportedTypes;
                var l = new List<(Type type, string markdown)>();

                foreach (var type in types.Where(t => t != null))
                {

                    var typeDoc = ConvertSandcastleToMarkdown(type?.GetDocumentation(), type);
                    var events = type.GetEvents().Select(e => ConvertSandcastleToMarkdown(e.GetDocumentation(), type, e)).Where(s => !string.IsNullOrWhiteSpace(s));
                    var fields = type.GetFields().Select(f => ConvertSandcastleToMarkdown(f.GetDocumentation(), type, f)).Where(s => !string.IsNullOrWhiteSpace(s));
                    var props = type.GetProperties().Select(p => ConvertSandcastleToMarkdown(p.GetDocumentation(), type, p)).Where(s => !string.IsNullOrWhiteSpace(s));
                    var methods = type.GetMethods().Select(m => ConvertSandcastleToMarkdown(m.GetDocumentation(), type, m)).Where(s => !string.IsNullOrWhiteSpace(s));

                    var markdown = typeDoc;

                    if (events.Any())
                        markdown += "\n\n### Events:\n\n" + string.Join("\n\n", events);

                    if (fields.Any())
                        markdown += "\n\n### Fields:\n\n" + string.Join("\n\n", fields);

                    if (props.Any())
                        markdown += "\n\n### Properties:\n\n" + string.Join("\n\n", props);

                    if (methods.Any())
                        markdown += "\n\n### Methods:\n\n" + string.Join("\n\n", methods);

                    l.Add((type, markdown));

                }

                return l.Where(kvp => !string.IsNullOrWhiteSpace(kvp.markdown)).ToArray();

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }

            return default;

        }

        static string IDToName(string id)
        {

            var name = id;
            if (id.EndsWith(")"))
                name = name.Remove(name.IndexOf('('));

            name = name.Substring(name.LastIndexOf('.') + 1);

            if (id.Contains("("))
                return name + id.Substring(id.IndexOf('('));
            else if (id.StartsWith("M:"))
                return name + "()";
            else
                return name;

        }

        static string ConvertSandcastleToMarkdown(string sandcastleXml, Type type, MemberInfo member = null)
        {

            if (sandcastleXml is null)
                return null;

            var isType = member is null;
            var isEvent = member is EventInfo;
            var isField = member is FieldInfo;
            var isProperty = member is PropertyInfo;
            var isMethod = member is MethodInfo;

            var hasGetter = (member as PropertyInfo)?.CanRead ?? false;
            var hasSetter = (member as PropertyInfo)?.CanWrite ?? (member as FieldInfo)?.IsInitOnly ?? false;
            var returnType = GetTypeName(
                (member as EventInfo)?.EventHandlerType ??
                (member as FieldInfo)?.FieldType ??
                (member as PropertyInfo)?.PropertyType ??
                (member as MethodInfo)?.ReturnType ??
                null);

            var parameters = (member as MethodInfo)?.GetParameters();

            var str = sandcastleXml.
            Replace("<summary>", "\n").Replace("</summary>", "").
            Replace("<para>", "\n\n").Replace("</para>", "").
            Replace("<list type=\"bullet\">", "").Replace("<list type=\"number\">", "").Replace("<list type=\"table\">", "").Replace("<list>", "").Replace("</list>", "\n").
            Replace("<item>", "\n* ").Replace("</item>", "").
            Replace("<term>", "").Replace("</term>", ": ").
            Replace("<description>", "").Replace("</description>", "").
            Replace("!:", "");

            while (str.Contains("<code>"))
            {
                var snippet = str.Remove(str.IndexOf("<code>"));
                //Add 'Example' header to code block
                if (snippet.Last() == '\n')
                    str = str.Replace("<code>", "\n#### Example:\n```csharp\n").Replace("</code>", "\n```\n");
                else
                    str = str.Replace("<code>", "\n```csharp\n").Replace("</code>", "\n```\n\n");
            }

            while (str.Contains("<remarks>"))
            {

                var snippet = str.Substring(str.IndexOf("<remarks>"));
                snippet = snippet.Remove(snippet.IndexOf("</remarks>") + "</remarks>".Length);
                var originalSnippet = snippet;

                snippet = snippet.Replace("<remarks>\n", "\n> ").Replace("<remarks>", "> ").Replace("</remarks>", "\n\n");

                var lines = snippet.Split('\n').Select(l1 => l1.Trim()).ToArray();
                var i2 = 0;
                for (var i1 = 0; i1 < lines.Length; i1++)
                {

                    if (i2 == 2)
                        lines[i1] = ">\n" + ">" + lines[i1];

                    if (string.IsNullOrWhiteSpace(lines[i1]))
                        i2 += 1;
                    else
                        i2 = 0;

                }

                snippet = ">" + string.Join("\n", lines.Where(l2 => l2.Trim().Length > 1));
                snippet = snippet.Replace("\n>", ">").Replace(">", "\n>").TrimEnd('>');

                str = str.Replace(originalSnippet, snippet + "\n");

            }

            str = str.Replace(Environment.NewLine, "").Replace("  ", "");
            str = str.Replace("</member>", "");

            //Header
            var name = member?.Name ?? type.Name;
            var property = (hasGetter ? " get; " : "") + (hasSetter ? " set; " : "");
            if (hasGetter || hasSetter)
                property = " {" + property + "}";

            var method = member is MethodInfo ? "()" : "";
            var isExtensionMethod = ((member as MethodInfo)?.IsDefined(typeof(ExtensionAttribute))) ?? false;
            if (parameters?.Any() ?? false)
                method = "(" + (isExtensionMethod ? "this " : "") +
                    string.Join(", ", parameters?.
                        Select(p => GetTypeName(p.ParameterType) + " " + p.Name + (p.IsOptional ? " = " + (p.DefaultValue?.ToString() ?? "null") : ""))
                        ?? Array.Empty<string>()
                    ) + ")";

            //Hide name if type, since github wiki provides its own (which means our doc viewer does too)
            var header = isType ? "" : ("##### " + returnType + " " + name + property + method);
            str = header + "\n\n" + str;

            //<see cref=""/>
            while (str.Contains("<see cref"))
            {
                var match = Regex.Match(str, pattern: "<see cref=\"(.*)\"?/>");
                if (match.Success)
                    str = str.Replace(match.Groups[0].Value, IDToName(match.Groups[1].Value.Trim().TrimEnd('"')));
            }

            //<see langword=""/>
            while (str.Contains("<see langword"))
            {
                var match = Regex.Match(str, pattern: "<see langword=\"(.*)\"?/>");
                if (match.Success)
                    str = str.Replace(match.Groups[0].Value, IDToName(match.Groups[1].Value.Trim().TrimEnd('"')));
            }

            while (str.Contains("<param name"))
            {
                var match = Regex.Match(str, pattern: "<param name=\"(.*)\">(.*)</param>");
                if (match.Success)
                    str = str.Replace(match.Groups[0].Value, "\n" + match.Groups[1].Value + ": " + match.Groups[2].Value);
            }

            while (str.Contains("<paramref name"))
            {
                var match = Regex.Match(str, pattern: "<paramref name=\"(.*)\"?/>");
                if (match.Success)
                    str = str.Replace(match.Groups[0].Value, IDToName(match.Groups[1].Value.Trim().TrimEnd('"')));
            }

            str = str.Replace("\n", Environment.NewLine);

            if (!isType)
                str = ">" + str.Replace(Environment.NewLine, Environment.NewLine + ">");

            if (isType)
                str = "```csharp\n" +
                      type.FullName +
                      "\n```" +
                      "\n" + str;

            return str;

        }

        #endregion

    }

}
