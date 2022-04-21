using DbContents;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;
using System.Threading;

namespace 复杂查询
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 组织结构树
            using(MyDbContext ctx = new MyDbContext ())
            {
                Organization o1 = new Organization { Name = "科技集团" };
                Organization o2 = new Organization { Name = "总经办" };
                Organization o3 = new Organization { Name = "技术部" };
                Organization o4 = new Organization { Name = "测试部" };
                Organization o5 = new Organization { Name = "人力资源部" };
                Organization o6 = new Organization { Name = "前端" };
                Organization o7 = new Organization { Name = "后端" };


                ctx.Organizations.Add (o1);
                ctx.SaveChanges();
            }


            #endregion

            #region 状态跟踪妙用

            using (MyDbContext ctx = new MyDbContext()) 
            { 
                var items = ctx.Organizations.Take(3).ToArray();
                var item1 = items[0];
                var item2 = items[1];
                var item3 = items[2];
                var item4 = new Organization { Name = "" };
                var item5 = new Organization { Name = "" };

                item1.Name = "测试";
                ctx.Organizations.Remove (item2);
                ctx.Organizations.Add(item4);

                EntityEntry e1 = ctx.Entry(item1);
                EntityEntry e2 = ctx.Entry(item2);
                EntityEntry e3 = ctx.Entry(item3);
                EntityEntry e4 = ctx.Entry(item4);
                EntityEntry e5 = ctx.Entry(item5);

                Console.WriteLine(e1.State);
                Console.WriteLine(e1.DebugView.LongView);
                Console.WriteLine(e2.State);
                Console.WriteLine(e3.State);
                Console.WriteLine(e4.State);
                Console.WriteLine(e5.State);
            }

            using (MyDbContext ctx = new MyDbContext())
            {
                var items = ctx.Organizations.Take(3).AsNoTracking().ToArray();
            }

            using (MyDbContext ctx = new MyDbContext ()) 
            {
                //修改
                Organization _updateentity = new Organization { Id = 1, Name = "科技集团公司" };
                var entry = ctx.Entry(_updateentity);
                entry.Property(_updateentity.Name).IsModified = true;
                Console.WriteLine(entry.DebugView.LongView);
                ctx.SaveChanges ();

                //删除
                Organization _delentity = new Organization { Id = 10};
                ctx.Entry(_delentity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                ctx.SaveChanges ();
            }


            #endregion

            #region 批量操作
            using (MyDbContext ctx = new MyDbContext ()) 
            {
                int delResult = ctx.DeleteRange<Organization>(b => b.Name.Contains("测试") || b.Name == "暂定部门");
                
                int updateResult = ctx.BatchUpdate<Organization>()
                    .Set(b => b.Name, b=>b.Name+"修改")
                    .Where(b => b.Id ==1)
                    .Execute();

            }

            #endregion

            #region 全局查询筛选器
            using (MyDbContext ctx = new MyDbContext ()) 
            {
               var result = ctx.Organizations.IgnoreQueryFilters().Where(b => b.Name.Contains("公司")).ToList();
            }

            #endregion

            #region EFCore 并发控制
            //模拟demo
            using (MyDbContext ctx = new MyDbContext())
            {
                Console.WriteLine("请输入预定房间号：");
                string roomNo = Console.ReadLine ();
                Console.WriteLine("请输入您的名字：");
                string customName = Console.ReadLine();

                var searchResult = ctx.Houses.Single(x => x.Id == Convert.ToInt64(roomNo));
                if(!string.IsNullOrEmpty(searchResult.Owner))
                {
                    if(searchResult.Owner == customName)
                    {
                        Console.WriteLine("房子已经被你预定成功");
                    }
                    else
                    {
                        Console.WriteLine($"房子已经被 【{searchResult.Owner}】占用");
                    }
                    return;
                }
                searchResult.Owner = customName;
                //模拟并发操作
                Thread.Sleep(100000);
                ctx.SaveChanges();
                Console.WriteLine($"恭喜你 【{customName}】，抢到了");
            }

            //悲观锁行锁方式处理
            using(MyDbContext ctx = new MyDbContext ())
            {
                using (var tx =ctx.Database.BeginTransaction())
                {
                    Console.WriteLine("请输入预定房间号：");
                    string roomNo = Console.ReadLine();
                    Console.WriteLine("请输入您的名字：");
                    string customName = Console.ReadLine();
                    //添加行锁（每种数据库可能不一致）
                    var searchResult = ctx.Houses.FromSqlInterpolated($"select * from T_Houses where \"Id\" = {roomNo} for update").Single();
                    if (!string.IsNullOrEmpty(searchResult.Owner))
                    {
                        if (searchResult.Owner == customName)
                        {
                            Console.WriteLine("房子已经被你预定成功");
                        }
                        else
                        {
                            Console.WriteLine($"房子已经被 【{searchResult.Owner}】占用");
                        }
                        return;
                    }
                    searchResult.Owner = customName;
                    //模拟并发操作
                    Thread.Sleep(100000);
                    
                    ctx.SaveChanges();
                    tx.Commit();
                    Console.WriteLine($"恭喜你 【{customName}】，抢到了");
                }
            }

            //乐观锁并发令牌方式处理
            using (var ctx = new MyDbContext())
            {
                Console.WriteLine("请输入预定房间号：");
                string roomNo = Console.ReadLine();
                Console.WriteLine("请输入您的名字：");
                string customName = Console.ReadLine();

                var searchResult = ctx.Houses.Single(x => x.Id == Convert.ToInt64(roomNo));
                if (!string.IsNullOrEmpty(searchResult.Owner))
                {
                    if (searchResult.Owner == customName)
                    {
                        Console.WriteLine("房子已经被你预定成功");
                    }
                    else
                    {
                        Console.WriteLine($"房子已经被 【{searchResult.Owner}】占用");
                    }
                    return;
                }
                searchResult.Owner = customName;
                //模拟并发操作
                Thread.Sleep(100000);
                try
                {
                    ctx.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Console.WriteLine("并发访问冲突");
                    var entry = ex.Entries.FirstOrDefault();
                    string oldValue = entry.OriginalValues.GetValue<string>("Owner"); //数据库原始值
                    string newValue = entry.GetDatabaseValues().GetValue<string>("Owner");//数据库现在值
                    string currentValue = entry.CurrentValues.GetValue<string>("Owner"); //当前内存值

                    Console.WriteLine($"数据库原始值：{oldValue} 数据库现在值：{newValue} 当前内存值：{currentValue}");
                    Console.WriteLine($"被【{newValue}】抢先了");
                }

                Console.WriteLine($"恭喜你 【{customName}】，抢到了");

            }

            //RowVersion方式（多字段并发处理）
            using(var ctx = new MyDbContext())
            {
                Console.WriteLine("请输入预定房间号：");
                string roomNo = Console.ReadLine();
                Console.WriteLine("请输入您的名字：");
                string customName = Console.ReadLine();

                var searchResult = ctx.Houses.Single(x => x.Id == Convert.ToInt64(roomNo));
                if (!string.IsNullOrEmpty(searchResult.Owner))
                {
                    if (searchResult.Owner == customName)
                    {
                        Console.WriteLine("房子已经被你预定成功");
                    }
                    else
                    {
                        Console.WriteLine($"房子已经被 【{searchResult.Owner}】占用");
                    }
                    return;
                }
                searchResult.Owner = customName;
                //模拟并发操作
                Thread.Sleep(100000);
                ctx.SaveChanges();
                Console.WriteLine($"恭喜你 【{customName}】，抢到了");
            }

            #endregion


            Console.ReadKey();
        }
    }
}
