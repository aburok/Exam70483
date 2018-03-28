using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chapter1
{
    class DiningPhilisophersFirstAttempt
    {
        private static int philisophersCount = 5;
        private static int waitingForForkTimeInSec = 1;
        private static int eatingTimeInSec = 5;
        private const int MinThinkingTimeInSec = 3;
        private const bool ForkIsAvailable = false;
        private const bool ForkIsNBeingUsed = true;

        public static void Dine()
        {

            var forks = new ConcurrentDictionary<int, bool>();
            Enumerable.Range(0, 5).ToList()
                .ForEach(ind => forks.AddOrUpdate(ind, (i) => ForkIsAvailable, (i, b) => ForkIsAvailable));

            var rand = new Random();

            Task task;
            for (var i = 0; i < philisophersCount; i++)
            {
                var index = i;
                task = Task.Run(() =>
                {
                    var isWaitingForLeftFork = true;
                    var isWaitingForRightFork = true;

                    var leftForkIndex = index;
                    var rigthForkIndex = (index + 1) % (philisophersCount - 1);

                    var lastEaten = DateTime.UtcNow;

                    Console.WriteLine("#{0} was created.", index);

                    while (true)
                    {
                        Console.WriteLine("#{0} random thinking time, ", index);
                        //Thread.Sleep(MinThinkingTimeInSec * 1000 + rand.Next(1000));

                        while (isWaitingForLeftFork)
                        {
                            if (forks.TryUpdate(leftForkIndex, ForkIsNBeingUsed, ForkIsAvailable))
                            {
                                isWaitingForLeftFork = false;
                                Console.WriteLine("#{0} has left fork.", index);

                                // Just to end processor time slice for current thread, so other threads can execute
                                Thread.Sleep(0);
                            }
                            else
                            {
                                //Console.WriteLine("#{0} is waiting for the left fork", index);
                                Thread.Sleep(waitingForForkTimeInSec * 1000);
                            }

                            var hasntAteFor = (DateTime.UtcNow - lastEaten).Seconds;
                            if (hasntAteFor > 10)
                            {
                                Console.WriteLine("#{0} has eaten for {1} sec.", index, hasntAteFor);
                            }
                        }

                        while (isWaitingForRightFork)
                        {

                            if (forks.TryUpdate(rigthForkIndex, ForkIsNBeingUsed, ForkIsAvailable))
                            {
                                isWaitingForRightFork = false;
                                Console.WriteLine("#{0} has right fork.", index);

                                // Just to end processor time slice for current thread, so other threads can execute
                                Thread.Sleep(0);
                            }
                            else
                            {
                                //Console.WriteLine("#{0} is waiting for the right fork", index);
                                Thread.Sleep(waitingForForkTimeInSec * 1000);
                            }

                            var hasntAteFor = (DateTime.UtcNow - lastEaten).Seconds;
                            if (hasntAteFor > 10)
                            {
                                Console.WriteLine("#{0} has eaten for {1} sec.", index, hasntAteFor);
                            }
                        }

                        Console.WriteLine("#{0} is eating...", index);
                        Thread.Sleep(eatingTimeInSec * 1000);
                        lastEaten = DateTime.UtcNow;

                        forks.TryUpdate(leftForkIndex, ForkIsAvailable, ForkIsNBeingUsed);
                        isWaitingForLeftFork = true;
                        forks.TryUpdate(rigthForkIndex, ForkIsAvailable, ForkIsNBeingUsed);
                        isWaitingForRightFork = true;
                    }

                });

            }

            Console.ReadKey();
        }
    }
}