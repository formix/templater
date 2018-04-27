using Jint.Native;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Templater
{
    public class Template
    {
        private const string WRITE = "out.Write({1}{0}{1});";
        private const string WRITE_LINE = "out.WriteLine({1}{0}{1});";

        private Jint.Engine _engine;
        private string _script;
        private bool _scriptLoaded;

        public string Text { get; private set; }
        public string Script
        {
            get
            {
                if (_script == null)
                {
                    _script = BuildScript();
                }
                return _script;
            }
        }

        public Template(string text)
        {
            Text = text;
            _script = null;
            _scriptLoaded = false;
            _engine = new Jint.Engine();
        }


        public void AddObject<T>(string name, T value)
        {
            _engine.SetValue(name, value);
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
            if (!_scriptLoaded)
            {
                _engine.Execute(Script);
                _engine.SetValue("out", output);
                _scriptLoaded = true;
            }
            _engine.SetValue("model", model);
            _engine.Execute("__render__()");
            output.Flush();
        }

        private string BuildScript()
        {
            StringBuilder script = new StringBuilder("function __render__() {\n\n");
            CompileTemplate(script);
            script.Append("\n\n}");
            return script.ToString();
        }

        private void CompileTemplate(StringBuilder output)
        {
            using (TextReader reader = new StringReader(Text))
            {
                bool insideScript = false;
                string line = reader.ReadLine();
                while (line != null)
                {
                    Convert(output, line, ref insideScript);
                    line = reader.ReadLine();
                }

                if (insideScript)
                {
                    throw new ClosingTagNotFoundException();
                }
            }
        }

        private void Convert(StringBuilder converted, string line, ref bool insideScript)
        {
            string escapedLine = line.Replace("'", "\\'");
            var scriptBracket = new Regex("<%|%>");
            var match = scriptBracket.Match(escapedLine);
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
