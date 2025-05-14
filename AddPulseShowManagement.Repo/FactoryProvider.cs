using AddPulseShowManagement.Data.DBModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddPulseShowManagement.Repo
{
    public class FactoryProvider<T> : IFactory<T> where T : IRepository
    {
        
        public TBase CreateInstance<TBase>(IServiceProvider provider) where TBase : class
        {
            return provider.GetRequiredService<TBase>();
        }
    }
}
