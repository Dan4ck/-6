using System;
using System.Collections.Generic;

namespace CourseRegistrationSystem
{
    public delegate void StudentNotificationHandler(string message);
    public delegate void TeacherNotificationHandler(int registeredStudents);

    public class Student
    {
        public string Name { get; }
        public bool IsVIP { get; }

        public Student(string name, bool isVIP = false)
        {
            Name = name;
            IsVIP = isVIP;
        }

        public void Notify(string message)
        {
            Console.WriteLine($"Студент {Name}: {message}");
        }
    }

    public class Course
    {
        public string Title { get; }
        public int Capacity { get; }
        private int enrolledStudents;
        private List<Student> students;

        public event StudentNotificationHandler OnStudentNotification;
        public event TeacherNotificationHandler OnTeacherNotification;

        public Course(string title, int capacity)
        {
            Title = title;
            Capacity = capacity;
            students = new List<Student>();
            enrolledStudents = 0;
        }

        public void RegisterStudent(Student student)
        {
            if (enrolledStudents >= Capacity && !student.IsVIP)
            {
                OnStudentNotification?.Invoke($"Нет мест на курсе «{Title}»");
                return;
            }

            if (!students.Contains(student))
            {
                students.Add(student);
                enrolledStudents++;
                OnStudentNotification?.Invoke($"Вы успешно записаны на курс «{Title}»");
                OnTeacherNotification?.Invoke(enrolledStudents);
            }
        }

        public void CompleteCourse()
        {
            Console.WriteLine($"Курс «{Title}» завершен.");
            ClearEventHandlers();
        }

        public void ListStudents()
        {
            Console.WriteLine($"Студенты, зарегистрированные на курс «{Title}»:");
            foreach (var student in students)
            {
                Console.WriteLine($"- {student.Name} (VIP: {student.IsVIP})");
            }
        }

        private void ClearEventHandlers()
        {
            OnStudentNotification = null;
            OnTeacherNotification = null;
        }
    }

    public class Teacher
    {
        public string Name { get; }

        public Teacher(string name)
        {
            Name = name;
        }

        public void Notify(int registeredStudents)
        {
            Console.WriteLine($"Преподаватель {Name}: зарегистрировано студентов — {registeredStudents}");
        }
    }

    class Program
    {
        static List<Course> courses = new List<Course>();
        static List<Student> students = new List<Student>();
        static Teacher teacher = new Teacher("Иван Иванович");

        static void Main(string[] args)
        {
            bool running = true;

            courses.Add(new Course("Программирование", 2));
            courses.Add(new Course("Дизайн", 3));
            courses.Add(new Course("Маркетинг", 2));

            while (running)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Добавить студента");
                Console.WriteLine("2. Показать курсы");
                Console.WriteLine("3. Зарегистрировать студента на несколько курсов");
                Console.WriteLine("4. Показать студентов на курсе");
                Console.WriteLine("5. Завершить курсы");
                Console.WriteLine("6. Завершить программу");
                Console.Write("Введите номер действия: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddStudent();
                        break;
                    case "2":
                        ShowCourses();
                        break;
                    case "3":
                        RegisterStudentOnMultipleCourses();
                        break;
                    case "4":
                        ShowStudentsInCourse();
                        break;
                    case "5":
                        CompleteMultipleCourses();
                        break;
                    case "6":
                        running = false;
                        Console.WriteLine("Программа завершена.");
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        static void AddStudent()
        {
            Console.Write("Введите имя студента: ");
            string name = Console.ReadLine();
            Console.Write("VIP-студент? (да/нет): ");
            bool isVIP = Console.ReadLine().ToLower() == "да";
            students.Add(new Student(name, isVIP));
            Console.WriteLine($"Студент {name} добавлен.");
        }

        static void ShowCourses()
        {
            Console.WriteLine("\nДоступные курсы:");
            for (int i = 0; i < courses.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {courses[i].Title} (Вместимость: {courses[i].Capacity})");
            }
        }

        static void RegisterStudentOnMultipleCourses()
        {
            ShowCourses();

            Console.Write("Выберите номера курсов через запятую: ");
            string[] courseNumbers = Console.ReadLine().Split(',');

            Console.WriteLine("\nСписок студентов:");
            for (int i = 0; i < students.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {students[i].Name} (VIP: {students[i].IsVIP})");
            }

            Console.Write("Выберите номер студента: ");
            if (!int.TryParse(Console.ReadLine(), out int studentNumber) || studentNumber < 1 || studentNumber > students.Count)
            {
                Console.WriteLine("Неверный номер студента.");
                return;
            }
            Student selectedStudent = students[studentNumber - 1];

            foreach (string courseNum in courseNumbers)
            {
                if (int.TryParse(courseNum.Trim(), out int courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
                {
                    Course selectedCourse = courses[courseIndex - 1];

                    selectedCourse.OnStudentNotification += selectedStudent.Notify;
                    selectedCourse.OnTeacherNotification += teacher.Notify;

                    selectedCourse.RegisterStudent(selectedStudent);
                }
                else
                {
                    Console.WriteLine($"Неверный номер курса: {courseNum.Trim()}");
                }
            }
        }

        static void ShowStudentsInCourse()
        {
            ShowCourses();

            Console.Write("Выберите номер курса для просмотра студентов: ");
            if (!int.TryParse(Console.ReadLine(), out int courseNumber) || courseNumber < 1 || courseNumber > courses.Count)
            {
                Console.WriteLine("Неверный номер курса.");
                return;
            }

            Course selectedCourse = courses[courseNumber - 1];
            selectedCourse.ListStudents();
        }

        static void CompleteMultipleCourses()
        {
            ShowCourses();

            Console.Write("Выберите номера курсов для завершения через запятую: ");
            string[] courseNumbers = Console.ReadLine().Split(',');

            foreach (string courseNum in courseNumbers)
            {
                if (int.TryParse(courseNum.Trim(), out int courseIndex) && courseIndex > 0 && courseIndex <= courses.Count)
                {
                    Course selectedCourse = courses[courseIndex - 1];
                    selectedCourse.CompleteCourse();
                }
                else
                {
                    Console.WriteLine($"Неверный номер курса: {courseNum.Trim()}");
                }
            }
        }
    }
}
