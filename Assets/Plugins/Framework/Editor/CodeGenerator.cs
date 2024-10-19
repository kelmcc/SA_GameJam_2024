using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Framework
{
    /// <summary>
    /// Utility class to make it easier to generate C# code files.
    /// </summary>
    public static class CodeGenerator
    {
        public enum Scope
        {
            Global,
            Class
        }

        public enum Accessibility
        {
            Public,
            Protected,
            Private
        }

        public const string NEW_LINE = "\n";

        public class CodeDefintion
        {
            public string Code => _code;
            public Scope Scope => _scope;

            private string _code;
            private Scope _scope;

            public CodeDefintion(string code, Scope scope)
            {
                _code = code;
                _scope = scope;
            }

            public void AppendAttribute(string attributeName, params string[] parameterCodeStrings)
            {
                StringBuilder code = new StringBuilder();
                code.Append('[');
                code.Append(attributeName);
                if (parameterCodeStrings.Length > 0)
                {
                    code.Append('(');
                    for (int i = 0; i < parameterCodeStrings.Length; i++)
                    {
                        code.Append(parameterCodeStrings[i]);

                        if (i < parameterCodeStrings.Length - 1)
                        {
                            code.Append(", ");
                        }
                    }
                    code.Append(')');
                }

                code.Append("]\n");
                _code = code + _code;
            }
        }

        public static CodeDefintion CreateEnumDefinition(string name, IList<string> enumNames)
        {
            return CreateEnumDefinition(name, enumNames, null, Scope.Global, Accessibility.Public);
        }

        public static CodeDefintion CreateEnumDefinition(string name, Accessibility accessibility, IList<string> enumNames)
        {
            return CreateEnumDefinition(name, enumNames, null, Scope.Class, accessibility);
        }

        public static CodeDefintion CreateEnumDefinition(string name, IList<string> enumNames, IList<int> enumValues)
        {
            return CreateEnumDefinition(name, enumNames, enumValues, Scope.Global, Accessibility.Public);
        }

        public static CodeDefintion CreateEnumDefinition(string name, Accessibility accessibility, IList<string> enumNames, IList<int> enumValues)
        {
            return CreateEnumDefinition(name, enumNames, enumValues, Scope.Class, accessibility);
        }

        static CodeDefintion CreateEnumDefinition(string name, IList<string> enumNames, IList<int> enumValues, Scope scope, Accessibility accessibility)
        {
            Assert.IsTrue(enumValues == null || enumNames.Count == enumValues.Count);

            StringBuilder code = new StringBuilder(accessibility.ToString().ToLower());
            code.Append(" enum ");
            code.Append(name);
            code.Append(NEW_LINE);
            code.Append("{");
            code.Append(NEW_LINE);

            for (int i = 0; i < enumNames.Count; i++)
            {
                code.Append("\t");
                code.Append(enumNames[i]);
                code.Append(" = ");
                code.Append(enumValues == null ? i : enumValues[i]);
                if (i < enumNames.Count - 1)
                {
                    code.Append(",");
                }
                code.Append(NEW_LINE);
            }

            code.Append("}");

            return new CodeDefintion(code.ToString(), scope);
        }

        public static void CreateSourceFile(string name, params CodeDefintion[] objects)
        {
            CreateSourceFile(name, objects as IList<CodeDefintion>);
        }

        public static void CreateSourceFile(string name, IList<CodeDefintion> objects)
        {
            name = name.Replace(".cs", "");
            string filepath;
            if (!FileUtils.GetAssetFilePath(name + ".cs", out filepath))
            {
                filepath = "Assets/" + name + ".cs";
            }

            StringBuilder code = new StringBuilder();
            code.Append("/* ------------------------ */" + NEW_LINE);
            code.Append("/* ---- AUTO GENERATED ---- */" + NEW_LINE);
            code.Append("/* ---- AVOID TOUCHING ---- */" + NEW_LINE);
            code.Append("/* ------------------------ */" + NEW_LINE);
            code.Append(NEW_LINE);

            code.Append("using UnityEngine;" + NEW_LINE);
            code.Append("using System.Collections.Generic;" + NEW_LINE);
            code.Append("using Framework;" + NEW_LINE);
            code.Append(NEW_LINE);

            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].Scope == Scope.Global)
                {
                    code.Append(objects[i].Code);
                    if (i < objects.Count - 1)
                    {
                        code.Append(NEW_LINE);
                        code.Append(NEW_LINE);
                    }
                }
                else
                {
                    throw new UnityException("Cannot add object definitions that are not at global-level scope to a source file.");
                }
            }

            FileUtils.CreateTextFile(filepath, code.ToString());
        }

        public static CodeDefintion CreateClass(string className, CodeDefintion codeDefinition, bool isStatic, bool isPartial, Scope scope = Scope.Global, Accessibility accessibility = Accessibility.Public)
        {
            return CreateClass(className, new[] { codeDefinition }, isStatic, isPartial, scope, accessibility);
        }

        public static CodeDefintion CreateClass(string className, IList<CodeDefintion> codeDefinitions, bool isStatic, bool isPartial, Scope scope = Scope.Global, Accessibility accessibility = Accessibility.Public)
        {

            StringBuilder code = new StringBuilder(accessibility.ToString().ToLower());
            if (isStatic) code.Append(" static");
            if (isPartial) code.Append(" partial");
            code.Append(" class ");
            code.Append(className);
            code.Append(NEW_LINE);
            code.Append("{");
            code.Append(NEW_LINE);

            for (int i = 0; i < codeDefinitions.Count; i++)
            {
                if (codeDefinitions[i].Scope == Scope.Class)
                {
                    code.Append(NEW_LINE);
                    string[] lines = codeDefinitions[i].Code.Split('\n');
                    for (int j = 0; j < lines.Length; j++)
                    {
                        code.Append("\t");
                        code.Append(lines[j]);
                        code.Append(NEW_LINE);
                    }
                }
                else
                {
                    throw new UnityException("Cannot add object definitions that are not at class-level scope to a class definition.");
                }
            }

            code.Append(NEW_LINE);
            code.Append("}");

            return new CodeDefintion(code.ToString(), scope);
        }

        public static CodeDefintion CreateScriptableEnumConstants(Type type)
        {
            Assert.IsTrue(typeof(ScriptableEnum).IsAssignableFrom(type));

            StringBuilder code = new StringBuilder();
            ScriptableEnum[] values = ScriptableEnum.GetValues(type);

            string typeName = type.Name;
            bool childType = type.BaseType != typeof(ScriptableEnum);


            string allFieldName = "__all" + StringUtils.Pluralise(typeName);

            code.Append("public static ");
            code.Append(typeName);
            code.Append("[] ");
            code.Append("All" + StringUtils.Pluralise(typeName));
            code.Append(" { get { if (");
            code.Append(allFieldName);
            code.Append(" == null) ");
            code.Append(allFieldName);
            code.Append(" = GetValues<");
            code.Append(typeName);
            code.Append(">(); return ");
            code.Append(allFieldName);
            code.Append("; } }");
            code.Append(NEW_LINE);


            for (int i = 0; i < values.Length; i++)
            {
                string fieldName = StringUtils.Variableize(values[i].name, true, true);

                code.Append("public static ");
                if (childType) code.Append("new ");
                code.Append(typeName);
                code.Append(" ");
                code.Append(StringUtils.Santise(values[i].name, false, false));
                code.Append(" { get { if (_");
                code.Append(fieldName);
                code.Append(" == null) _");
                code.Append(fieldName);
                code.Append(" = GetValue<");
                code.Append(typeName);
                code.Append(">(\"");
                code.Append(values[i].name);
                code.Append("\"); return _");
                code.Append(fieldName);
                code.Append("; } }");
                code.Append(NEW_LINE);
            }

            code.Append(NEW_LINE);

            code.Append("protected static ");
            code.Append(typeName);
            code.Append("[] ");
            code.Append(allFieldName);
            code.Append(";");


            code.Append(NEW_LINE);

            for (int i = 0; i < values.Length; i++)
            {

                code.Append("protected static ");
                if (childType) code.Append("new ");
                code.Append(typeName);
                code.Append(" _");
                code.Append(StringUtils.Variableize(values[i].name, true, true));
                code.Append(";");

                if (i < values.Length - 1)
                {
                    code.Append(NEW_LINE);
                }

            }

            return new CodeDefintion(code.ToString(), Scope.Class);
        }

        public static CodeDefintion CreateConstantLayerMasks(IList<string> names, IList<LayerMask> values, bool includeAllAndNothing, bool isStatic = true, Accessibility accessibility = Accessibility.Public)
        {
            Assert.AreEqual(values.Count, names.Count);

            StringBuilder code = new StringBuilder();

            if (includeAllAndNothing)
            {
                code.Append(accessibility.ToString().ToLower());

                if (isStatic) code.Append(" static");

                code.Append(" readonly LayerMask ALL_LAYERS = ~0;");
                code.Append(NEW_LINE);

                code.Append(accessibility.ToString().ToLower());
                if (isStatic)
                {
                    code.Append(" static");
                }
                code.Append(" readonly LayerMask NO_LAYERS = 0;");

                if (names.Count > 0)
                {
                    code.Append(NEW_LINE);
                }
            }

            for (int i = 0; i < names.Count; i++)
            {
                code.Append(accessibility.ToString().ToLower());
                if (isStatic) code.Append(" static");

                code.Append(" readonly LayerMask ");
                code.Append(names[i]);
                code.Append(" = ");
                code.Append(values[i]);
                code.Append(";");

                if (i < names.Count - 1)
                {
                    code.Append(NEW_LINE);
                }
            }

            return new CodeDefintion(code.ToString(), Scope.Class);
        }


        public static CodeDefintion CreateConstantStrings(IList<string> names, IList<string> values, Accessibility accessibility = Accessibility.Public)
        {
            Assert.AreEqual(values.Count, names.Count);

            StringBuilder code = new StringBuilder();

            for (int i = 0; i < names.Count; i++)
            {
                code.Append(accessibility.ToString().ToLower());
                code.Append(" const string ");
                code.Append(names[i]);
                code.Append(" = \"");
                code.Append(values[i]);
                code.Append("\";");

                if (i < names.Count - 1)
                {
                    code.Append(NEW_LINE);
                }
            }

            return new CodeDefintion(code.ToString(), Scope.Class);
        }

        public static CodeDefintion CreateConstantInts(IList<string> names, IList<int> values, Accessibility accessibility = Accessibility.Public)
        {
            Assert.AreEqual(values.Count, names.Count);

            StringBuilder code = new StringBuilder();

            for (int i = 0; i < names.Count; i++)
            {
                code.Append(accessibility.ToString().ToLower());
                code.Append(" const int ");
                if (char.IsNumber(names[i][0]))
                {
                    code.Append('_');
                }
                code.Append(names[i]);
                code.Append(" = ");
                code.Append(values[i]);
                code.Append(";");

                if (i < names.Count - 1)
                {
                    code.Append(NEW_LINE);
                }
            }

            return new CodeDefintion(code.ToString(), Scope.Class);
        }

        public static CodeDefintion CreateArray<T>(string name, IList<T> values, bool isStatic = true, Accessibility accessibility = Accessibility.Public)
        {
            bool isString = typeof(T) == typeof(string);

            StringBuilder code = new StringBuilder();
            code.Append(accessibility.ToString().ToLower());

            if (isStatic)
            {
                code.Append(" static");
            }

            code.Append(" ");
            code.Append(typeof(T));
            code.Append("[] ");
            code.Append(name);
            code.Append(" = {");
            code.Append(NEW_LINE);

            for (int i = 0; i < values.Count; i++)
            {
                code.Append("\t");
                if (isString) code.Append("\"");
                code.Append(values[i]);
                if (isString) code.Append("\"");

                if (i < values.Count - 1)
                {
                    code.Append(",");
                }

                code.Append(NEW_LINE);
            }

            code.Append("};");

            return new CodeDefintion(code.ToString(), Scope.Class);
        }


    }
}
