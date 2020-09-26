using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartHub.WebApp.Util
{
    public class MultiEndpointConventionBuilder : List<IEndpointConventionBuilder>, IEndpointConventionBuilder
    {

        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var conventionBuilder in this)
                conventionBuilder.Add(convention);
        }

    }
}
