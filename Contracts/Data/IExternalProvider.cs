#region Using directives

using System.Collections.Generic;
using System.Linq;
using Contracts.Models;
using Data.Entities;

#endregion


namespace Contracts.Data
{
    public interface IExternalProvider 
    {
        IList<Widget> GetWidgets();
    }
}