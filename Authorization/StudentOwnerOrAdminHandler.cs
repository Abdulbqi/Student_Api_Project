using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Student_Api_Project.Authorization
{
    public class StudentOwnerOrAdminHandler : AuthorizationHandler <StudentOwnerOrAdminRequirement,int> 
    {
       protected override Task HandleRequirementAsync(AuthorizationHandlerContext context , StudentOwnerOrAdminRequirement requirment,int StudentId)
       {
            if(context.User.IsInRole("Admin"))
            {
                context.Succeed(requirment);
                return Task.CompletedTask;
            }
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int authenticatedStudentId) &&
           authenticatedStudentId == StudentId)
            {
                context.Succeed(requirment);
               
            }
            return Task.CompletedTask;
        }
    }
}
