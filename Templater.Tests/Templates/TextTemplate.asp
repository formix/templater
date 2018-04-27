<%

    function tryMe(i) {
        out.WriteLine(i + ") That works!!!");
    }
    
%>
+---------------------------------------------------------+
|<% =align.Center(model.Quote.Title, 57) %>|
+---------------------------+-------+----------+----------+
| Description               |   Qty |    Price |   Amount |
+---------------------------+-------+----------+----------+
<% 
    var subtotal = 0;
    for (var i = 0; i < model.Quote.Lines.Count; i++ ) {
        var line = model.Quote.Lines[i];
        var amount = line.Quantity * line.Price;
        subtotal += amount;
        var quantity = format("0.0", line.Quantity);
        var price = format("$0.00", line.Price);
        var formattedAmount = format("$0.00", amount);

%>| <% =align.Left(line.Description, 25) %> | <% =align.Right(quantity,5) %> | <% =align.Right(price, 8) %> | <% =align.Right(formattedAmount, 8) %> |
+---------------------------+-------+----------+----------+
<%
    }
    var formattedSubtotal = format("$0.00", subtotal)
%>|<% =align.Right("Sub-total", 45) %> | <% =align.Right(formattedSubtotal, 8) %> |
+----------------------------------------------+----------+

<% tryMe(1); %><%
tryMe(2);
%>