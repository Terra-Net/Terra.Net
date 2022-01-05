using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Terra.Net.Crypto.Implemetations;
using System.Runtime.CompilerServices;
namespace Terra.Net.Crypto
{
    public static class Program
    {
        static readonly string _42 = "42424242";
        static readonly string p2 = "0l23456789";
        static readonly string art = "artemartem";
        static readonly string ridicoulous = "rldlcoulous";

        static readonly string sat = "satoshlnakamoto";

        static readonly Regex regex = new Regex(@"(\w)\1{6,}");
        static ulong count = 0;
        static ulong generated = 0;
        private static readonly Timer _timer = new Timer(DoWork, null, TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(60));
        private static DateTime _date = DateTime.UtcNow;
        private static void DoWork(object? state)
        {
            Console.WriteLine($"{DateTime.UtcNow}: Generated {count} total and {generated} addreses. APS: {(int)(count / DateTime.UtcNow.Subtract(_date).TotalSeconds)}/s");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Main(params string[] args)
        {
            var t = "1sdfsdf1k1b23hm51jbv1l1k1jk1v1f1o1y1i1oo1o11c1d1gh1415161981".GroupBy(c => c).Select(c=> new { c.Key,c=c.Count()}).ToList();
            While(new ParallelOptions { MaxDegreeOfParallelism = 16 }, () => generated < 100, b => Run());
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Run()
        {
            Interlocked.Increment(ref count);
            var mnm = new MnemonicKey();

            var address = Bech32.Encode("terra", mnm.RawAddress);
           
            if (regex.IsMatch(address)
                || address.StartsWith("terra1ARTEM42",StringComparison.OrdinalIgnoreCase)
                || address.GroupBy(c => c).Any(c => c.Count() > 12) 
                || address.Contains(_42, StringComparison.OrdinalIgnoreCase)
                  || address.Contains(art, StringComparison.OrdinalIgnoreCase)
                  || address.Contains(sat, StringComparison.OrdinalIgnoreCase)
                  || address.Contains(ridicoulous, StringComparison.OrdinalIgnoreCase)

                || address.Contains(p2, StringComparison.OrdinalIgnoreCase))
            {
                Interlocked.Increment(ref generated);

                File.WriteAllText($"C:\\terra\\generated\\{address}.txt", mnm.Mnemonic);
            }
            // Console.WriteLine(count);
        }

        public static void While(
    ParallelOptions parallelOptions, Func<bool> condition,
    Action<ParallelLoopState> body)
        {
            Parallel.ForEach(Infinite(), parallelOptions, (ignored, loopState) =>
            {
                if (condition()) body(loopState);
                else loopState.Stop();
            });
        }

        private static IEnumerable<bool> Infinite()
        {
            while (true) yield return true;
        }
    }
}
