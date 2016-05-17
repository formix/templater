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
            var template = new Template("Templates/TextTemplate.asp");
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
        }

        [TestMethod]
        public void TestRenderGithubTemplate()
        {
            var template = new Template("Templates/github-example.txt");
            TextWriter tw = new StringWriter();
            template.Render(tw);
            string rendered = tw.ToString();
            Console.Write(rendered);
        }
    }
}
