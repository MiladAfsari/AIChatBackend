using Shared.Exception.Abstraction;
using Shared.Exception.Abstraction.Domain;

namespace Domain.Core.Entities.UserTemplateAggregate.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string id)
        {
            Id = id;
        }

        [CustomExceptionProperty]
        public string Id { get; }
    }
}
