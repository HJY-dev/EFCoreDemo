using System;

namespace Using资源释放
{
    /// <summary>
    /// 反编译查看 IL代码
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            using (A ctx = new A())
            using (C ctxc = new C())
            {
                // IL try finally 会释放资源
                throw new Exception("异常");
                Console.WriteLine("执行");
            }

            B _bb = new B();

            throw new Exception("异常");
            Console.ReadKey();
        }
    }

    class A : IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine("资源释放");
            Console.ReadKey();
        }
    }
    class B
    {

    }

    class C : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
