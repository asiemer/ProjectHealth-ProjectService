using System.Collections.Generic;

namespace Projects.Domain
{
    public interface IPublisher
    {
        IEnumerable<object> GetPublicEvents();
        void ClearPublicEvents();
    }
}