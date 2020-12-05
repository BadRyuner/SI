﻿using System.Threading.Tasks;

namespace SIQuester.ViewModel.Core
{
    public static class UI
    {
        public static TaskScheduler Scheduler { get; private set; }

        public static void Initialize()
        {
            Scheduler = TaskScheduler.FromCurrentSynchronizationContext();
        }
    }
}
