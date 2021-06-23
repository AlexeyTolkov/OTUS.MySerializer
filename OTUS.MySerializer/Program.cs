using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Reflection;

namespace OTUS.MySerializer
{
    delegate void DisplayMessage(string message);

    class Program
    {
        const int iterCnt = 100000;
        MyClass myClass = MyClass.InitByDefault();

        static void Main(string[] args)
        {
            var myProgram = new Program();
            myProgram.RunMySerializer();
            myProgram.RunJSONSerializer();
        }

        private void RunMySerializer()
        {
            string csv = "";
            Object obj = null;
            Stopwatch stopWatch = new Stopwatch();
            long serTotalTime = 0;
            long deserTotalTime = 0;

            Console.WriteLine("Мой рефлекшен:");

            // Serializing
            stopWatch.Start();
            for (int i = 1; i <= iterCnt; i++)
            {
                csv = Serializer.SerializeFromObjectToCSV(myClass);
            }
            stopWatch.Stop();
            serTotalTime = stopWatch.ElapsedMilliseconds;

            // Deserializing
            stopWatch.Reset();
            stopWatch.Start();
            for (int i = 1; i <= iterCnt; i++)
            {
                obj = Serializer.DeserializeFromCSVToObject(myClass.GetType(), csv);
            }
            stopWatch.Stop();
            deserTotalTime = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"\tВремя на сериализацию = {serTotalTime} мс; для {iterCnt} итераций");
            Console.WriteLine($"\tВремя на десериализацию = {deserTotalTime}мс; для {iterCnt} итераций");
        }

        private void RunJSONSerializer()
        {
            string output = "";
            Object obj = null;
            Stopwatch stopWatch = new Stopwatch();
            long serTotalTime = 0;
            long deserTotalTime = 0;
            
            Console.WriteLine("Стандартный механизм(NewtonsoftJson):");
            // Serializing
            stopWatch.Start();
            for (int i = 1; i <= iterCnt; i++)
            {
                output = JsonConvert.SerializeObject(myClass);
            }
            stopWatch.Stop();
            serTotalTime = stopWatch.ElapsedMilliseconds;

            // Deserializing
            stopWatch.Reset();
            stopWatch.Start();
            for (int i = 1; i <= iterCnt; i++)
            {
                obj = JsonConvert.DeserializeObject<MyClass>(output);
            }
            stopWatch.Stop();
            deserTotalTime = stopWatch.ElapsedMilliseconds;

            Console.WriteLine($"\tВремя на сериализацию = {serTotalTime} мс; для {iterCnt} итераций");
            Console.WriteLine($"\tВремя на десериализацию = {deserTotalTime}мс; для {iterCnt} итераций");
        }
    }
}