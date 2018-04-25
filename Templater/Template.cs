using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Templater
{
    public class Template
    {
        private const string WRITE = "output.Write({1}{0}{1});";
        private const string WRITE_LINE = "output.WriteLine({1}{0}{1});";

        private Regex _scriptBracket;
        private Jint.Engine _engine;

        public string FilePath { get; private set; }


        public Template(string filePath)
        {
            FilePath = filePath;
            _scriptBracket = new Regex("<%|%>");
            _engine = new Jint.Engine();
        }


        public void SetValue(object value, string name)
        {
            _engine.SetValue(name, value);
        }

        public object GetValue(string name)
        {
            var value = _engine.GetValue(name);
            if (value == null)
            {
                return null;
            }
            return value.AsObject();
        }

        public byte[] Render(object model = null)
        {
            MemoryStream stream = new MemoryStream();
            Render(stream, model);
            return stream.ToArray();
        }


        public void Render(Stream outStream, object model = null)
        {
            StreamWriter outputWriter = new StreamWriter(outStream, Encoding.UTF8);
            Render(outputWriter, model);
        }

        public void Render(TextWriter output, object model = null)
        {
            string script = LoadScript();
            Console.WriteLine(script);
            _engine.Execute(script);
            _engine.SetValue("output", output);
            _engine.SetValue("model", model);
            _engine.Execute("render()");
            output.Flush();
        }

        private string LoadScript()
        {
            StringBuilder script = new StringBuilder("function render() {\n\n");
            CompileTemplate(script);
            script.Append("\n\n}");
            return script.ToString();
        }

        private void CompileTemplate(StringBuilder output)
        {
            using (StreamReader reader = new StreamReader(FilePath))
            {
                bool insideScript = false;
                string line = reader.ReadLine();
                while (line != null)
                {
                    Convert(output, line, ref insideScript);
                    line = reader.ReadLine();
                }

            }
        }

        private void Convert(StringBuilder converted, string line, ref bool insideScript)
        {
            string escapedLine = line.Replace("'", "\\'");
            var match = _scriptBracket.Match(escapedLine);
            int lastIndex = 0;

            while (match.Success) 
            {
                if (match.Value == "<%") 
                {
                    if (match.Index > 0)
                    {
                        string part = escapedLine.Substring(lastIndex, match.Index - lastIndex);
                        if (part.Length > 0)
                        {
                            converted.AppendLine(string.Format(WRITE, part, "'"));
                        }
                    }
                    insideScript = true;
                } 
                else
                {
                    if (!insideScript)
                    {
                        throw new InvalidOperationException(string.Format("Found closing script tag outside of scripting context.", match.Value));
                    }

                    string part = escapedLine;
                    if (match.Success)
                    {
                        part = escapedLine.Substring(lastIndex, match.Index - lastIndex);
                    }
                    
                    if (part.Trim().StartsWith("="))
                    {
                        converted.AppendLine(string.Format(WRITE, part.Trim().Substring(1).Trim(), ""));
                    }
                    else
                    {
                        converted.AppendLine(part);
                    }
                    insideScript = false;
                }

                lastIndex = match.Index + match.Value.Length;
                match = match.NextMatch();
            }

            if (insideScript)
            {
                converted.AppendLine(escapedLine.Substring(lastIndex));
            }
            else
            {
                converted.AppendLine(
                        string.Format(WRITE_LINE, 
                        escapedLine.Substring(lastIndex), "'"));
            }

        }
    }
}
