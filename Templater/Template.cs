using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Templater
{
    public class Template
    {
        private const string WRITE = "output.Write({1}{0}{1});";
        private const string WRITE_LINE = "output.WriteLine({1}{0}{1});";

        private Regex scriptBracket;
        private Jint.Engine engine;

        public string FilePath { get; private set; }


        public Template(string filePath)
        {
            this.FilePath = filePath;
            this.scriptBracket = new Regex("<%|%>");
            this.engine = null;
        }


        public byte[] Render(object model = null)
        {
            MemoryStream stream = new MemoryStream();
            this.Render(stream, model);
            return stream.ToArray();
        }


        public void Render(Stream stream, object model = null)
        {
            StreamWriter outputWriter = new StreamWriter(stream, Encoding.UTF8);
            this.Render(outputWriter, model);
        }

        public void Render(TextWriter output, object model = null)
        {
            if (this.engine == null)
            {
                string script = this.LoadScript();
                this.engine = new Jint.Engine();
                Console.WriteLine(script);
                this.engine.Execute(script);
            }
            this.engine.SetValue("output", output);
            this.engine.SetValue("model", model);
            this.engine.Execute("render()");
            output.Flush();
        }

        private string LoadScript()
        {
            StringBuilder script = new StringBuilder("function render() {\n\n");
            this.CompileTemplate(script);
            script.Append("\n\n}");
            return script.ToString();
        }

        private void CompileTemplate(StringBuilder output)
        {
            using (StreamReader reader = new StreamReader(this.FilePath))
            {
                bool insideScript = false;
                string line = reader.ReadLine();
                while (line != null)
                {
                    this.Convert(output, line, ref insideScript);
                    line = reader.ReadLine();
                }

            }
        }

        private void Convert(StringBuilder converted, string line, ref bool insideScript)
        {
            string escapedLine = line.Replace("'", "\\'");
            var match = this.scriptBracket.Match(escapedLine);
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
