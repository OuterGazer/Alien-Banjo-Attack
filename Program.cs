using System;

namespace Alien_Banjo_Attack
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BanjoAttack game = new BanjoAttack())
            {
                game.Run();
            }
        }
    }
#endif
}

