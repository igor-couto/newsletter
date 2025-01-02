using System.Data;
using Dapper;
using NewsletterSDK.Models;

namespace NewsletterSDK.Repositories;

internal class PublicationRepository(IDbConnection dbConnection)
{
    private readonly IDbConnection _dbConnection = dbConnection;

    internal async Task<IEnumerable<Publication>> ReadAll(CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT 
                id AS Id,
                title AS Title,
                content AS Content,
                status_id AS PublicationStatus,
                created_at AS CreatedAt,
                sending_date AS SendingDate
            FROM publications;
        ";

        var publications = await _dbConnection.QueryAsync<Publication>(query, cancellationToken);

        return publications;
    }

    internal async Task<(IEnumerable<Publication> Publications, int Total)> ReadFiltered(
        PublicationStatus? status,
        string? title,
        int page,
        int limit)
    {
        var baseQuery = $@"
            FROM publications
            WHERE (@status IS NULL OR status_id = @status) AND status_id != {(short) PublicationStatus.Deleted}
            AND (@title IS NULL OR title ILIKE '%' || @title || '%')
        ";

        var countQuery = @"
            SELECT COUNT(*) " + baseQuery + @";
        ";

        var dataQuery = @"
            SELECT 
                id AS Id,
                title AS Title,
                content AS Content,
                status_id AS PublicationStatus,
                created_at AS CreatedAt,
                sending_date AS SendingDate
            " + baseQuery + @"
            ORDER BY created_at DESC
            LIMIT @limit OFFSET @offset;
        ";

        var parameters = new
        {
            status = status == null ? (short?) null : (short) status,
            title = string.IsNullOrWhiteSpace(title) ? null : title,
            limit,
            offset = (page - 1) * limit
        };

        var total = await _dbConnection.ExecuteScalarAsync<int>(countQuery, parameters);
        var publications = await _dbConnection.QueryAsync<Publication>(dataQuery, parameters);

        return (publications, total);
    }
    
    internal async Task<IEnumerable<Publication>> ReadUnsent(CancellationToken cancellationToken)
    {
        const string query = @"
            SELECT
                id AS Id,
                title AS Title,
                content AS Content,
                status_id AS PublicationStatus,
                created_at AS CreatedAt,
                sending_date AS SendingDate
            FROM publications
            WHERE status_id = 0
            ORDER BY sending_date ASC;
        ";

        var publications = await _dbConnection.QueryAsync<Publication>(query, cancellationToken);

        return publications;
    }

    internal async Task<bool> Insert(Publication publication, CancellationToken cancellationToken)
    {
        const string command = 
            @"INSERT INTO 
                publications(id, title, content, status_id, created_at, sending_date)
                VALUES(@Id, @Title, @Content, @PublicationStatus, @CreatedAt, @SendingDate);";

        var affectedRows = await _dbConnection.ExecuteAsync(new CommandDefinition(command, publication, cancellationToken: cancellationToken));           
        return affectedRows == 1;
    }

    internal async Task<bool> UpdatePublication(Publication publication, CancellationToken cancellationToken)
    {
        const string command = 
            @"UPDATE publications
                SET 
                    title = @Title,
                    content = @Content,
                    status_id = @PublicationStatus,
                    sending_date = @SendingDate
                WHERE id = @Id;";

        var affectedRows = await _dbConnection.ExecuteAsync(new CommandDefinition(command, publication, cancellationToken: cancellationToken));           
        return affectedRows > 0;
    }

    internal async Task Delete(Guid id)
    {
        var status = await _dbConnection.QuerySingleOrDefaultAsync<short?>(
            @"SELECT status_id FROM publications WHERE id = @Id",
            new { Id = id }
        );

        if (status is null)
            throw new KeyNotFoundException($"Publication with ID {id} was not found.");

        if (status.Value != (short)PublicationStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Publication with ID {id} is in status {status}, not Pending."
            );
        }

        var parameters = new
        {
            Id = id,
            PublicationStatus = (short)PublicationStatus.Deleted,
            DeletedAt = DateTime.UtcNow
        };

        var command =
            $@"UPDATE publications
            SET status_id = @PublicationStatus, deleted_at = @DeletedAt
            WHERE id = @Id;";

        var affectedRows = await _dbConnection.ExecuteAsync(command, parameters);

        if (affectedRows == 0)
            throw new Exception("Publication was removed between the status check and this update.");
    }
}