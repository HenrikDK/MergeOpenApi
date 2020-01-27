using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MergeOpenApi.Merge
{
    public interface IScheduler
    {
        void ExecuteWithDelay(CancellationToken token, TimeSpan delay);
    }
    
    public class Scheduler : IScheduler
    {
        private readonly IProcessDeployedServices _processDeployedServices;
        private TimeSpan _delay = TimeSpan.FromMinutes(5);

        public Scheduler(IProcessDeployedServices processDeployedServices)
        {
            _processDeployedServices = processDeployedServices;
        }
        
        public void ExecuteWithDelay(CancellationToken token, TimeSpan delay)
        {
            _delay = delay;
            var stopWatch = new Stopwatch();
            while (!token.IsCancellationRequested)
            {
                stopWatch.Start();
                
                _processDeployedServices.Execute();

                WaitForNextExecution(token, stopWatch);
            }
        }

        private void WaitForNextExecution(CancellationToken token, Stopwatch stopWatch)
        {
            stopWatch.Stop();
            var elapsed = stopWatch.Elapsed;
            var calculated = _delay.Subtract(elapsed);
            stopWatch.Reset();

            if (elapsed < _delay)
            {
                Task.Delay(calculated).Wait(token);
            }
        }
    }
}
