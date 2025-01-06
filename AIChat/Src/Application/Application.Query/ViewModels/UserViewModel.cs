namespace Application.Query.ViewModels
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public short DepartmentId { get; set; }
    }
}
