using BCrypt.Net;
namespace Student_Api_Project.DataSimulation
{
    public class StudentDataSimulation
    {
        public static readonly List<Model.Student> Students = new List<Model.Student>
        {
            new Model.Student {
                Id = 1,
                Name = "Ahmed",
                Age = 20,
                Grade = 90 ,
                Email="Ahmed@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ahmed123"),
                Role="Student"

            },
            new Model.Student {
                Id = 2,
                Name = "Ali",
                Age = 22,
                Grade = 85,
                Email="Ali@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Ali123"),
                Role="Student"

            },
            new Model.Student {
                Id = 3,
                Name = "Faud",
                Age = 21,
                Grade = 88,
                Email="Faud@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Faud123"),
                Role="Student"
            },
            new Model.Student {
                Id = 4,
                Name = "Diana",
                Age = 23,
                Grade = 92,
                Email="Diana@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Diana123"),
                Role="Admin"
            },
            new Model.Student {
                Id = 5,
                Name = "Zena",
                Age = 20,
                Grade = 87,
                Email="Zana@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Zana123"),
                Role="Student"
            }
        };
    }
}
