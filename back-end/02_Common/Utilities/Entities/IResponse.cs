using System.Collections.Generic;

namespace Newegg.MIS.API.Utilities.Entities
{
    public interface IResponse
    {
        bool? Succeeded { get; }
        List<Error> Errors { get; set; }
    }
}
