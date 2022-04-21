using DbContents;
using System;
using Entity;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data.Common;

namespace 一对多
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region 插入数据
            using (MyDbContext ctx = new MyDbContext())
            {
                //Article article = new Article ();
                //article.Title = ".NET内存性能分析指南";
                //article.Message = "本文旨在帮助.NET开发者，如何思考内存性能分析，并在需要时找到正确的方法来进行这种分析。在本文档中.NET的包括.NET Framework和.NET Core。为了在垃圾收集器和框架的其他部分获得最新的内存改进，我强烈建议你使用.NET Core，如果你还没有的话，因为那是应该尽快去升级的地方。";

                //Comment comment1 = new Comment { Content = "大佬66" };
                //Comment comment2 = new Comment { Content = "支持，先收藏有空看看" };
                //article.Comments.Add(comment1);
                //article.Comments.Add(comment2);

                //ctx.Articles.Add (article);
                //ctx.SaveChanges ();
            }
            #endregion

            #region FluentApi查询
            using (MyDbContext ctx = new MyDbContext())
            {
                var alist = ctx.Articles.AsQueryable().ToList();
                Console.ForegroundColor = ConsoleColor.Green;
                alist.ForEach(article => Console.WriteLine($"{article.Id + "," + article.Title}"));

                //关联查询
                var tlist = ctx.Articles.Include(a => a.Comments).Single(x => x.Id == 1);
                foreach (var item in tlist.Comments)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{item.Id},{item.Content}");
                }

                //关联查询
                Comment cmt = ctx.Comments.Include(c => c.C_Article).Single(x => x.Id == 1);
                Console.WriteLine($"{cmt.Id},{cmt.C_Article.Title},{cmt.C_Article.Message}");

                //只获取Id，Config配置HasForeignKey指定C_ArticleId，避免联查
                var cmt2 = ctx.Comments.Select(x=>new { x.Id,x.C_ArticleId}).Single(x=>x.Id == 1);
                Console.WriteLine($"{cmt2.Id},{cmt2.C_ArticleId}");
            }

            #endregion

            #region Linq查询
            using (MyDbContext ctx = new MyDbContext())
            {
                //左关联联查
                var tData = (from Article in ctx.Articles
                             join Comment in ctx.Comments
                             on Article.Id equals Comment.C_Article.Id into grouping
                             from p in grouping.DefaultIfEmpty()
                             where p.Id == 1
                             select new
                             {
                                 Article.Id,
                                 Article.Title,
                                 Article.Message,
                                 Article.Comments
                             }).FirstOrDefault();

                //内联查询
                var tData2 = (from Article in ctx.Articles
                              join Comment in ctx.Comments
                              on Article.Id equals Comment.C_Article.Id
                              where Article.Id == 1
                              select new
                              {
                                  Article.Id,
                                  Article.Title,
                                  Article.Message,
                                  Article.Comments
                              }).FirstOrDefault();

            }

            #endregion

            #region Dapper查询
            using (MyDbContext ctx = new MyDbContext())
            {
                var conn = ctx.Database.GetConnectionString();
                var items = ctx.Database.GetDbConnection().Query<Article>("select * from \"T_Articles\" where \"Id\"=@id ",new { id = 1});
            }

            #endregion

            #region 原生查询
            using (MyDbContext ctx = new MyDbContext())
            {
                DbConnection conn = ctx.Database.GetDbConnection();
                if(conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }
                using(var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "select * from \"T_Articles\" where \"Id\"=@id or \"Title\" = @title";
                    var p = cmd.CreateParameter();
                    p.ParameterName = "id";
                    p.Value = 1;
                    cmd.Parameters.Add(p);

                    var p2 = cmd.CreateParameter();
                    p2.ParameterName = "title";
                    p2.Value = ".NET内存";
                    cmd.Parameters.Add(p2);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var title = reader.GetString(1);
                            Console.WriteLine($"文章标题：{title}");
                        }
                    }
                }
            }

            #endregion

            Console.ReadLine();
        }
    }
}
