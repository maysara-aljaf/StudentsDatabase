using StudentsDomain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace StudentsDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            const string file_name = "data.csv";
            var students = new List<Student>();
            using var file = File.OpenText(file_name);
            while (!file.EndOfStream)
            {
                var line = file.ReadLine();
                //if (line == null || line.Length == 0)
                if (string.IsNullOrEmpty(line))// the same think
                    continue;
                var items = line.Split(';');
                if (items.Length < 5)
                    continue;
                var student = new Student
                {
                    LastName = items[0],
                    Name = items[1],
                    patronymic = items[2],
                    DateOfBirth = items[3],
                    Rating = double.Parse(items[4])
                };
                students.Add(student);
            }
            var groups = new Dictionary<int, List<Student>>();
            foreach (var student in students)
            {
                // 24.3987/10 = 2.43987
                // Round (2.43987) = 2
                // 2 * 10 = 20
                var key = (int)(student.Rating / 10) * 10;
                // Math.Round()
                // Math.Floor() -> (int)(student.Rating / 10) * 10;
                // Math.Ceiling()
                if(groups.TryGetValue(key, out var group))
                {
                    group.Add(student);
                }
                else
                {
                    group = new List<Student>();
                    group.Add(student);
                    groups.Add(key, group);
                }
            }
            var data = groups.Select(
                g => new StudentsGroup
            {
                Rating = g.Key,
                Students = g.Value
                    .OrderBy(s => s.Rating)
                    .ToList()
            })
        .OrderBy(g => g.Rating)
        .ToArray();

            const string result_file = "result.txt";

            using var result = File.CreateText(result_file);

            var serializer = new XmlSerializer(typeof(StudentsGroup[]));

            serializer.Serialize(result, data);

        }
    }
}