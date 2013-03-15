#region Using directives

using System;
using AutoMapper;
using Contracts.Data;

#endregion


namespace Services
{
    public abstract class BaseService
    {
        private readonly Func<IUnitOfWork> _uow;

        static BaseService()
        {
            Mapper = AutoMapperConfig.CreateMappingEngine();
        }

        protected BaseService(Func<IUnitOfWork> uow)
        {
            _uow = uow;
        }

        protected static MappingEngine Mapper { get; private set; }

        public Func<IUnitOfWork> UoW
        {
            get { return _uow; }
        }
    }
}