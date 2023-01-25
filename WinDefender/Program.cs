// See https://aka.ms/new-console-template for more information

namespace WinDefender
{
    class Program
    {
        public static async Task Main(string[] args) 
        {
            CancellationToken cancellationToken = new CancellationToken();
            bool isVirus = await Jitbit.Utils.WinDefender.IsVirus(cancellationToken);
            Console.WriteLine(isVirus ? "System have a virus" : "System does not contain any virus");
        }
    }
}

