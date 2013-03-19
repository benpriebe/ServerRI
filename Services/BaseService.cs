#region Using directives

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public List<ValidationResult> Validate(params object[] models)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            foreach (var model in models)
            {
                var vc = new ValidationContext(model, null, null);
                Validator.TryValidateObject(model, vc, validationResults);
            }
            return validationResults;
        }
    }
}