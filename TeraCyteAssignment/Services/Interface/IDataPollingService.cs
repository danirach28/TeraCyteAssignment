using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeraCyteAssignment.Models;

namespace TeraCyteAssignment.Services.Interface
{
    public interface IDataPollingService
    {
        event Action<ImageResultPaireData>? NewDataReceived;
        event Action<string>? ErrorOccurred;
        void StartPolling();
        void StopPolling();
    }
}
