
using Azure;

namespace N5.Api.Permissions.Get
{
    public record GetPermissionsQuery() : IQuery<GetPermissionsResult>;
    public record GetPermissionsResult(IEnumerable<Permission> Permissions);

    internal class GetPermissionQueryHandler(ElasticsearchClient elasticClient, IProducer<Null, string> kafkaProducer)
        : IQueryHandler<GetPermissionsQuery, GetPermissionsResult>
    {
        public async Task<GetPermissionsResult> Handle(GetPermissionsQuery query, CancellationToken cancellationToken)
        {
            //var permissions = await unitOfWork.PermissionRepository.GetAllAsync(cancellationToken);
            //return new GetPermissionsResult(permissions);

            // Realizar la búsqueda en Elasticsearch
            var searchResponse = await elasticClient.SearchAsync<Permission>(s => s
                .Index("permission"), cancellationToken);

            var kafkaMessage = new
            {
                Id = Guid.NewGuid(),
                OperationName = "get",
            };

            var messageValue = JsonSerializer.Serialize(kafkaMessage);
            await kafkaProducer.ProduceAsync("permissions-topic", new Message<Null, string> { Value = messageValue }, cancellationToken);

            return searchResponse.IsValidResponse ? new GetPermissionsResult(searchResponse.Documents.ToList()) : default;
        }
    }
}
