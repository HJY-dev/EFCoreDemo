using DbContents;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace 多对多
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region 初始化数据
            using (MyDbContext ctx = new MyDbContext ())
            {
                if (await ctx.Students.CountAsync() == 0)
                {
                    Student stu1 = new Student { Name = "张三" };
                    Student stu2 = new Student { Name = "李四" };
                    Student stu3 = new Student { Name = "王五" };

                    Teacher t1 = new Teacher { Name = "Tom" };
                    Teacher t2 = new Teacher { Name = "Jack" };
                    Teacher t3 = new Teacher { Name = "Alice" };

                    stu1.Teachers.Add(t1);
                    stu1.Teachers.Add(t2);

                    stu2.Teachers.Add(t2);
                    stu2.Teachers.Add(t3);

                    stu3.Teachers.Add(t1);
                    stu3.Teachers.Add(t2);
                    stu3.Teachers.Add(t3);

                    ctx.Students.Add(stu1);
                    ctx.Students.Add(stu2);
                    ctx.Students.Add(stu3);

                    ctx.SaveChanges();
                }
                
            }
            #endregion

            #region FluentApi查询
            using (MyDbContext ctx = new MyDbContext())
            {
                var result = await ctx.Students.Include(x => x.Teachers).ToListAsync();
                foreach (var item in result)
                {
                    Console.WriteLine($"{item.Name}-{item.Teachers.Count}");
                    foreach (var t in item.Teachers)
                    {
                        Console.WriteLine("\t" + t.Name);
                    }
                }
            }
            #endregion

            #region Linq查询
            using (MyDbContext ctx = new MyDbContext())
            {
                var result = from t in ctx.Students
                             join relation in ctx.T_Students_Teacherss
                             on t.Id equals relation.StudentsId
                             select new { 
                                Students= t,
                                Teachers = ctx.Teachers.Where(x=>x.Id == relation.TeachersId).ToList()
                             };
                foreach (var item in result)
                {
                    Console.WriteLine($"{item.Students.Name}-{item.Teachers.Count}");
                    foreach (var t in item.Teachers)
                    {
                        Console.WriteLine("\t" + t.Name);
                    }
                }

            }
            #endregion

            #region Dapper查询
            using (MyDbContext ctx = new MyDbContext())
            {

            }
            #endregion


            Console.ReadKey();
        }
    }
}
