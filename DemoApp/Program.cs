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

account.MapGet("/transactions", () => Results.Ok(Account.GetTransactions()));

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

account.MapPost("/deposits/batch", (decimal[] amounts) =>
{
    if (amounts == null || amounts.Length == 0 || amounts.Length > 20)
        return Results.BadRequest("Invalid batch.");

    var newBalance = Account.BatchDeposit(amounts);
    return Results.Ok(new { balance = newBalance });
});

app.Run();
