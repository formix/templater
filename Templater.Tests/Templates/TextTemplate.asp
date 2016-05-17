

# Quote Title: <% =model.Quote.Title %>

Lines

Description                 Quantity    Price   Amount
<% 
    var subtotal = 0;
    for (var i = 0; i < model.Quote.Lines.Count; i++ ) {
        var line = model.Quote.Lines[i];
        var amount = line.Quantity * line.Price;
        subtotal += amount;
%>
<% =line.Description %> <% =line.Quantity %> <% =line.Price %> <% =amount %>
<%
    }
%>

Sub-total: <% =subtotal %> 