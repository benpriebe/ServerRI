#region Using directives

using System;
using AutoMapper;
using Contracts.Data;
using Core;

#endregion


namespace Services
{
    public abstract class BaseService
    {
        private readonly OperationContext _context;
        private readonly Func<IUnitOfWork> _uow;

        static BaseService()
        {
            Mapper = AutoMapperConfig.CreateMappingEngine();
        }

        protected BaseService(OperationContext context, Func<IUnitOfWork> uow)
        {
            _context = context;
            _uow = uow;
        }

        protected static MappingEngine Mapper { get; private set; }

        public Func<IUnitOfWork> UoW
        {
            get { return _uow; }
        }

        public OperationContext Context
        {
            get { return _context; }
        }
    }
}