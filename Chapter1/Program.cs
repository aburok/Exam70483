using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter1
{
    class Program
    {
        static void Main(string[] args)
        {

            //DiningPhilisophersFirstAttempt.Dine();

            //SuccessCancelTask.Do();
            ;
            //ContinueWithTask.Do();

            //MeaningOfLifeQuestion.Do();

            //SimpleTask.Do();

            //ThreadPoolSimple.Do();

            //ThreadLocallyInitializedAttribute.Do();

            //ThreadWithStaticVariableAttribute.Do();

            //SafeThreadSAbort.Do();

            //UnsafeAbortThread.Do();

            //ParamethrizedThread.Do();

            //ForegroundThread();

            //SimpleCallThread();

            //Console.ReadLine();
        }


    }

    class SubTasksExecution
    {
        
    }

    class SuccessCancelTask
    {
        public static void Do()
        {
            var task = Task.Run(() =>
            {
                return 42;
            });
            task.ContinueWith((t) =>
            {
                Console.WriteLine("Canceled");
            }, TaskContinuationOptions.OnlyOnCanceled);
            task.ContinueWith(t =>
            {
                Console.WriteLine("On Foult");
            }, TaskContinuationOptions.OnlyOnFaulted);

            var completed = task.ContinueWith(t =>
            {
                Console.WriteLine("Completed");
            }, TaskContinuationOptions.OnlyOnRanToCompletion);

            completed.Wait();
            Console.WriteLine("result : {0}.", task.Result);
        }
    }

    class ContinueWithTask
    {
        public static void Do()
        {
            var task = Task.Run(() =>
            {
                return 42;
            }).ContinueWith((t) =>
            {
                return t.Result * 2;
            });

            // Wait is not needed as calling 'task.Result' will do the same. It will wait for the result.
            task.Wait();
            Console.WriteLine("Task result is {0}.", task.Result);
        }
    }

    class MeaningOfLifeQuestion
    {
        public static void Do()
        {
            var meaningOfLife = Task.Run(() =>
            {
                return 42;
            });

            meaningOfLife.Wait();

            var result = meaningOfLife.Result;

            Console.WriteLine("The meaning of life is {0}.", result);
        }
    }

    class SimpleTask
    {
        public static void Do()
        {
            // Task is better than Thread because you can get the Promise of Result 
            // Result -> object that holds the execution and result of Task

            // Task wraps the Thread Pool 

            var task = Task.Run(() =>
            {

                for (int i = 0; i < 100; i++)
                {
                    Console.Write("*");
                }
            });

            task.Wait();
        }
    }

    class ThreadPoolSimple
    {
        public static void Do()
        {
            ThreadPool.QueueUserWorkItem((s) =>
            {
                Console.WriteLine("Queued work item is being processed...");
            });

            // ReadKey is needed as Thread Pool is Background worker and will be terminated with calling thread.
            Console.ReadKey();
        }
    }

    class ThreadLocallyInitializedAttribute
    {
        private static ThreadLocal<int> threadId = new ThreadLocal<int>(() =>
        {
            return Thread.CurrentThread.ManagedThreadId;
        });

        public static void Do()
        {
            new Thread(() =>
            {
                for (int i = 0; i < threadId.Value; i++)
                {
                    Console.WriteLine("Thread A : {0}", i);
                }
            }).Start();

            new Thread(() =>
            {
                for (int i = 0; i < threadId.Value; i++)
                {
                    Console.WriteLine("Thread B : {0}", i);
                }
            }).Start();
        }
    }

    class ThreadWithStaticVariableAttribute
    {

        static int _globalField = 0;

        [ThreadStatic]
        static int _threadLocalField = 0;

        public static void Do()
        {
            new Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    _globalField++;
                    _threadLocalField++;
                    Console.WriteLine("Thread A : Global {0}, Local {1}.", _globalField, _threadLocalField);
                }
            }).Start();
            new Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    _globalField++;
                    _threadLocalField++;
                    Console.WriteLine("Thread B : Global {0}, Local {1}.", _globalField, _threadLocalField);
                }
            }).Start();
        }
    }

    class SafeThreadSAbort
    {
        public static void Do()
        {
            bool _canRun = true;

            Thread t = new Thread(() =>
            {
                while (_canRun)
                {
                    Console.WriteLine("Doing some work...");
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Safly ending background thread work");
            });

            t.Start();

            Thread.Sleep(3000);

            _canRun = false;
        }
    }

    class UnsafeAbortThread
    {
        public static void Do()
        {
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("Doing some work...");
                    Thread.Sleep(1000);
                }
            });
            t.Start();

            Thread.Sleep(3000);
            t.Abort();
        }
    }

    class ParamethrizedThread
    {

        public static void Do()
        {
            Thread t = new Thread(new ParameterizedThreadStart(ThreadMethod));

            t.Start(5);
        }

        static void ThreadMethod(object o)
        {
            for (int i = 0; i < (int)o; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                // This will end time for this thread assign by System level Process Scheduler
                Thread.Sleep(0);
            }
        }
    }

    class BackgroundThread
    {
        static void ForegroundThread()
        {
            Thread t = new Thread(SimpleThread.ThreadMethod);

            // This will run the thread as a background one. Background thread can be terminated before it finish its work.
            // It will be terminated when the main thread will finsih
            t.IsBackground = true;
            //t.IsBackground = false;
            t.Start(5);
            //t.Join();
        }

    }

    class SimpleThread
    {
        public static void Do()
        {
            SimpleCallThread();
        }

        static void SimpleCallThread()
        {
            Thread t = new Thread(ThreadMethod);
            t.Start();

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("Main thread: Do some work. " + i);
                // This will end time for this thread assign by System level Process Scheduler
                Thread.Sleep(0);
            }

            // Wait for all created threads to execute before executing the main thread.
            // Without it, the main thread will end even if the other threads are being executed
            t.Join();
        }

        public static void ThreadMethod()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("ThreadProc: {0}", i);
                // This will end time for this thread assign by System level Process Scheduler
                Thread.Sleep(0);
            }
        }
    }
}
