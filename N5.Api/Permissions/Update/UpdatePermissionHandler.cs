
namespace N5.Api.Permissions.Update
{

    public record UpdatePermissionCommand(string EmployeeForename, string EmployeeSurname, int PermissionTypeId) 
        : ICommand<UpdatePermissionsResult>;

    public record UpdatePermissionCommandWithId(int Id, UpdatePermissionCommand Command)
        : ICommand<UpdatePermissionsResult>;

    public record UpdatePermissionsResult();

    public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
    {
        public UpdatePermissionCommandValidator()
        {
            RuleFor(command => command.EmployeeForename).NotEmpty().WithMessage("The EmployeeForename is required.");
            RuleFor(command => command.EmployeeSurname).NotEmpty().WithMessage("The EmployeeSurname is required.");
            RuleFor(command => command.PermissionTypeId).GreaterThan(0).WithMessage("PermissionTypeId must be greater than zero.");
        }
    }

    internal class UpdatePermissionCommandHandler(IUnitOfWork unitOfWork, ElasticsearchClient elasticClient, IProducer<Null, string> kafkaProducer)
        : ICommandHandler<UpdatePermissionCommandWithId, UpdatePermissionsResult>
    {
        public async Task<UpdatePermissionsResult> Handle(UpdatePermissionCommandWithId commandWithId, CancellationToken cancellationToken)
        {
            var command = commandWithId.Command;
            var id = commandWithId.Id;

            var permission = await unitOfWork.PermissionRepository.GetByIdAsync(id);
            if (permission == null)
                throw new BusinessException($"Permission does not exist {id}", HttpStatusCode.NotFound);

            permission.EmployeeForename = command.EmployeeForename;
            permission.EmployeeSurname = command.EmployeeSurname;
            permission.PermissionTypeId = command.PermissionTypeId;

            await unitOfWork.PermissionRepository.UpdateAsync(permission, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var updateResponse = await elasticClient.IndexAsync(permission, idx =>
            {
                idx.Index("permission").OpType(OpType.Index);
            }, cancellationToken);

            if (!updateResponse.IsValidResponse)
                throw new BusinessException("Error updating in Elasticsearch", HttpStatusCode.BadRequest);

            var kafkaMessage = new
            {
                Id = Guid.NewGuid(),
                OperationName = "modify"
            };

            var messageValue = JsonSerializer.Serialize(kafkaMessage);

            await kafkaProducer.ProduceAsync("permissions-topic", new Message<Null, string> { Value = messageValue }, cancellationToken);
            return new UpdatePermissionsResult();
        }
    }
}
