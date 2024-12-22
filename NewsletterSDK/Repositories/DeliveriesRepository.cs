using System.Data;
using Dapper;
using NewsletterSDK.Models;

namespace NewsletterSDK.Repositories;

internal class DeliveriesRepository(IDbConnection dbConnection)
{
    private readonly IDbConnection _dbConnection = dbConnection;

    internal async Task<bool> Insert(PublicationDelivery publicationDelivery, CancellationToken cancellationToken)
    {
        const string command = 
            @"INSERT INTO 
                publication_deliveries(subscriber_email, publication_id, status_id, sending_date)
                VALUES(@SubscriberEmail, @PublicationId, @DeliveryStatus, @SendingDate);";

        var success = await _dbConnection.ExecuteAsync(new CommandDefinition(command, publicationDelivery, cancellationToken: cancellationToken));           
        return success > 0;
    }
}