
namespace N5.Api.Permissions.Create
{
    public record CreatePermissionCommand(string EmployeeForename, string EmployeeSurname, int PermissionTypeId) : ICommand<CreatePermissionsResult>;

    public record CreatePermissionsResult(int Id, string EmployeeForename, string EmployeeSurname, int PermissionTypeId, DateTime PermissionDate);

    public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        public CreatePermissionCommandValidator()
        {
            RuleFor(x => x.EmployeeForename).NotEmpty().WithMessage("EmployeeForename cannot be empty.");
            RuleFor(x => x.EmployeeSurname).NotEmpty().WithMessage("EmployeeSurname cannot be empty.");
            RuleFor(x => x.PermissionTypeId).GreaterThan(0).WithMessage("PermissionTypeId must be greater than zero.");
        }
    }

    public class CreatePermissionCommandHandler(IUnitOfWork unitOfWork, ElasticsearchClient elasticClient, IProducer<Null, string> kafkaProducer)
        : ICommandHandler<CreatePermissionCommand, CreatePermissionsResult>
    {
        public async Task<CreatePermissionsResult> Handle(CreatePermissionCommand command, CancellationToken cancellationToken)
        {
            var permission = new Permission
            {
                EmployeeForename = command.EmployeeForename,
                EmployeeSurname = command.EmployeeSurname,
                PermissionTypeId = command.PermissionTypeId,
                PermissionDate = DateTime.Now
            };

            var existPermission = await unitOfWork.PermissionRepository.ExistAsync(x => x.EmployeeForename == command.EmployeeForename && x.EmployeeSurname == command.EmployeeSurname);

            if (existPermission)
                throw new BusinessException("Permission already exists", HttpStatusCode.BadRequest);

            await unitOfWork.PermissionRepository.CreateAsync(permission, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var generatedId = permission.Id;

            var indexResponse = await elasticClient.IndexAsync(permission, idx => idx.Index("permission"), cancellationToken);
            if(!indexResponse.IsValidResponse)
                throw new BusinessException("Error while indexing permission", HttpStatusCode.InternalServerError);

            var kafkaMessage = new
            {
                Id = Guid.NewGuid(),
                OperationName = "request",
            };

            var messageValue = JsonSerializer.Serialize(kafkaMessage);
            await kafkaProducer.ProduceAsync("permissions-topic", new Message<Null, string> { Value = messageValue }, cancellationToken);
            
            return new CreatePermissionsResult(generatedId, permission.EmployeeForename, permission.EmployeeSurname, permission.PermissionTypeId, permission.PermissionDate);
        }
    }
}
