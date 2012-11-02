//-------------------------------------------------------------------------------
// <copyright file="Worker.cs" company="Appccelerate">
//   Copyright (c) 2008-2012
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-------------------------------------------------------------------------------

namespace ActiveObjectBlock
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Threading.Tasks.Dataflow;

    public class Worker
    {
        private readonly Action<int> action;

        private readonly List<int> buffer;
        
        private ActionBlock<int> actionBlock;

        private ActionBlock<Task<Action>> actionExecuter;

        private bool running;

        private CancellationTokenSource cancellationTokenSource;

        ConcurrentExclusiveSchedulerPair taskScheduler = new ConcurrentExclusiveSchedulerPair();

        public Worker(Action<int> action)
        {
            this.action = action;

            this.buffer = new List<int>();

            this.actionExecuter = new ActionBlock<Task<Action>>(t => t.Start());
        }

        public Task Start()
        {
            Task task = new Task(this.SynchronizedStart);

            task.Start(this.taskScheduler.ExclusiveScheduler);

            return task;
        }

        public Task Stop()
        {
            Task task = new Task(this.SynchronizedStop);

            task.Start(this.taskScheduler.ExclusiveScheduler);

            return task;
        }

        public void Abort()
        {
            this.cancellationTokenSource.Cancel();

            this.running = false;
        }

        public Task Post(int i)
        {
            Task task = new Task(() => this.Queue(i));

            task.Start(this.taskScheduler.ExclusiveScheduler);

            return task;
        }

        private void SynchronizedStart()
        {
            if (this.running)
            {
                return;
            }

            this.cancellationTokenSource = new CancellationTokenSource();
            this.actionBlock = new ActionBlock<int>(this.action, new ExecutionDataflowBlockOptions { CancellationToken = this.cancellationTokenSource.Token });

            foreach (var i in this.buffer)
            {
                this.actionBlock.Post(i);
            }

            this.running = true;

            this.buffer.Clear();
        }

        private void SynchronizedStop()
        {
            if (!this.running)
            {
                return;
            }

            var task = this.actionBlock.Completion;

            this.actionBlock.Complete();

            this.running = false;

            task.Wait();
        }

        private void Queue(int i)
        {
            if (this.running)
            {
                this.actionBlock.Post(i);
            }
            else
            {
                this.buffer.Add(i);
            }
        }
    }
}