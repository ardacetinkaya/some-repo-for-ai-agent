var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var account = app.MapGroup("/account");

account.MapGet("/balance", () => Results.Ok(new { balance = Account.Balance }));

account.MapGet("/transactions", (string? type) =>
{
    if (type!= null && (type.ToLower()!= "deposit" && type.ToLower()!= "withdrawal"))
        return Results.StatusCode(400, "Invalid type");

    var filteredTransactions = Account.GetTransactions().Where(t =>
        (type == null || (type.ToLower() == "deposit" && t.Amount > 0) || (type.ToLower() == "withdrawal" && t.Amount < 0)));

    return Results.Ok(filteredTransactions);
});

account.MapPost("/deposit", (AmountRequest request) =>
{
    if (request.Amount <= 0)
        return Results.BadRequest("Amount must be greater than zero.");

    return Results.Ok(new { balance = Account.Deposit(request.Amount) });
});

account.MapPost("/withdraw", (AmountRequest request) =>
{
    if (request.Amount <= 0)
        return Results.BadRequest("Amount must be greater than zero.");

    var balance = Account.Withdraw(request.Amount);
    return balance is null
        ? Results.BadRequest("Insufficient funds.")
        : Results.Ok(new { balance });
});

app.Run();