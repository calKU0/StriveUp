using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StriveUp.Shared.Interfaces
{
    public interface IMedalStateService
    {
        public event Action OnChange;
        int MedalsToClaim { get; }

        void SetMedalsCount(int count);
        void DecrementMedalsCount();
    }
}
