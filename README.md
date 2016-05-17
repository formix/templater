templater
=========

Template absolutely any text file using this simple ASP like Javascript 
templating engine.

## Usage

### Without model

```C#
    var template = new Template("Templates/github-example.txt");
    template.Render(Console.Out);  // writes to any TextWriter
```

### With a model

You can render using a model. The `model` object is then available to the 
script.

```C#
    var template = new Template("Templates/github-example.txt");
    template.Render(Console.Out, new {
        FirstName = "John",
        LastName = "Doe",
        Age = 30
    });
```

When the render method is called for the first time, the template is compiled 
to a script that is reused if further calls to `Render` is made with different 
models.

The `output` object is the `System.IO.TextWriter` that is passed to the 
`Render` method.

## Template example

*github-example.txt*
```ASP
<%
    var x = 5 * 3;
%>
The result is: '<% =x %>'

<% for (var i = 0; i < 3; i++) { %>
Hello for the <%
    if (i === 0) {
        output.Write("first");
    } else if (i === 1) {
        output.Write("second");
    } else if (i === 2) {
        output.Write("third");
    }
%> time! <% 
} %>

Thanks
```

This will produce the following text file:

```

The result is: '15'


Hello for the first time! 
Hello for the second time! 
Hello for the third time! 

Thanks
```

