using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Templater.Tests.Models;
using System.IO;

namespace Templater.Tests
{
    [TestClass]
    public class TemplateTests
    {
        [TestMethod]
        public void TestRender()
        {
            string templateText = null;
            using (StreamReader sr = new StreamReader("Templates/TextTemplate.asp"))
            {
                templateText = sr.ReadToEnd();
            }

            var template = new Template(templateText);

            template.AddObject("align", new Align());
            template.AddObject("format", (Func<string, decimal, string>)((f, a) => string.Format($"{{0:{f}}}", a)));

            var q = new Quote()
            {
                Title = "Demo Quote"
            };
            q.Lines.Add(new Line() 
            {
                Description = "First item",
                Quantity = 1,
                Price = 10
            });
            q.Lines.Add(new Line()
            {
                Description = "Second item",
                Quantity = 2,
                Price = 20
            });
            q.Lines.Add(new Line()
            {
                Description = "Third item",
                Quantity = 3,
                Price = 30
            });
            q.Lines.Add(new Line()
            {
                Description = "Fourth item",
                Quantity = 4,
                Price = 44
            });

            template.Render(Console.Out, new { Quote = q });

            Console.WriteLine(template.Script);
        }

        [TestMethod]
        public void TestRenderGithubTemplate()
        {
            string templateText = null;
            using (StreamReader sr = new StreamReader("Templates/github-example.txt"))
            {
                templateText = sr.ReadToEnd();
            }
            var template = new Template(templateText);
            TextWriter tw = new StringWriter();
            template.Render(tw);
            string rendered = tw.ToString();
            Console.Write(rendered);
        }
    }
}
