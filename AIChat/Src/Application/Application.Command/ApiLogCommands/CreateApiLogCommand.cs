using Domain.Core.ApiLog;
using Domain.Core.UnitOfWorkContracts;
using MediatR;

namespace Application.Command.ApiLogCommands
{
    public class CreateApiLogCommand : IRequest<Unit>
    {
        public string? ControllerName { get; }
        public string? RequestHeaders { get; }
        public string? AccessToken { get; }
        public string? MethodInput { get; }
        public string? MethodOutput { get; }
        public int ResultCode { get; }
        public string? ResultMessage { get; }
        public DateTime DateTime { get; }
        public string? IP { get; }
        public double ResponseTime { get; }

        public CreateApiLogCommand(
            string? controllerName,
            string? requestHeaders,
            string? accessToken,
            string? methodInput,
            string? methodOutput,
            int resultCode,
            string? resultMessage,
            DateTime dateTime,
            string? ip,
            double responseTime)
        {
            ControllerName = controllerName;
            RequestHeaders = requestHeaders;
            AccessToken = accessToken;
            MethodInput = methodInput;
            MethodOutput = methodOutput;
            ResultCode = resultCode;
            ResultMessage = resultMessage;
            DateTime = dateTime;
            IP = ip;
            ResponseTime = responseTime;
        }
    }

    public class CreateApiLogCommandHandler : IRequestHandler<CreateApiLogCommand, Unit>
    {
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;
        private readonly IApiLogRepository _apiLogRepository;

        public CreateApiLogCommandHandler(IApplicationDbContextUnitOfWork unitOfWork, IApiLogRepository apiLogRepository)
        {
            _unitOfWork = unitOfWork;
            _apiLogRepository = apiLogRepository;
        }

        public async Task<Unit> Handle(CreateApiLogCommand request, CancellationToken cancellationToken)
        {
            var apiLog = new ApiLog()
            {
                ControllerName = request.ControllerName,
                RequestHeaders = request.RequestHeaders,
                AccessToken = request.AccessToken,
                MethodInput = request.MethodInput,
                MethodOutput = request.MethodOutput,
                ResultCode = request.ResultCode,
                ResultMessage = request.ResultMessage,
                DateTime = request.DateTime,
                IP = request.IP,
                ResponseTime = request.ResponseTime
            };
            await _apiLogRepository.CreateApiLog(apiLog, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
