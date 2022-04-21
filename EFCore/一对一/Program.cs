using DbContents;
using Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace 一对一
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            #region 初始化数据
            using (MyDbContext ctx = new MyDbContext ())
            {
                if(await ctx.Orders.CountAsync()==0)
                {
                    Order order = new Order();
                    order.Address = "北京";
                    order.GoodsName = "书";

                    Delivery delivery = new Delivery();
                    delivery.CompanyName = "顺丰快递";
                    delivery.Number = "N123456789";
                    delivery.Order = order;

                    ctx.Orders.Add(order);
                    ctx.Deliverys.Add(delivery);
                    ctx.SaveChanges();
                }
            }

            #endregion

            #region FluentApi查询
            using(MyDbContext ctx = new MyDbContext())
            {
                var result =await ctx.Orders.Include(x=>x.Delivery).ToListAsync();
                result.ForEach(x => { Console.WriteLine($"{x.Id}{x.GoodsName}{x.Address}{x.Delivery.OrderId}{x.Delivery.CompanyName}{x.Delivery.Number}"); });
            }


            #endregion

            #region Linq查询
            using (MyDbContext ctx = new MyDbContext())
            {
                var result = (from order in ctx.Orders
                              join delivery in ctx.Deliverys
                              on order.Id equals delivery.OrderId
                              select new {
                                  Delivery = delivery,
                                  Order = order,
                              }).ToList();
                Console.ForegroundColor = ConsoleColor.Green;
                result.ForEach(x => { Console.WriteLine($"{x.Order.Id}{x.Order.GoodsName}{x.Order.Address}{x.Delivery.OrderId}{x.Delivery.CompanyName}{x.Delivery.Number}"); });

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
