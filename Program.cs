using System;
using System.IO;

namespace Semantica
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Byte x = (Byte)256;
                Console.WriteLine(x);
                x++;
                x++;
                Console.WriteLine(x);
                Lenguaje a = new Lenguaje();

                a.Programa();

                //while(!a.FinArchivo())
                //{
                  //  a.NextToken();
                //}

                a.cerrar();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}