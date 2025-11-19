namespace Student_Api_Project.DataSimulation
{
    public class StudentDataSimulation
    {
        public static readonly List<Model.Student> Students = new List<Model.Student>
        {
            new Model.Student { Id = 1, Name = "Ahmed", Age = 20, Grade = 90 },
            new Model.Student { Id = 2, Name = "Ali", Age = 22, Grade = 85 },
            new Model.Student { Id = 3, Name = "Faud", Age = 21, Grade = 88 },
            new Model.Student { Id = 4, Name = "Diana", Age = 23, Grade = 92 },
            new Model.Student { Id = 5, Name = "Zena", Age = 20, Grade = 87 }
        };
    }
}
