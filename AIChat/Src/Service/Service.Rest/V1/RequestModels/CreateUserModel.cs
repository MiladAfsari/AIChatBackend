namespace Service.Rest.V1.RequestModels
{
    public class CreateUserModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string FullName { get; set; }
        public short DepartmentId { get; set; }
    }
}
