using FanOutFanIn.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanOutFanIn.Service.Interfaces
{
    public interface ICalculateProbabilityService
    {
        Task<SentimentResult> Calculate(int userId);
    }
}
