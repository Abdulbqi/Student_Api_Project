using Microsoft.AspNetCore.Authorization;
namespace Student_Api_Project.Authorization
{
    public class StudentOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
