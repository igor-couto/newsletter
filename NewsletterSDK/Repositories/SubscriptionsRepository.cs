using System.Data;
using System.Runtime.InteropServices;
using Dapper;
using NewsletterSDK.Models;

namespace NewsletterSDK.Repositories;

internal class SubscriptionsRepository(IDbConnection dbConnection)
{
    private readonly IDbConnection _dbConnection = dbConnection;
    
    internal async Task<IEnumerable<Subscriber>> ReadAllSubscribers(CancellationToken cancellationToken)
    {
        const string query = "SELECT email AS Email, name AS Name, created_at AS CreatedAt FROM subscribers;";

        return await _dbConnection.QueryAsync<Subscriber>(query, cancellationToken);
    }

    internal async Task<Subscriber?> FindByEmail(string email, CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT
                email AS Email,
                name AS Name,
                created_at AS CreatedAt
            FROM subscribers 
            WHERE email = @Email;";

        var parameters = new { Email = email };
            
        return await _dbConnection
            .QuerySingleOrDefaultAsync<Subscriber>(new CommandDefinition(query, parameters, cancellationToken: cancellationToken));
    }

    internal async Task<bool> SubscriberExists(string email)
    {
        const string query = "SELECT EXISTS(SELECT 1 FROM subscribers WHERE email = @Email);";

        return await _dbConnection.ExecuteScalarAsync<bool>(query, new { Email = email });
    }

    internal async Task<bool> Insert(Subscriber subscriber, CancellationToken cancellationToken)
    {
        const string command = @"INSERT INTO subscribers (email, name) VALUES (@Email, @Name);";
;
        var success = await _dbConnection.ExecuteAsync(new CommandDefinition(command, subscriber, cancellationToken: cancellationToken));           
        return success > 0;
    }

    //TODO: Refactor this method, maybe
    internal async Task<bool> Unsubscribe(Subscriber subscriber, string? reason)
    {
        if (_dbConnection.State == ConnectionState.Closed)
            _dbConnection.Open();
        
        using var transaction = _dbConnection.BeginTransaction();

        try
        {
            const string deleteCommand = "DELETE FROM subscribers WHERE email = @Email;";

            var deleteResult = await _dbConnection.ExecuteAsync(deleteCommand, new { subscriber.Email }, transaction);
            if(deleteResult <= 0) 
                throw new Exception("Failed to delete subscriber");

            const string insertCommand = 
            @"INSERT INTO 
                unsubscriptions(email, name, subscribed_at, unsubscribed_at, reason)
                VALUES(@Email, @Name, @SubscribedAt, @UnsubscribedAt, @Reason);";

            var parameters = new
            {
                subscriber.Email,
                subscriber.Name,
                SubscribedAt = subscriber.CreatedAt,
                UnsubscribedAt = DateTime.UtcNow,
                Reason = reason
            };

            var result = await _dbConnection.ExecuteAsync(insertCommand, parameters, transaction);
            if(result <= 0) 
                throw new Exception("Failed to insert new unsubscrition");

            transaction.Commit();
            return true;
        }
        catch (Exception exception)
        {
            transaction.Rollback();
            Console.WriteLine(exception.Message);
            return false;
        }
    }

    internal async Task<IEnumerable<Unsubscription>> ReadAllUnsubscribed(CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT 
                email AS Email,
                name AS Name,
                subscribed_at AS SubscribedAt,
                unsubscribed_at AS UnsubscribedAt,
                reason AS Reason
            FROM unsubscriptions;";

        return await _dbConnection.QueryAsync<Unsubscription>(query, cancellationToken);
    }

    internal async Task<(IEnumerable<Subscriber> subscribers, int total)> ReadAllSubscribersPaginated(int page, int limit)
    {
        const string countQuery = "SELECT COUNT(*) FROM subscribers;";

        const string dataQuery = @"
            SELECT 
                email AS Email,
                name AS Name,
                created_at AS CreatedAt
            FROM subscribers
            ORDER BY created_at DESC
            LIMIT @Offset OFFSET @offset;
        ";

        var parameters = new
        { 
            Offset = (page - 1) * limit,
            Limit = limit 
        };

        var total = await _dbConnection.ExecuteScalarAsync<int>(countQuery);
        var subscribers = await _dbConnection.QueryAsync<Subscriber>(dataQuery, parameters);

        return (subscribers, total);
    }
}