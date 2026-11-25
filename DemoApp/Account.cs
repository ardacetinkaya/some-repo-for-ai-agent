public record Transaction(DateTimeOffset Date, decimal Amount);

public record AmountRequest(decimal Amount);

public static class Account
{
    private static readonly object Lock = new();
    private static readonly List<Transaction> Transactions = new();
    private static decimal _balance;

    public static decimal Balance
    {
        get { lock (Lock) return _balance; }
    }

    public static decimal Deposit(decimal amount)
    {
        lock (Lock)
        {
            _balance += amount;
            Transactions.Add(new Transaction(DateTimeOffset.UtcNow, amount));
            return _balance;
        }
    }

    public static decimal? Withdraw(decimal amount)
    {
        lock (Lock)
        {
            if (amount > _balance) return null;
            _balance -= amount;
            Transactions.Add(new Transaction(DateTimeOffset.UtcNow, -amount));
            return _balance;
        }
    }

    public static IReadOnlyList<Transaction> GetTransactions()
    {
        lock (Lock) return Transactions.ToArray();
    }

    public static decimal BatchDeposit(decimal[] amounts)
    {
        lock (Lock)
        {
            decimal newBalance = _balance;
            foreach (var amount in amounts)
            {
                newBalance = Deposit(amount);
            }
            return newBalance;
        }
    }
}
